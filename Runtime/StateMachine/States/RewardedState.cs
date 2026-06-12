namespace MobileFramework.Core.StateMachine.States
{
    /// <summary>
    /// Stato post rewarded ad. Il gioco (o l'SDK ads) chiama CompleteReward
    /// quando il video termina: con successo si riprende la partita (continue),
    /// altrimenti si torna al game over.
    /// </summary>
    public class RewardedState : GameState
    {
        public void CompleteReward(bool resumeGameplay)
        {
            if (resumeGameplay)
            {
                // Continue: la sessione riparte da dove si era interrotta.
                Manager.IsSessionActive = true;
                Manager.ChangeState<PlayingState>();
            }
            else
            {
                Manager.ChangeState<GameOverState>();
            }
        }
    }
}
