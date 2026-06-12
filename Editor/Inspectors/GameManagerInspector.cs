using MobileFramework.Core.Bootstrap;
using MobileFramework.Core.StateMachine.States;
using UnityEditor;
using UnityEngine;

namespace MobileFramework.CoreEditor.Inspectors
{
    /// <summary>
    /// Inspector runtime del GameManager. Il GameManager è una classe pura,
    /// quindi il CustomEditor vive sul CoreBootstrapper che lo possiede:
    /// in Play mode mostra stato corrente, sessione e scorciatoie di transizione.
    /// </summary>
    [CustomEditor(typeof(CoreBootstrapper))]
    public sealed class GameManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var bootstrapper = (CoreBootstrapper)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("GameManager", EditorStyles.boldLabel);

            if (!Application.isPlaying || bootstrapper.GameManager == null)
            {
                EditorGUILayout.HelpBox("Le informazioni runtime compaiono in Play mode.", MessageType.Info);
                return;
            }

            var gameManager = bootstrapper.GameManager;

            EditorGUILayout.LabelField("Stato corrente", gameManager.CurrentState?.GetType().Name ?? "nessuno");
            EditorGUILayout.LabelField("Sessione attiva", gameManager.IsSessionActive.ToString());
            EditorGUILayout.LabelField("MiniGame", gameManager.MiniGame?.GameId ?? "non registrato");
            EditorGUILayout.LabelField("Ultimo game over", gameManager.LastGameOverReason.ToString());

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Pausa") && gameManager.CurrentState is PlayingState playing)
                {
                    playing.Pause();
                }

                if (GUILayout.Button("Torna al menu"))
                {
                    gameManager.ChangeState<UnloadingState>();
                }
            }

            Repaint();
        }
    }
}
