namespace MobileFramework.Core.StateMachine.States
{
    /// <summary>
    /// Interruzione di sistema: chiamata telefonica, notifica, focus perso.
    /// Vi si entra solo tramite GameManager.NotifyOSInterrupt; al rientro
    /// (NotifyOSResume) il gameplay riprende in pausa, mai direttamente in gioco.
    /// </summary>
    public sealed class OSInterruptState : GameState
    {
        public override void Enter()
        {
            Context.Input.InputEnabled = false;

            if (Manager.IsSessionActive && !Manager.IsMiniGamePaused)
            {
                Manager.IsMiniGamePaused = true;
                Manager.MiniGame.PauseGame();
            }
        }
    }
}
