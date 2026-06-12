using MobileFramework.Core.Managers.UI;

namespace MobileFramework.Core.StateMachine.States
{
    public class MainMenuState : GameState
    {
        public override void Enter()
        {
            Context.Input.InputEnabled = true;
            Context.UI.PopToRoot();
            Context.UI.Push(CorePanelIds.MainMenu);
        }

        /// <summary>Chiamato dal pulsante Play del menu.</summary>
        public void StartGame()
        {
            Manager.ChangeState<LoadingState>();
        }
    }
}
