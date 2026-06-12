namespace MobileFramework.Core.Contracts
{
    /// <summary>Motivo della fine partita, passato a <see cref="IMiniGame.EndGame"/>.</summary>
    public enum GameOverReason
    {
        Win,
        Lose,
        Timeout,
        PlayerQuit
    }
}
