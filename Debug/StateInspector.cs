using MobileFramework.Core.Bootstrap;
using MobileFramework.Core.Managers.UI;
using MobileFramework.Core.StateMachine;
using UnityEngine;

namespace MobileFramework.Debugging
{
    /// <summary>Stato del GameManager in tempo reale: stato corrente, sessione, stack UI.</summary>
    public sealed class StateInspector : MonoBehaviour
    {
        private void OnGUI()
        {
            if (!ServiceLocator.Instance.TryGet<GameManager>(out var gameManager))
            {
                return;
            }

            GUILayout.BeginArea(new Rect(10, Screen.height - 130, 380, 120), GUI.skin.box);
            GUILayout.Label($"Stato: {gameManager.CurrentState?.GetType().Name ?? "nessuno"}");
            GUILayout.Label($"Sessione attiva: {gameManager.IsSessionActive}   In pausa: {gameManager.IsMiniGamePaused}");
            GUILayout.Label($"MiniGame: {gameManager.MiniGame?.GameId ?? "non registrato"}");

            if (ServiceLocator.Instance.TryGet<IUIManager>(out var ui))
            {
                GUILayout.Label($"UI stack ({ui.StackDepth}): top = {ui.CurrentPanelId ?? "-"}");
            }

            GUILayout.EndArea();
        }
    }
}
