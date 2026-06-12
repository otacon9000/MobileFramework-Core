using System.Collections.Generic;
using UnityEngine;

namespace MobileFramework.Core.Localization
{
    public enum TextDirection
    {
        LeftToRight,
        RightToLeft
    }

    /// <summary>
    /// Direzione di layout per le lingue right-to-left (arabo, ebraico, …).
    /// Il reshaping dei glifi arabi è demandato al text renderer (TMP + plugin);
    /// qui si decide solo la direzione del layout.
    /// </summary>
    public static class RTLSupport
    {
        private static readonly HashSet<string> RtlLanguages = new HashSet<string>
        {
            "ar", // arabo
            "he", // ebraico
            "fa", // persiano
            "ur"  // urdu
        };

        /// <summary>Accetta sia "ar" che "ar-SA".</summary>
        public static bool IsRTL(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
            {
                return false;
            }

            var separator = languageCode.IndexOf('-');
            var baseCode = separator > 0 ? languageCode.Substring(0, separator) : languageCode;
            return RtlLanguages.Contains(baseCode.ToLowerInvariant());
        }

        public static TextDirection GetDirection(string languageCode)
        {
            return IsRTL(languageCode) ? TextDirection.RightToLeft : TextDirection.LeftToRight;
        }

        /// <summary>Specchia ancore e pivot di un elemento UI sull'asse orizzontale.</summary>
        public static void MirrorHorizontally(RectTransform rectTransform)
        {
            if (rectTransform == null)
            {
                return;
            }

            var min = rectTransform.anchorMin;
            var max = rectTransform.anchorMax;
            rectTransform.anchorMin = new Vector2(1f - max.x, min.y);
            rectTransform.anchorMax = new Vector2(1f - min.x, max.y);

            var pivot = rectTransform.pivot;
            rectTransform.pivot = new Vector2(1f - pivot.x, pivot.y);

            var pos = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition = new Vector2(-pos.x, pos.y);
        }
    }
}
