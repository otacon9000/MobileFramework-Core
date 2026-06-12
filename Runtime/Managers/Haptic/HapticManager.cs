using UnityEngine;

namespace MobileFramework.Core.Managers.Haptic
{
    /// <summary>
    /// Implementazione di IHapticManager. Senza plugin nativi Unity espone solo
    /// Handheld.Vibrate(): i pattern leggeri vengono saltati per non risultare
    /// invadenti, quelli medi/forti vibrano. Sostituire con un plugin nativo
    /// (UIImpactFeedbackGenerator / VibrationEffect) quando serve fedeltà reale.
    /// </summary>
    public sealed class HapticManager : IHapticManager
    {
        public bool HapticsEnabled { get; set; } = true;

        public void Play(HapticFeedbackType type)
        {
            if (!HapticsEnabled)
            {
                return;
            }

#if UNITY_IOS || UNITY_ANDROID
            switch (type)
            {
                case HapticFeedbackType.LightImpact:
                    // troppo debole per Handheld.Vibrate: no-op finché non c'è il plugin nativo
                    break;
                case HapticFeedbackType.MediumImpact:
                case HapticFeedbackType.HeavyImpact:
                case HapticFeedbackType.Success:
                case HapticFeedbackType.Warning:
                case HapticFeedbackType.Failure:
                    Handheld.Vibrate();
                    break;
            }
#endif
        }
    }
}
