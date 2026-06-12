using MobileFramework.Core.Managers.UI;

namespace MobileFramework.Core.StateMachine.States
{
    /// <summary>
    /// Fine partita. Vi si arriva quando il gioco emette un GameOverEvent:
    /// il GameManager memorizza il motivo e transiziona qui.
    /// </summary>
    public class GameOverState : GameState
    {
        public override void Enter()
        {
            Context.Input.InputEnabled = false;
            Manager.IsSessionActive = false;
            Manager.IsMiniGamePaused = false;
            Manager.MiniGame.EndGame(Manager.LastGameOverReason);
            Context.UI.Push(CorePanelIds.GameOver);
        }

        /// <summary>Nuova partita immediata (pulsante Riprova).</summary>
        public void Retry()
        {
            Manager.ChangeState<LoadingState>();
        }

        /// <summary>Torna al menu liberando la sessione.</summary>
        public void GoHome()
        {
            Manager.ChangeState<UnloadingState>();
        }

        /// <summary>Il giocatore ha scelto di guardare un rewarded ad (es. continue).</summary>
        public void OfferRewarded()
        {
            Manager.ChangeState<RewardedState>();
        }
    }
}
