namespace MobileFramework.Core.Events
{
    /// <summary>Emesso dal LocalizationManager quando cambia la lingua attiva.</summary>
    public readonly struct LanguageChangedEvent
    {
        public readonly string LanguageCode;

        public LanguageChangedEvent(string languageCode)
        {
            LanguageCode = languageCode;
        }
    }
}
