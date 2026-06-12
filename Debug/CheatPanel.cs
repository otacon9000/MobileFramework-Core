using System.Diagnostics;
using MobileFramework.Core.Bootstrap;
using MobileFramework.Core.Contracts;
using MobileFramework.Core.Events;
using MobileFramework.Core.Managers.Save;
using MobileFramework.Core.StateMachine;
using MobileFramework.Core.StateMachine.States;
using UnityEngine;

namespace MobileFramework.Debugging
{
    /// <summary>Pannello comandi di sviluppo: forza game over, pausa, reset salvataggi, time scale.</summary>
    public sealed class CheatPanel : MonoBehaviour
    {
        [SerializeField] private bool visible;

        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("UNITY_EDITOR")]
        public void Toggle()
        {
            visible = !visible;
        }

        private void OnGUI()
        {
            if (!visible)
            {
                if (GUI.Button(new Rect(Screen.width - 90, 10, 80, 30), "CHEATS"))
                {
                    visible = true;
                }

                return;
            }

            var locator = ServiceLocator.Instance;
            GUILayout.BeginArea(new Rect(Screen.width - 230, 10, 220, 340), GUI.skin.box);
            GUILayout.Label("CHEAT PANEL");

            if (locator.TryGet<IEventBus>(out var events))
            {
                if (GUILayout.Button("Force Win"))
                {
                    events.Emit(new GameOverEvent(GameOverReason.Win));
                }

                if (GUILayout.Button("Force Lose"))
                {
                    events.Emit(new GameOverEvent(GameOverReason.Lose));
                }
            }

            if (locator.TryGet<GameManager>(out var gameManager))
            {
                if (GUILayout.Button("Pause") && gameManager.CurrentState is PlayingState playing)
                {
                    playing.Pause();
                }

                if (GUILayout.Button("Quit to Menu"))
                {
                    gameManager.ChangeState<UnloadingState>();
                }
            }

            if (locator.TryGet<ISaveSystem>(out var saveSystem) && GUILayout.Button("Delete All Saves"))
            {
                saveSystem.DeleteAll();
                DebugOverlay.Log("Salvataggi cancellati");
            }

            GUILayout.Label($"Time scale: {Time.timeScale:F2}");
            if (GUILayout.Button("Time x0.25"))
            {
                Time.timeScale = 0.25f;
            }

            if (GUILayout.Button("Time x1"))
            {
                Time.timeScale = 1f;
            }

            if (GUILayout.Button("Chiudi"))
            {
                visible = false;
            }

            GUILayout.EndArea();
        }
    }
}
