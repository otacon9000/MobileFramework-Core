using System;
using System.Collections.Generic;
using MobileFramework.Core.StateMachine.States;

namespace MobileFramework.Core.StateMachine.Transitions
{
    /// <summary>
    /// Regole di transizione della state machine: da quale stato si può andare dove.
    /// La prima transizione (stato corrente null) è sempre permessa.
    /// I giochi estendono le regole con AddRule per i propri stati.
    /// </summary>
    public sealed class StateTransitionValidator
    {
        private readonly Dictionary<Type, HashSet<Type>> _rules = new Dictionary<Type, HashSet<Type>>();

        public void AddRule(Type from, params Type[] to)
        {
            if (!_rules.TryGetValue(from, out var allowed))
            {
                allowed = new HashSet<Type>();
                _rules[from] = allowed;
            }

            foreach (var target in to)
            {
                allowed.Add(target);
            }
        }

        public void AddRule<TFrom, TTo>()
            where TFrom : GameState
            where TTo : GameState
        {
            AddRule(typeof(TFrom), typeof(TTo));
        }

        public bool CanTransition(Type from, Type to)
        {
            if (from == null)
            {
                return true;
            }

            return _rules.TryGetValue(from, out var allowed) && allowed.Contains(to);
        }

        /// <summary>Regole del flusso standard del Core.</summary>
        public static StateTransitionValidator CreateDefault()
        {
            var v = new StateTransitionValidator();

            v.AddRule(typeof(BootState), typeof(MainMenuState));
            v.AddRule(typeof(MainMenuState), typeof(LoadingState));
            v.AddRule(typeof(LoadingState), typeof(PlayingState), typeof(MainMenuState));
            v.AddRule(typeof(PlayingState), typeof(PausedState), typeof(OSInterruptState), typeof(GameOverState), typeof(UnloadingState));
            v.AddRule(typeof(PausedState), typeof(PlayingState), typeof(OSInterruptState), typeof(UnloadingState));
            v.AddRule(typeof(OSInterruptState), typeof(PlayingState), typeof(PausedState), typeof(MainMenuState), typeof(UnloadingState));
            v.AddRule(typeof(GameOverState), typeof(RewardedState), typeof(LoadingState), typeof(UnloadingState));
            v.AddRule(typeof(RewardedState), typeof(PlayingState), typeof(GameOverState), typeof(UnloadingState));
            v.AddRule(typeof(UnloadingState), typeof(MainMenuState));

            return v;
        }
    }
}
