using MobileFramework.Core.Bootstrap;
using MobileFramework.Core.StateMachine;
using MobileFramework.Core.StateMachine.States;

namespace MobileFramework.Core.Managers.UI.Panels
{
    /// <summary>
    /// Menu principale di base. I giochi lo sostituiscono registrando un proprio
    /// pannello con id CorePanelIds.MainMenu (via IUISlot).
    /// I metodi On*Clicked si collegano ai Button da inspector.
    /// </summary>
    public class MainMenuPanel : UIPanel
    {
        protected override string DefaultPanelId => CorePanelIds.MainMenu;

        public void OnPlayClicked()
        {
            if (ServiceLocator.Instance.TryGet<GameManager>(out var gameManager)
                && gameManager.CurrentState is MainMenuState menu)
            {
                menu.StartGame();
            }
        }
    }
}
