using System.Collections;
using UnityEngine;

namespace MobileFramework.Core.Managers.Audio
{
    /// <summary>
    /// Implementazione di IAudioManager: una source dedicata alla musica
    /// (con fade in coroutine) e un AudioPool per gli effetti.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AudioManager : MonoBehaviour, IAudioManager
    {
        [SerializeField] private int sfxPoolSize = 8;

        private AudioSource _musicSource;
        private AudioPool _sfxPool;
        private Coroutine _fadeRoutine;
        private AudioClip _pendingMusic;
        private bool _pendingLoop = true;

        private bool _musicEnabled = true;
        private bool _sfxEnabled = true;
        private float _musicVolume = 1f;
        private float _sfxVolume = 1f;

        private void Awake()
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;
            _sfxPool = new AudioPool(gameObject, sfxPoolSize);
        }

        public bool MusicEnabled
        {
            get => _musicEnabled;
            set
            {
                if (_musicEnabled == value)
                {
                    return;
                }

                _musicEnabled = value;
                if (!value)
                {
                    _musicSource.Pause();
                }
                else if (_musicSource.clip != null)
                {
                    _musicSource.UnPause();
                    if (!_musicSource.isPlaying)
                    {
                        _musicSource.Play();
                    }
                }
                else if (_pendingMusic != null)
                {
                    PlayMusic(_pendingMusic, _pendingLoop);
                }
            }
        }

        public bool SfxEnabled
        {
            get => _sfxEnabled;
            set => _sfxEnabled = value;
        }

        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = Mathf.Clamp01(value);
                _musicSource.volume = _musicVolume;
            }
        }

        public float SfxVolume
        {
            get => _sfxVolume;
            set => _sfxVolume = Mathf.Clamp01(value);
        }

        public void PlayMusic(AudioClip clip, bool loop = true, float fadeSeconds = 0f)
        {
            if (clip == null)
            {
                return;
            }

            _pendingMusic = clip;
            _pendingLoop = loop;

            if (!_musicEnabled)
            {
                return; // verrà ripresa quando MusicEnabled torna true
            }

            StopFade();
            _musicSource.clip = clip;
            _musicSource.loop = loop;

            if (fadeSeconds > 0f)
            {
                _musicSource.volume = 0f;
                _musicSource.Play();
                _fadeRoutine = StartCoroutine(FadeTo(_musicVolume, fadeSeconds, stopAtEnd: false));
            }
            else
            {
                _musicSource.volume = _musicVolume;
                _musicSource.Play();
            }
        }

        public void StopMusic(float fadeSeconds = 0f)
        {
            _pendingMusic = null;
            StopFade();

            if (fadeSeconds > 0f && _musicSource.isPlaying)
            {
                _fadeRoutine = StartCoroutine(FadeTo(0f, fadeSeconds, stopAtEnd: true));
            }
            else
            {
                _musicSource.Stop();
            }
        }

        public void PlaySfx(AudioClip clip, float volumeScale = 1f, float pitch = 1f)
        {
            if (clip == null || !_sfxEnabled)
            {
                return;
            }

            var source = _sfxPool.GetFree();
            source.pitch = pitch;
            source.PlayOneShot(clip, _sfxVolume * Mathf.Clamp01(volumeScale));
        }

        public void StopAllSfx() => _sfxPool.StopAll();

        private void StopFade()
        {
            if (_fadeRoutine != null)
            {
                StopCoroutine(_fadeRoutine);
                _fadeRoutine = null;
            }
        }

        private IEnumerator FadeTo(float targetVolume, float seconds, bool stopAtEnd)
        {
            var start = _musicSource.volume;
            var elapsed = 0f;

            while (elapsed < seconds)
            {
                elapsed += Time.unscaledDeltaTime;
                _musicSource.volume = Mathf.Lerp(start, targetVolume, elapsed / seconds);
                yield return null;
            }

            _musicSource.volume = targetVolume;
            if (stopAtEnd)
            {
                _musicSource.Stop();
                _musicSource.volume = _musicVolume;
            }

            _fadeRoutine = null;
        }
    }
}
