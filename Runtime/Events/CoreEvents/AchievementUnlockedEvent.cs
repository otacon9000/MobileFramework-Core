namespace MobileFramework.Core.Events
{
    /// <summary>Emesso quando il giocatore sblocca un achievement.</summary>
    public readonly struct AchievementUnlockedEvent
    {
        public readonly string AchievementId;

        public AchievementUnlockedEvent(string achievementId)
        {
            AchievementId = achievementId;
        }
    }
}
