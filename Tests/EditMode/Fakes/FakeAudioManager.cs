using System.Collections.Generic;
using MobileFramework.Core.Managers.Audio;
using UnityEngine;

namespace MobileFramework.Tests.EditMode.Fakes
{
    /// <summary>IAudioManager finto: registra le clip riprodotte senza toccare l'engine audio.</summary>
    public sealed class FakeAudioManager : IAudioManager
    {
        public bool MusicEnabled { get; set; } = true;
        public bool SfxEnabled { get; set; } = true;
        public float MusicVolume { get; set; } = 1f;
        public float SfxVolume { get; set; } = 1f;

        public AudioClip CurrentMusic;
        public bool MusicIsPlaying;
        public readonly List<AudioClip> SfxPlayed = new List<AudioClip>();
        public int StopMusicCalls;
        public int StopAllSfxCalls;

        public void PlayMusic(AudioClip clip, bool loop = true, float fadeSeconds = 0f)
        {
            CurrentMusic = clip;
            MusicIsPlaying = clip != null;
        }

        public void StopMusic(float fadeSeconds = 0f)
        {
            StopMusicCalls++;
            MusicIsPlaying = false;
        }

        public void PlaySfx(AudioClip clip, float volumeScale = 1f, float pitch = 1f)
        {
            if (SfxEnabled)
            {
                SfxPlayed.Add(clip);
            }
        }

        public void StopAllSfx() => StopAllSfxCalls++;
    }
}
