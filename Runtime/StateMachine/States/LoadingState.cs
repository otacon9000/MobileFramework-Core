using MobileFramework.Core.Events;
using UnityEngine;

namespace MobileFramework.Core.StateMachine.States
{
    /// <summary>
    /// Prepara la sessione: inizializza l'IMiniGame con il GameContext ed emette
    /// il progresso di caricamento. Il caricamento scene/Addressables specifico
    /// è responsabilità del gioco, dentro IMiniGame.Initialize.
    /// </summary>
    public class LoadingState : GameState
    {
        public override void Enter()
        {
            if (Manager.MiniGame == null)
            {
                Debug.LogWarning("[LoadingState] Nessun IMiniGame registrato: torno al menu.");
                Manager.ChangeState<MainMenuState>();
                return;
            }

            Context.Events.Emit(new SceneLoadProgressEvent(Manager.MiniGame.GameId, 0f));
            Manager.MiniGame.Initialize(Context);
            Context.Events.Emit(new SceneLoadProgressEvent(Manager.MiniGame.GameId, 1f));

            Manager.ChangeState<PlayingState>();
        }
    }
}
