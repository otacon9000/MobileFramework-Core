namespace MobileFramework.Core.StateMachine.States
{
    /// <summary>
    /// Primo stato dopo il bootstrap. I servizi sono già registrati dal
    /// CoreBootstrapper, quindi passa subito al menu principale.
    /// </summary>
    public sealed class BootState : GameState
    {
        public override void Enter()
        {
            Manager.ChangeState<MainMenuState>();
        }
    }
}
