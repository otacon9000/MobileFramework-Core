namespace MobileFramework.Core.Managers.Haptic
{
    /// <summary>Pattern haptic unificati tra iOS e Android.</summary>
    public enum HapticFeedbackType
    {
        LightImpact,
        MediumImpact,
        HeavyImpact,
        Success,
        Warning,
        Failure
    }

    public interface IHapticManager
    {
        bool HapticsEnabled { get; set; }

        void Play(HapticFeedbackType type);
    }
}
