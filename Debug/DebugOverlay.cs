using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MobileFramework.Debugging
{
    /// <summary>
    /// Overlay di sviluppo: FPS counter e buffer degli ultimi log condizionali.
    /// L'intero assembly è escluso dalle release dal defineConstraints; i metodi
    /// statici sono comunque marcati [Conditional] così le chiamate spariscono
    /// dai chiamanti nelle build di produzione.
    /// </summary>
    public sealed class DebugOverlay : MonoBehaviour
    {
        private const int MaxLines = 12;

        private static readonly Queue<string> Lines = new Queue<string>(MaxLines);

        [SerializeField] private bool visible = true;

        private float _smoothedDeltaTime;

        /// <summary>Log visibile solo in Editor e development build.</summary>
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("UNITY_EDITOR")]
        public static void Log(string message)
        {
            Debug.Log("[DEV] " + message);

            if (Lines.Count >= MaxLines)
            {
                Lines.Dequeue();
            }

            Lines.Enqueue($"[{Time.realtimeSinceStartup:F1}] {message}");
        }

        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("UNITY_EDITOR")]
        public void Toggle()
        {
            visible = !visible;
        }

        private void Update()
        {
            _smoothedDeltaTime = Mathf.Lerp(_smoothedDeltaTime, Time.unscaledDeltaTime, 0.1f);

            // tre dita: toggle dell'overlay su device
            if (UnityEngine.Input.touchCount == 3
                && UnityEngine.Input.GetTouch(2).phase == TouchPhase.Began)
            {
                Toggle();
            }
        }

        private void OnGUI()
        {
            if (!visible)
            {
                return;
            }

            var fps = _smoothedDeltaTime > 0f ? 1f / _smoothedDeltaTime : 0f;
            GUILayout.BeginArea(new Rect(10, 10, 360, 280), GUI.skin.box);
            GUILayout.Label($"FPS: {fps:F0}  ({_smoothedDeltaTime * 1000f:F1} ms)");

            foreach (var line in Lines)
            {
                GUILayout.Label(line);
            }

            GUILayout.EndArea();
        }
    }
}
