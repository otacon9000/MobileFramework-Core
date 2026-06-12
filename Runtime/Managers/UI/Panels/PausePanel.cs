using MobileFramework.Core.Bootstrap;
using MobileFramework.Core.StateMachine;
using MobileFramework.Core.StateMachine.States;

namespace MobileFramework.Core.Managers.UI.Panels
{
    public class PausePanel : UIPanel
    {
        protected override string DefaultPanelId => CorePanelIds.Pause;

        public void OnResumeClicked()
        {
            if (ServiceLocator.Instance.TryGet<GameManager>(out var gameManager)
                && gameManager.CurrentState is PausedState paused)
            {
                paused.Resume();
            }
        }

        public void OnHomeClicked()
        {
            if (ServiceLocator.Instance.TryGet<GameManager>(out var gameManager)
                && gameManager.CurrentState is PausedState paused)
            {
                paused.QuitToMenu();
            }
        }
    }
}
