using MobileFramework.Core.Bootstrap;
using MobileFramework.Core.Contracts;

namespace MobileFramework.Tests.EditMode.Fakes
{
    /// <summary>IMiniGame finto: registra ogni chiamata del ciclo di vita.</summary>
    public sealed class FakeMiniGame : IMiniGame
    {
        public string GameId => "fake_game";

        public GameContext LastContext;
        public GameOverReason? LastEndReason;

        public int InitializeCalls;
        public int StartCalls;
        public int PauseCalls;
        public int ResumeCalls;
        public int TickCalls;
        public int EndCalls;
        public int CleanupCalls;
        public float TotalTickTime;

        public void Initialize(GameContext context)
        {
            InitializeCalls++;
            LastContext = context;
        }

        public void StartGame() => StartCalls++;

        public void PauseGame() => PauseCalls++;

        public void ResumeGame() => ResumeCalls++;

        public void Tick(float deltaTime)
        {
            TickCalls++;
            TotalTickTime += deltaTime;
        }

        public void EndGame(GameOverReason reason)
        {
            EndCalls++;
            LastEndReason = reason;
        }

        public void Cleanup() => CleanupCalls++;
    }
}
