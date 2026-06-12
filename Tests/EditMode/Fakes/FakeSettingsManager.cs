using System;
using MobileFramework.Core.Managers.Settings;

namespace MobileFramework.Tests.EditMode.Fakes
{
    /// <summary>ISettingsManager finto: stato in memoria, nessuna persistenza.</summary>
    public sealed class FakeSettingsManager : ISettingsManager
    {
        private bool _musicEnabled = true;
        private bool _sfxEnabled = true;
        private bool _hapticsEnabled = true;
        private string _language = "en";

        public int LoadCalls;
        public int SaveCalls;
        public int ChangedCount;

        public event Action SettingsChanged;

        public bool MusicEnabled
        {
            get => _musicEnabled;
            set => Set(ref _musicEnabled, value);
        }

        public bool SfxEnabled
        {
            get => _sfxEnabled;
            set => Set(ref _sfxEnabled, value);
        }

        public bool HapticsEnabled
        {
            get => _hapticsEnabled;
            set => Set(ref _hapticsEnabled, value);
        }

        public string Language
        {
            get => _language;
            set => Set(ref _language, value);
        }

        public void Load() => LoadCalls++;

        public void Save() => SaveCalls++;

        private void Set<T>(ref T field, T value)
        {
            if (Equals(field, value))
            {
                return;
            }

            field = value;
            ChangedCount++;
            SettingsChanged?.Invoke();
        }
    }
}
