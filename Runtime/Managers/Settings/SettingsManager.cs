using System;
using System.Collections.Generic;
using MobileFramework.Core.Managers.Save;

namespace MobileFramework.Core.Managers.Settings
{
    /// <summary>
    /// Implementazione di ISettingsManager. Persiste tramite ISaveSystem
    /// (file "core_settings"); ogni modifica salva subito ed emette SettingsChanged.
    /// </summary>
    public sealed class SettingsManager : ISettingsManager
    {
        private sealed class SettingsData : VersionedSaveData
        {
            public override string SaveKey => "core_settings";
            public override int DataVersion => 1;

            public bool musicEnabled = true;
            public bool sfxEnabled = true;
            public bool hapticsEnabled = true;
            public string language = "en";
        }

        private readonly ISaveSystem _saveSystem;
        private SettingsData _data = new SettingsData();

        public event Action SettingsChanged;

        public SettingsManager(ISaveSystem saveSystem)
        {
            _saveSystem = saveSystem ?? throw new ArgumentNullException(nameof(saveSystem));
        }

        public bool MusicEnabled
        {
            get => _data.musicEnabled;
            set => Set(ref _data.musicEnabled, value);
        }

        public bool SfxEnabled
        {
            get => _data.sfxEnabled;
            set => Set(ref _data.sfxEnabled, value);
        }

        public bool HapticsEnabled
        {
            get => _data.hapticsEnabled;
            set => Set(ref _data.hapticsEnabled, value);
        }

        public string Language
        {
            get => _data.language;
            set => Set(ref _data.language, value);
        }

        public void Load() => _data = _saveSystem.Load<SettingsData>();

        public void Save() => _saveSystem.Save(_data);

        private void Set<T>(ref T field, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return;
            }

            field = value;
            Save();
            SettingsChanged?.Invoke();
        }
    }
}
