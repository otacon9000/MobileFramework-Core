using MobileFramework.Core.Bootstrap;
using MobileFramework.Core.StateMachine;
using MobileFramework.Core.StateMachine.States;

namespace MobileFramework.Core.Managers.UI.Panels
{
    /// <summary>
    /// HUD di base mostrato durante il gameplay. I giochi tipicamente lo
    /// sostituiscono con un pannello specifico (es. FlappyHUDPanel) che mostra
    /// punteggio e stato leggendo gli eventi del proprio gioco.
    /// </summary>
    public class HUDPanel : UIPanel
    {
        protected override string DefaultPanelId => CorePanelIds.HUD;

        public void OnPauseClicked()
        {
            if (ServiceLocator.Instance.TryGet<GameManager>(out var gameManager)
                && gameManager.CurrentState is PlayingState playing)
            {
                playing.Pause();
            }
        }
    }
}
