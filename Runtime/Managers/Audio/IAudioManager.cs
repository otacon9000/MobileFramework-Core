using UnityEngine;

namespace MobileFramework.Core.Managers.Audio
{
    /// <summary>Servizio audio del Core: musica con fade e SFX su pool senza GC.</summary>
    public interface IAudioManager
    {
        bool MusicEnabled { get; set; }
        bool SfxEnabled { get; set; }

        /// <summary>Volume musica, range [0,1].</summary>
        float MusicVolume { get; set; }

        /// <summary>Volume effetti, range [0,1].</summary>
        float SfxVolume { get; set; }

        void PlayMusic(AudioClip clip, bool loop = true, float fadeSeconds = 0f);
        void StopMusic(float fadeSeconds = 0f);
        void PlaySfx(AudioClip clip, float volumeScale = 1f, float pitch = 1f);
        void StopAllSfx();
    }
}
