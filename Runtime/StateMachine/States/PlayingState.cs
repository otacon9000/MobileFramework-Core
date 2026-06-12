using MobileFramework.Core.Managers.UI;

namespace MobileFramework.Core.StateMachine.States
{
    /// <summary>
    /// Gameplay attivo. Alla prima entrata della sessione chiama StartGame e mostra
    /// l'HUD; ai rientri (da pausa, interruzione OS o rewarded) chiama ResumeGame.
    /// </summary>
    public class PlayingState : GameState
    {
        public override void Enter()
        {
            Context.Input.InputEnabled = true;

            if (!Manager.IsSessionActive)
            {
                Manager.IsSessionActive = true;
                Context.UI.Push(CorePanelIds.HUD);
                Manager.MiniGame.StartGame();
            }
            else
            {
                Manager.IsMiniGamePaused = false;
                Manager.MiniGame.ResumeGame();
            }
        }

        public override void Tick(float deltaTime)
        {
            Manager.MiniGame?.Tick(deltaTime);
        }

        /// <summary>Pausa richiesta dal giocatore.</summary>
        public void Pause()
        {
            Manager.ChangeState<PausedState>();
        }
    }
}
