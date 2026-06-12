using System.Collections.Generic;
using MobileFramework.Core.Managers.Haptic;

namespace MobileFramework.Tests.EditMode.Fakes
{
    /// <summary>IHapticManager finto: registra i pattern richiesti.</summary>
    public sealed class FakeHapticManager : IHapticManager
    {
        public bool HapticsEnabled { get; set; } = true;

        public readonly List<HapticFeedbackType> Played = new List<HapticFeedbackType>();

        public void Play(HapticFeedbackType type)
        {
            if (HapticsEnabled)
            {
                Played.Add(type);
            }
        }
    }
}
