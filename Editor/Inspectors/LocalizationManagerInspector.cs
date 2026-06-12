using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using JsonSerializer = MobileFramework.Core.Serialization.JsonSerializer;

namespace MobileFramework.CoreEditor.Inspectors
{
    /// <summary>
    /// Ispettore delle localizzazioni: carica i JSON da Resources/Localization
    /// e segnala le chiavi mancanti rispetto all'inglese (lingua base obbligatoria).
    /// Il LocalizationManager è una classe pura, quindi l'ispezione avviene
    /// in una EditorWindow invece che in un CustomEditor.
    /// </summary>
    public sealed class LocalizationManagerInspector : EditorWindow
    {
        private const string BaseLanguage = "en";

        private readonly Dictionary<string, HashSet<string>> _keysByLanguage = new Dictionary<string, HashSet<string>>();
        private Vector2 _scroll;

        [MenuItem("MobileFramework/Localization Inspector")]
        public static void Open()
        {
            GetWindow<LocalizationManagerInspector>("Localization").Reload();
        }

        private void Reload()
        {
            _keysByLanguage.Clear();

            foreach (var asset in Resources.LoadAll<TextAsset>("Localization"))
            {
                if (!JsonSerializer.TryDeserialize<Dictionary<string, object>>(asset.text, out var entries))
                {
                    Debug.LogWarning($"[LocalizationInspector] JSON non valido: {asset.name}");
                    continue;
                }

                _keysByLanguage[asset.name] = new HashSet<string>(entries.Keys.Where(k => k != "_meta"));
            }
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Ricarica"))
            {
                Reload();
            }

            if (!_keysByLanguage.TryGetValue(BaseLanguage, out var baseKeys))
            {
                EditorGUILayout.HelpBox($"Lingua base '{BaseLanguage}' non trovata in Resources/Localization.", MessageType.Error);
                return;
            }

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            foreach (var pair in _keysByLanguage.OrderBy(p => p.Key))
            {
                EditorGUILayout.LabelField($"{pair.Key} — {pair.Value.Count} chiavi", EditorStyles.boldLabel);

                if (pair.Key == BaseLanguage)
                {
                    continue;
                }

                var missing = baseKeys.Except(pair.Value).ToList();
                var extra = pair.Value.Except(baseKeys).ToList();

                if (missing.Count == 0 && extra.Count == 0)
                {
                    EditorGUILayout.LabelField("   ✓ allineata con " + BaseLanguage);
                    continue;
                }

                foreach (var key in missing)
                {
                    EditorGUILayout.LabelField($"   MANCANTE: {key}");
                }

                foreach (var key in extra)
                {
                    EditorGUILayout.LabelField($"   EXTRA (assente in {BaseLanguage}): {key}");
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
