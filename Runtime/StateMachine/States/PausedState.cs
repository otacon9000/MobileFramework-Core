using MobileFramework.Core.Managers.UI;

namespace MobileFramework.Core.StateMachine.States
{
    public class PausedState : GameState
    {
        public override void Enter()
        {
            Context.Input.InputEnabled = false;

            if (!Manager.IsMiniGamePaused)
            {
                Manager.IsMiniGamePaused = true;
                Manager.MiniGame.PauseGame();
            }

            Context.UI.Push(CorePanelIds.Pause);
        }

        public override void Exit()
        {
            if (Context.UI.CurrentPanelId == CorePanelIds.Pause)
            {
                Context.UI.Pop();
            }
        }

        /// <summary>Riprende il gameplay (pulsante Resume).</summary>
        public void Resume()
        {
            Manager.ChangeState<PlayingState>();
        }

        /// <summary>Abbandona la partita e torna al menu (pulsante Home).</summary>
        public void QuitToMenu()
        {
            Manager.ChangeState<UnloadingState>();
        }
    }
}
