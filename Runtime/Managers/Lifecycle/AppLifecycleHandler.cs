using MobileFramework.Core.StateMachine;
using UnityEngine;

namespace MobileFramework.Core.Managers.Lifecycle
{
    /// <summary>
    /// Traduce gli eventi di sistema (pausa app, perdita focus, quit) in
    /// transizioni della state machine: gameplay → OSInterruptState e ritorno.
    /// I metodi Handle* sono pubblici perché i test PlayMode li chiamano
    /// direttamente, non potendo simulare i message di Unity.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AppLifecycleHandler : MonoBehaviour
    {
        private GameManager _gameManager;

        public void Initialize(GameManager gameManager) => _gameManager = gameManager;

        private void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                HandleInterrupt();
            }
            else
            {
                HandleResume();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                HandleInterrupt();
            }
            else
            {
                HandleResume();
            }
        }

        private void OnApplicationQuit() => HandleQuit();

        public void HandleInterrupt() => _gameManager?.NotifyOSInterrupt();

        public void HandleResume() => _gameManager?.NotifyOSResume();

        public void HandleQuit()
        {
            // I manager del Core salvano a ogni modifica, quindi non serve un
            // salvataggio d'emergenza. Hook lasciato per i giochi che ne avessero bisogno.
        }
    }
}
