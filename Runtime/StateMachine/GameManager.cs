using System;
using System.Collections.Generic;
using MobileFramework.Core.Bootstrap;
using MobileFramework.Core.Contracts;
using MobileFramework.Core.Events;
using MobileFramework.Core.StateMachine.States;
using MobileFramework.Core.StateMachine.Transitions;
using UnityEngine;

namespace MobileFramework.Core.StateMachine
{
    /// <summary>
    /// Macchina a stati del framework, unico punto di controllo del flusso di gioco.
    /// Classe pura (non MonoBehaviour): il CoreBootstrapper la crea e la tick-a,
    /// i test EditMode la istanziano direttamente con i Fake*.
    /// </summary>
    public sealed class GameManager
    {
        private readonly Dictionary<Type, GameState> _states = new Dictionary<Type, GameState>();

        public GameContext Context { get; }
        public StateTransitionValidator Validator { get; }
        public GameState CurrentState { get; private set; }
        public IMiniGame MiniGame { get; private set; }

        /// <summary>True tra StartGame e EndGame/Cleanup del minigame.</summary>
        public bool IsSessionActive { get; internal set; }

        /// <summary>True quando PauseGame è stato chiamato e ResumeGame non ancora. Evita doppie pause (pausa utente + interruzione OS).</summary>
        public bool IsMiniGamePaused { get; internal set; }

        public GameOverReason LastGameOverReason { get; private set; }

        /// <summary>Stato in cui ci si trovava quando è arrivata l'interruzione OS.</summary>
        public Type StateBeforeInterrupt { get; private set; }

        public GameManager(GameContext context, StateTransitionValidator validator = null)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Validator = validator ?? StateTransitionValidator.CreateDefault();
            SubscribeCoreEvents();
        }

        /// <summary>
        /// Sottoscrive gli eventi Core sul bus. Pubblico perché UnloadingState
        /// chiama EventBus.Clear() e il GameManager deve riregistrarsi subito.
        /// </summary>
        public void SubscribeCoreEvents()
        {
            Context.Events.Subscribe<GameOverEvent>(OnGameOverEvent);
        }

        public void RegisterState(GameState state)
        {
            if (state == null)
            {
                return;
            }

            state.Attach(this);
            _states[state.GetType()] = state;
        }

        public void RegisterStates(params GameState[] states)
        {
            foreach (var state in states)
            {
                RegisterState(state);
            }
        }

        public void RegisterMiniGame(IMiniGame miniGame) => MiniGame = miniGame;

        public bool ChangeState<T>() where T : GameState => ChangeState(typeof(T));

        public bool ChangeState(Type stateType)
        {
            if (!_states.TryGetValue(stateType, out var next))
            {
                Debug.LogWarning($"[GameManager] Stato {stateType.Name} non registrato.");
                return false;
            }

            var fromType = CurrentState?.GetType();
            if (fromType == stateType)
            {
                return false;
            }

            if (!Validator.CanTransition(fromType, stateType))
            {
                Debug.LogWarning($"[GameManager] Transizione non valida: {fromType?.Name ?? "null"} → {stateType.Name}.");
                return false;
            }

            CurrentState?.Exit();
            CurrentState = next;
            Context.Events.Emit(new GameStateChangedEvent(fromType, stateType));
            next.Enter();
            return true;
        }

        public void Tick(float deltaTime) => CurrentState?.Tick(deltaTime);

        public bool IsIn<T>() where T : GameState => CurrentState is T;

        /// <summary>Chiamato da AppLifecycleHandler quando l'app perde focus o va in pausa.</summary>
        public void NotifyOSInterrupt()
        {
            if (CurrentState is PlayingState || CurrentState is PausedState)
            {
                StateBeforeInterrupt = CurrentState.GetType();
                ChangeState<OSInterruptState>();
            }
        }

        /// <summary>
        /// Chiamato da AppLifecycleHandler al rientro. Dal gameplay si rientra
        /// sempre in pausa: la ripresa è una scelta esplicita del giocatore.
        /// </summary>
        public void NotifyOSResume()
        {
            if (!(CurrentState is OSInterruptState))
            {
                return;
            }

            var target = StateBeforeInterrupt == typeof(PlayingState)
                ? typeof(PausedState)
                : StateBeforeInterrupt ?? typeof(MainMenuState);

            ChangeState(target);
        }

        private void OnGameOverEvent(GameOverEvent evt)
        {
            LastGameOverReason = evt.Reason;

            if (CurrentState is PlayingState)
            {
                ChangeState<GameOverState>();
            }
        }
    }
}
