using MobileFramework.Core.Contracts;

namespace MobileFramework.Core.Events
{
    /// <summary>
    /// Emesso dal gioco quando la partita finisce. Il GameManager lo intercetta
    /// e transiziona a GameOverState chiamando IMiniGame.EndGame(Reason).
    /// </summary>
    public readonly struct GameOverEvent
    {
        public readonly GameOverReason Reason;
        public readonly int Score;

        public GameOverEvent(GameOverReason reason, int score = 0)
        {
            Reason = reason;
            Score = score;
        }
    }
}
