using System;

namespace MobileFramework.Core.Managers.Settings
{
    /// <summary>
    /// Preferenze utente persistenti: audio, haptics, lingua.
    /// Ogni modifica viene salvata immediatamente ed emette SettingsChanged.
    /// </summary>
    public interface ISettingsManager
    {
        bool MusicEnabled { get; set; }
        bool SfxEnabled { get; set; }
        bool HapticsEnabled { get; set; }

        /// <summary>Codice lingua ISO 639-1 (es. "en", "it").</summary>
        string Language { get; set; }

        event Action SettingsChanged;

        void Load();
        void Save();
    }
}
