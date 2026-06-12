using MobileFramework.Core.Bootstrap;

namespace MobileFramework.Core.Contracts
{
    /// <summary>
    /// Contratto principale del gameplay. Ogni gioco implementa questa interfaccia
    /// e la registra nel CoreBootstrapper. Il ciclo di vita è guidato dagli stati:
    /// LoadingState → Initialize, PlayingState → StartGame/ResumeGame/Tick,
    /// PausedState → PauseGame, GameOverState → EndGame, UnloadingState → Cleanup.
    /// </summary>
    public interface IMiniGame
    {
        /// <summary>Identificatore univoco del titolo (es. "flappy_clone").</summary>
        string GameId { get; }

        /// <summary>Chiamato una volta per sessione, prima di StartGame. Riceve tutti i servizi del Core.</summary>
        void Initialize(GameContext context);

        /// <summary>Inizio di una nuova partita.</summary>
        void StartGame();

        /// <summary>Il gameplay è sospeso (pausa utente o interruzione OS).</summary>
        void PauseGame();

        /// <summary>Il gameplay riprende dopo una pausa.</summary>
        void ResumeGame();

        /// <summary>Update per frame, chiamato solo in PlayingState.</summary>
        void Tick(float deltaTime);

        /// <summary>La partita è terminata. Salvare punteggi e stato qui.</summary>
        void EndGame(GameOverReason reason);

        /// <summary>Libera asset e stato. Dopo questa chiamata il gioco può essere re-inizializzato.</summary>
        void Cleanup();
    }
}
