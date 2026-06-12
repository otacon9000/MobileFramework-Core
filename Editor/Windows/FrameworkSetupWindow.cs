using MobileFramework.Core.Bootstrap;
using UnityEditor;
using UnityEngine;

namespace MobileFramework.CoreEditor.Windows
{
    /// <summary>
    /// Setup guidato del framework in un nuovo progetto. Versione minima:
    /// da estendere quando arriveranno il secondo e terzo gioco (vedi SPEC).
    /// </summary>
    public sealed class FrameworkSetupWindow : EditorWindow
    {
        [MenuItem("MobileFramework/Setup")]
        public static void Open()
        {
            GetWindow<FrameworkSetupWindow>("Framework Setup");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Mobile Game Framework — Setup", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "Checklist nuovo gioco:\n" +
                "1. Crea la cartella Assets/_Game/ con l'asmdef del gioco (referenzia MobileFramework.Core)\n" +
                "2. Crea la scena Core con il CoreBootstrapper (scena 0 in Build Settings)\n" +
                "3. Implementa IMiniGame e IGameSaveData\n" +
                "4. Registra il minigame con CoreBootstrapper.RegisterMiniGame\n" +
                "5. Play: verifica Boot → MainMenu → Loading → Playing",
                MessageType.Info);

            EditorGUILayout.Space();

            if (GUILayout.Button("Crea CoreBootstrapper nella scena attiva"))
            {
                CreateBootstrapper();
            }
        }

        private static void CreateBootstrapper()
        {
            if (FindFirstObjectByType<CoreBootstrapper>() != null)
            {
                EditorUtility.DisplayDialog("Framework Setup", "La scena contiene già un CoreBootstrapper.", "OK");
                return;
            }

            var go = new GameObject("[Core]");
            go.AddComponent<CoreBootstrapper>();
            Undo.RegisterCreatedObjectUndo(go, "Crea CoreBootstrapper");
            Selection.activeGameObject = go;
        }
    }
}
