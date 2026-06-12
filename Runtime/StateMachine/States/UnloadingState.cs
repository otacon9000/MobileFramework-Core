namespace MobileFramework.Core.StateMachine.States
{
    /// <summary>
    /// Chiude la sessione: Cleanup del minigame, stack UI svuotato, EventBus
    /// azzerato. Dopo Clear il GameManager si riregistra subito agli eventi Core;
    /// ogni altro listener di lunga durata deve risottoscriversi (di norma lo fa
    /// rientrando in MainMenuState).
    /// </summary>
    public sealed class UnloadingState : GameState
    {
        public override void Enter()
        {
            Manager.MiniGame?.Cleanup();
            Manager.IsSessionActive = false;
            Manager.IsMiniGamePaused = false;

            Context.UI.PopToRoot();

            Context.Events.Clear();
            Manager.SubscribeCoreEvents();

            Manager.ChangeState<MainMenuState>();
        }
    }
}
