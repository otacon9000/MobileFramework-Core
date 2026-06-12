using System.Collections.Generic;
using MobileFramework.Core.Events;
using UnityEngine;
using JsonSerializer = MobileFramework.Core.Serialization.JsonSerializer;

namespace MobileFramework.Core.Localization
{
    /// <summary>
    /// Chiave → testo con fallback sull'inglese e cambio lingua a runtime.
    /// Le stringhe Core vivono in Resources/Localization/{code}.json;
    /// i giochi aggiungono le proprie con MergeFromJson.
    /// </summary>
    public sealed class LocalizationManager
    {
        public const string FallbackLanguage = "en";
        private const string ResourcePath = "Localization/";
        private const string MetaKey = "_meta";

        private readonly Dictionary<string, string> _strings = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _fallback = new Dictionary<string, string>();
        private readonly IEventBus _events;

        public string CurrentLanguage { get; private set; } = FallbackLanguage;

        public LocalizationManager(IEventBus events = null)
        {
            _events = events;
        }

        /// <summary>Carica la lingua dalle Resources del Core. Emette LanguageChangedEvent.</summary>
        public bool LoadLanguage(string languageCode)
        {
            var asset = Resources.Load<TextAsset>(ResourcePath + languageCode);
            if (asset == null)
            {
                Debug.LogWarning($"[LocalizationManager] Lingua '{languageCode}' non trovata in Resources/{ResourcePath}.");
                return false;
            }

            if (languageCode != FallbackLanguage)
            {
                var fallbackAsset = Resources.Load<TextAsset>(ResourcePath + FallbackLanguage);
                if (fallbackAsset != null)
                {
                    ParseInto(_fallback, fallbackAsset.text, clearFirst: true);
                }
            }

            LoadFromJson(languageCode, asset.text);
            return true;
        }

        /// <summary>Carica la lingua da una stringa JSON (test e pipeline custom).</summary>
        public void LoadFromJson(string languageCode, string json)
        {
            ParseInto(_strings, json, clearFirst: true);
            CurrentLanguage = languageCode;
            _events?.Emit(new LanguageChangedEvent(languageCode));
        }

        /// <summary>Imposta il dizionario di fallback da JSON (test e pipeline custom).</summary>
        public void SetFallbackFromJson(string json)
        {
            ParseInto(_fallback, json, clearFirst: true);
        }

        /// <summary>Overlay del gioco: aggiunge o sovrascrive chiavi senza toccare le altre.</summary>
        public void MergeFromJson(string json)
        {
            ParseInto(_strings, json, clearFirst: false);
        }

        public bool Has(string key) => _strings.ContainsKey(key) || _fallback.ContainsKey(key);

        /// <summary>
        /// Risolve una chiave. Ordine: lingua corrente → fallback inglese → chiave stessa.
        /// Con argomenti applica anche i token plurali {0:singolare|plurale}.
        /// </summary>
        public string Get(string key, params object[] args)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            if (!_strings.TryGetValue(key, out var template) && !_fallback.TryGetValue(key, out template))
            {
                return key;
            }

            return args != null && args.Length > 0 ? PluralResolver.Resolve(template, args) : template;
        }

        private static void ParseInto(Dictionary<string, string> target, string json, bool clearFirst)
        {
            if (!JsonSerializer.TryDeserialize<Dictionary<string, object>>(json, out var raw))
            {
                Debug.LogWarning("[LocalizationManager] JSON di localizzazione non valido, ignorato.");
                return;
            }

            if (clearFirst)
            {
                target.Clear();
            }

            foreach (var pair in raw)
            {
                if (pair.Key == MetaKey)
                {
                    continue;
                }

                target[pair.Key] = pair.Value?.ToString() ?? string.Empty;
            }
        }
    }
}
