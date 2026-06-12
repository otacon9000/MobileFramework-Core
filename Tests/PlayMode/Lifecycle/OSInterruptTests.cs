using System.Collections;
using MobileFramework.Core.StateMachine.States;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace MobileFramework.Tests.PlayMode.Lifecycle
{
    public class OSInterruptTests : LifecycleTestBase
    {
        [UnityTest]
        public IEnumerator Interrupt_InMainMenu_IsIgnored()
        {
            Lifecycle.HandleInterrupt();

            Assert.IsInstanceOf<MainMenuState>(Bootstrapper.GameManager.CurrentState);
            yield break;
        }

        [UnityTest]
        public IEnumerator Interrupt_WhilePaused_ReturnsToPausedOnResume()
        {
            DriveToPlaying();
            ((PlayingState)Bootstrapper.GameManager.CurrentState).Pause();
            Assert.AreEqual(1, MiniGame.PauseCalls);

            Lifecycle.HandleInterrupt();
            Assert.IsInstanceOf<OSInterruptState>(Bootstrapper.GameManager.CurrentState);
            Assert.AreEqual(1, MiniGame.PauseCalls, "già in pausa: nessuna seconda PauseGame");

            Lifecycle.HandleResume();
            Assert.IsInstanceOf<PausedState>(Bootstrapper.GameManager.CurrentState);
            yield break;
        }

        [UnityTest]
        public IEnumerator Resume_WithoutInterrupt_DoesNothing()
        {
            DriveToPlaying();

            Lifecycle.HandleResume();

            Assert.IsInstanceOf<PlayingState>(Bootstrapper.GameManager.CurrentState);
            yield break;
        }

        [UnityTest]
        public IEnumerator RepeatedInterrupts_AreIdempotent()
        {
            DriveToPlaying();

            Lifecycle.HandleInterrupt();
            Lifecycle.HandleInterrupt(); // secondo evento (pause + focus loss): nessun effetto

            Assert.IsInstanceOf<OSInterruptState>(Bootstrapper.GameManager.CurrentState);
            Assert.AreEqual(1, MiniGame.PauseCalls);
            yield break;
        }
    }
}
