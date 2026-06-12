using MobileFramework.Core.Bootstrap;
using MobileFramework.Core.StateMachine;
using MobileFramework.Core.StateMachine.States;

namespace MobileFramework.Core.Managers.UI.Panels
{
    public class GameOverPanel : UIPanel
    {
        protected override string DefaultPanelId => CorePanelIds.GameOver;

        public void OnRetryClicked()
        {
            if (ServiceLocator.Instance.TryGet<GameManager>(out var gameManager)
                && gameManager.CurrentState is GameOverState gameOver)
            {
                gameOver.Retry();
            }
        }

        public void OnHomeClicked()
        {
            if (ServiceLocator.Instance.TryGet<GameManager>(out var gameManager)
                && gameManager.CurrentState is GameOverState gameOver)
            {
                gameOver.GoHome();
            }
        }
    }
}
