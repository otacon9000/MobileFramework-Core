using MobileFramework.Core.StateMachine;
using MobileFramework.Core.StateMachine.States;
using MobileFramework.Core.StateMachine.Transitions;
using NUnit.Framework;

namespace MobileFramework.Tests.EditMode.StateMachine
{
    public class StateTransitionTests
    {
        private sealed class CustomTutorialState : GameState
        {
        }

        private StateTransitionValidator _validator;

        [SetUp]
        public void SetUp() => _validator = StateTransitionValidator.CreateDefault();

        [Test]
        public void FirstTransition_FromNull_IsAlwaysAllowed()
        {
            Assert.IsTrue(_validator.CanTransition(null, typeof(BootState)));
            Assert.IsTrue(_validator.CanTransition(null, typeof(PlayingState)));
        }

        [TestCase(typeof(BootState), typeof(MainMenuState))]
        [TestCase(typeof(MainMenuState), typeof(LoadingState))]
        [TestCase(typeof(LoadingState), typeof(PlayingState))]
        [TestCase(typeof(PlayingState), typeof(PausedState))]
        [TestCase(typeof(PlayingState), typeof(GameOverState))]
        [TestCase(typeof(PlayingState), typeof(OSInterruptState))]
        [TestCase(typeof(PausedState), typeof(PlayingState))]
        [TestCase(typeof(GameOverState), typeof(RewardedState))]
        [TestCase(typeof(GameOverState), typeof(LoadingState))]
        [TestCase(typeof(RewardedState), typeof(PlayingState))]
        [TestCase(typeof(UnloadingState), typeof(MainMenuState))]
        public void DefaultRules_AllowStandardFlow(System.Type from, System.Type to)
        {
            Assert.IsTrue(_validator.CanTransition(from, to), $"{from.Name} → {to.Name} dovrebbe essere permessa");
        }

        [TestCase(typeof(BootState), typeof(PlayingState))]
        [TestCase(typeof(MainMenuState), typeof(PausedState))]
        [TestCase(typeof(MainMenuState), typeof(GameOverState))]
        [TestCase(typeof(PlayingState), typeof(MainMenuState))]
        [TestCase(typeof(GameOverState), typeof(PlayingState))]
        [TestCase(typeof(UnloadingState), typeof(PlayingState))]
        public void DefaultRules_DenyInvalidJumps(System.Type from, System.Type to)
        {
            Assert.IsFalse(_validator.CanTransition(from, to), $"{from.Name} → {to.Name} dovrebbe essere negata");
        }

        [Test]
        public void AddRule_ExtendsDefaultRules_ForGameSpecificStates()
        {
            Assert.IsFalse(_validator.CanTransition(typeof(MainMenuState), typeof(CustomTutorialState)));

            _validator.AddRule<MainMenuState, CustomTutorialState>();
            _validator.AddRule(typeof(CustomTutorialState), typeof(LoadingState));

            Assert.IsTrue(_validator.CanTransition(typeof(MainMenuState), typeof(CustomTutorialState)));
            Assert.IsTrue(_validator.CanTransition(typeof(CustomTutorialState), typeof(LoadingState)));
            // le regole esistenti restano intatte
            Assert.IsTrue(_validator.CanTransition(typeof(MainMenuState), typeof(LoadingState)));
        }
    }
}
