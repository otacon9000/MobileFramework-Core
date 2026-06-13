using System.Collections;
using MobileFramework.Core.Bootstrap;
using MobileFramework.Core.Contracts;
using MobileFramework.Core.Managers.Lifecycle;
using MobileFramework.Core.StateMachine.States;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MobileFramework.Tests.PlayMode.Lifecycle
{
    /// <summary>IMiniGame minimo per i test PlayMode (i Fake* vivono nell'assembly EditMode).</summary>
    public sealed class TestMiniGame : IMiniGame
    {
        public string GameId => "playmode_test_game";

        public int PauseCalls;
        public int ResumeCalls;

        public void Initialize(GameContext context)
        {
        }

        public void StartGame()
        {
        }

        public void PauseGame() => PauseCalls++;

        public void ResumeGame() => ResumeCalls++;

        public void Tick(float deltaTime)
        {
        }

        public void EndGame(GameOverReason reason)
        {
        }

        public void Cleanup()
        {
        }
    }

    /// <summary>Base condivisa: avvia un CoreBootstrapper reale e lo smonta tra i test.</summary>
    public abstract class LifecycleTestBase
    {
        protected GameObject CoreGo;
        protected CoreBootstrapper Bootstrapper;
        protected AppLifecycleHandler Lifecycle;
        protected TestMiniGame MiniGame;

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            CoreGo = new GameObject("[Core under test]");
            Bootstrapper = CoreGo.AddComponent<CoreBootstrapper>();
            MiniGame = new TestMiniGame();
            Bootstrapper.RegisterMiniGame(MiniGame);
            Lifecycle = CoreGo.GetComponent<AppLifecycleHandler>();

            yield return null; // lascia girare Start(): Boot → MainMenu
        }

        [UnityTearDown]
        public IEnumerator UnityTearDown()
        {
            Object.Destroy(CoreGo);
            yield return null; // OnDestroy svuota ServiceLocator e azzera il singleton
        }

        protected void DriveToPlaying()
        {
            ((MainMenuState)Bootstrapper.GameManager.CurrentState).StartGame();
        }
    }

    public class AppLifecycleTests : LifecycleTestBase
    {
        [UnityTest]
        public IEnumerator Bootstrap_ReachesMainMenu()
        {
            Assert.IsTrue(Bootstrapper.IsInitialized);
            Assert.IsInstanceOf<MainMenuState>(Bootstrapper.GameManager.CurrentState);
            yield break;
        }

        [UnityTest]
        public IEnumerator PauseDuringGameplay_EntersOSInterrupt_AndPausesMiniGame()
        {
            DriveToPlaying();
            Assert.IsInstanceOf<PlayingState>(Bootstrapper.GameManager.CurrentState);

            Lifecycle.HandleInterrupt();

            Assert.IsInstanceOf<OSInterruptState>(Bootstrapper.GameManager.CurrentState);
            Assert.AreEqual(1, MiniGame.PauseCalls);
            yield break;
        }

        [UnityTest]
        public IEnumerator ResumeAfterInterrupt_LandsInPausedState_NotPlaying()
        {
            DriveToPlaying();
            Lifecycle.HandleInterrupt();

            Lifecycle.HandleResume();

            Assert.IsInstanceOf<PausedState>(Bootstrapper.GameManager.CurrentState);
            Assert.AreEqual(1, MiniGame.PauseCalls, "il rientro non deve ri-pausare il minigame");
            Assert.AreEqual(0, MiniGame.ResumeCalls, "la ripresa è una scelta del giocatore, non automatica");
            yield break;
        }

        [UnityTest]
        public IEnumerator PlayerResume_FromPausedAfterInterrupt_ResumesMiniGame()
        {
            DriveToPlaying();
            Lifecycle.HandleInterrupt();
            Lifecycle.HandleResume();

            ((PausedState)Bootstrapper.GameManager.CurrentState).Resume();

            Assert.IsInstanceOf<PlayingState>(Bootstrapper.GameManager.CurrentState);
            Assert.AreEqual(1, MiniGame.ResumeCalls);
            yield break;
        }
    }
}
