using System;
using System.Collections.Generic;
using MobileFramework.Core.Bootstrap;
using MobileFramework.Core.Contracts;
using MobileFramework.Core.Events;
using MobileFramework.Core.StateMachine;
using MobileFramework.Core.StateMachine.States;
using MobileFramework.Tests.EditMode.Fakes;
using NUnit.Framework;

namespace MobileFramework.Tests.EditMode.StateMachine
{
    public class GameManagerTests
    {
        private sealed class Harness
        {
            public GameManager Manager;
            public FakeMiniGame Game;
            public FakeUIManager UI;
            public FakeInputManager Input;
            public EventBus Bus;
        }

        private static Harness Create()
        {
            var bus = new EventBus();
            var ui = new FakeUIManager();
            var input = new FakeInputManager();
            var context = new GameContext(
                bus,
                new FakeAudioManager(),
                ui,
                new FakeSaveSystem(),
                input,
                new FakeHapticManager(),
                new FakeSettingsManager());

            var manager = new GameManager(context);
            manager.RegisterStates(
                new BootState(),
                new MainMenuState(),
                new LoadingState(),
                new PlayingState(),
                new PausedState(),
                new OSInterruptState(),
                new GameOverState(),
                new RewardedState(),
                new UnloadingState());

            var game = new FakeMiniGame();
            manager.RegisterMiniGame(game);

            return new Harness { Manager = manager, Game = game, UI = ui, Input = input, Bus = bus };
        }

        private static void DriveToPlaying(Harness h)
        {
            h.Manager.ChangeState<BootState>();
            ((MainMenuState)h.Manager.CurrentState).StartGame();
        }

        [Test]
        public void Boot_ChainsToMainMenu_AndShowsMenuPanel()
        {
            var h = Create();

            h.Manager.ChangeState<BootState>();

            Assert.IsInstanceOf<MainMenuState>(h.Manager.CurrentState);
            Assert.AreEqual("main_menu", h.UI.CurrentPanelId);
        }

        [Test]
        public void InvalidTransition_ReturnsFalse_AndStateIsUnchanged()
        {
            var h = Create();
            h.Manager.ChangeState<BootState>();

            var result = h.Manager.ChangeState<PausedState>();

            Assert.IsFalse(result);
            Assert.IsInstanceOf<MainMenuState>(h.Manager.CurrentState);
        }

        [Test]
        public void UnregisteredState_ReturnsFalse()
        {
            var bus = new EventBus();
            var context = new GameContext(
                bus, new FakeAudioManager(), new FakeUIManager(), new FakeSaveSystem(),
                new FakeInputManager(), new FakeHapticManager(), new FakeSettingsManager());
            var manager = new GameManager(context);

            Assert.IsFalse(manager.ChangeState<BootState>());
            Assert.IsNull(manager.CurrentState);
        }

        [Test]
        public void HappyPath_StartGame_InitializesAndStartsMiniGame()
        {
            var h = Create();

            DriveToPlaying(h);

            Assert.IsInstanceOf<PlayingState>(h.Manager.CurrentState);
            Assert.AreEqual(1, h.Game.InitializeCalls);
            Assert.AreEqual(1, h.Game.StartCalls);
            Assert.AreSame(h.Manager.Context, h.Game.LastContext);
            Assert.IsTrue(h.Manager.IsSessionActive);
            Assert.IsTrue(h.UI.IsOpen("hud"));
        }

        [Test]
        public void Loading_WithoutMiniGame_FallsBackToMainMenu()
        {
            var h = Create();
            h.Manager.RegisterMiniGame(null);
            h.Manager.ChangeState<BootState>();

            ((MainMenuState)h.Manager.CurrentState).StartGame();

            Assert.IsInstanceOf<MainMenuState>(h.Manager.CurrentState);
        }

        [Test]
        public void Tick_InPlaying_ForwardsToMiniGame()
        {
            var h = Create();
            DriveToPlaying(h);

            h.Manager.Tick(0.5f);
            h.Manager.Tick(0.25f);

            Assert.AreEqual(2, h.Game.TickCalls);
            Assert.AreEqual(0.75f, h.Game.TotalTickTime, 0.0001f);
        }

        [Test]
        public void Tick_InMainMenu_DoesNotReachMiniGame()
        {
            var h = Create();
            h.Manager.ChangeState<BootState>();

            h.Manager.Tick(1f);

            Assert.AreEqual(0, h.Game.TickCalls);
        }

        [Test]
        public void PauseAndResume_CallMiniGameOnce_AndToggleInput()
        {
            var h = Create();
            DriveToPlaying(h);

            ((PlayingState)h.Manager.CurrentState).Pause();
            Assert.IsInstanceOf<PausedState>(h.Manager.CurrentState);
            Assert.AreEqual(1, h.Game.PauseCalls);
            Assert.IsFalse(h.Input.InputEnabled);
            Assert.IsTrue(h.UI.IsOpen("pause"));

            ((PausedState)h.Manager.CurrentState).Resume();
            Assert.IsInstanceOf<PlayingState>(h.Manager.CurrentState);
            Assert.AreEqual(1, h.Game.ResumeCalls);
            Assert.AreEqual(1, h.Game.StartCalls, "ResumeGame non deve ri-chiamare StartGame");
            Assert.IsTrue(h.Input.InputEnabled);
            Assert.IsFalse(h.UI.IsOpen("pause"));
        }

        [Test]
        public void GameOverEvent_DuringPlaying_TransitionsAndEndsGameWithReason()
        {
            var h = Create();
            DriveToPlaying(h);

            h.Bus.Emit(new GameOverEvent(GameOverReason.Timeout, score: 99));

            Assert.IsInstanceOf<GameOverState>(h.Manager.CurrentState);
            Assert.AreEqual(1, h.Game.EndCalls);
            Assert.AreEqual(GameOverReason.Timeout, h.Game.LastEndReason);
            Assert.IsFalse(h.Manager.IsSessionActive);
            Assert.IsTrue(h.UI.IsOpen("game_over"));
        }

        [Test]
        public void GameOverEvent_OutsidePlaying_IsIgnored()
        {
            var h = Create();
            h.Manager.ChangeState<BootState>();

            h.Bus.Emit(new GameOverEvent(GameOverReason.Lose));

            Assert.IsInstanceOf<MainMenuState>(h.Manager.CurrentState);
            Assert.AreEqual(0, h.Game.EndCalls);
        }

        [Test]
        public void OSInterrupt_FromPlaying_PausesGame_AndResumesIntoPausedState()
        {
            var h = Create();
            DriveToPlaying(h);

            h.Manager.NotifyOSInterrupt();
            Assert.IsInstanceOf<OSInterruptState>(h.Manager.CurrentState);
            Assert.AreEqual(1, h.Game.PauseCalls);

            h.Manager.NotifyOSResume();
            Assert.IsInstanceOf<PausedState>(h.Manager.CurrentState);
            Assert.AreEqual(1, h.Game.PauseCalls, "il rientro in pausa non deve ri-pausare il minigame");
        }

        [Test]
        public void OSInterrupt_InMainMenu_IsIgnored()
        {
            var h = Create();
            h.Manager.ChangeState<BootState>();

            h.Manager.NotifyOSInterrupt();

            Assert.IsInstanceOf<MainMenuState>(h.Manager.CurrentState);
        }

        [Test]
        public void Unloading_CleansUp_ClearsBus_AndReturnsToMenu()
        {
            var h = Create();
            DriveToPlaying(h);

            var orphanCalls = 0;
            h.Bus.Subscribe<SceneLoadProgressEvent>(_ => orphanCalls++);

            h.Manager.ChangeState<UnloadingState>();

            Assert.IsInstanceOf<MainMenuState>(h.Manager.CurrentState);
            Assert.AreEqual(1, h.Game.CleanupCalls);
            Assert.IsFalse(h.Manager.IsSessionActive);
            Assert.AreEqual(1, h.UI.StackDepth, "dopo l'unloading resta solo il menu");

            h.Bus.Emit(new SceneLoadProgressEvent("x", 1f));
            Assert.AreEqual(0, orphanCalls, "Clear() deve aver rimosso i listener della sessione");
        }

        [Test]
        public void AfterUnloading_GameOverStillWorks_InNextSession()
        {
            var h = Create();
            DriveToPlaying(h);
            h.Manager.ChangeState<UnloadingState>();

            // seconda sessione: il GameManager deve essersi riregistrato dopo Clear()
            ((MainMenuState)h.Manager.CurrentState).StartGame();
            h.Bus.Emit(new GameOverEvent(GameOverReason.Lose));

            Assert.IsInstanceOf<GameOverState>(h.Manager.CurrentState);
            Assert.AreEqual(1, h.Game.EndCalls);
        }

        [Test]
        public void RewardedContinue_ResumesSession()
        {
            var h = Create();
            DriveToPlaying(h);
            h.Bus.Emit(new GameOverEvent(GameOverReason.Lose));

            ((GameOverState)h.Manager.CurrentState).OfferRewarded();
            Assert.IsInstanceOf<RewardedState>(h.Manager.CurrentState);

            ((RewardedState)h.Manager.CurrentState).CompleteReward(resumeGameplay: true);

            Assert.IsInstanceOf<PlayingState>(h.Manager.CurrentState);
            Assert.IsTrue(h.Manager.IsSessionActive);
            Assert.AreEqual(1, h.Game.ResumeCalls);
        }

        [Test]
        public void StateChanges_EmitGameStateChangedEvents()
        {
            var h = Create();
            var transitions = new List<(Type from, Type to)>();
            h.Bus.Subscribe<GameStateChangedEvent>(e => transitions.Add((e.PreviousState, e.NewState)));

            h.Manager.ChangeState<BootState>();

            Assert.AreEqual(2, transitions.Count);
            Assert.IsNull(transitions[0].from);
            Assert.AreEqual(typeof(BootState), transitions[0].to);
            Assert.AreEqual(typeof(BootState), transitions[1].from);
            Assert.AreEqual(typeof(MainMenuState), transitions[1].to);
        }
    }
}
