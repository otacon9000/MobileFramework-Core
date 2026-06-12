using System.Collections.Generic;
using UnityEngine;

namespace MobileFramework.Core.Managers.Audio
{
    /// <summary>
    /// Pool di AudioSource sullo stesso GameObject: zero allocazioni a regime.
    /// Una source è "libera" quando non sta riproducendo; se sono tutte occupate
    /// il pool cresce di una unità.
    /// </summary>
    public sealed class AudioPool
    {
        private readonly GameObject _host;
        private readonly List<AudioSource> _sources = new List<AudioSource>();

        public int Count => _sources.Count;

        public AudioPool(GameObject host, int initialSize = 8)
        {
            _host = host;
            for (var i = 0; i < initialSize; i++)
            {
                CreateSource();
            }
        }

        public AudioSource GetFree()
        {
            for (var i = 0; i < _sources.Count; i++)
            {
                if (!_sources[i].isPlaying)
                {
                    return _sources[i];
                }
            }

            return CreateSource();
        }

        public void StopAll()
        {
            for (var i = 0; i < _sources.Count; i++)
            {
                _sources[i].Stop();
            }
        }

        private AudioSource CreateSource()
        {
            var source = _host.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            _sources.Add(source);
            return source;
        }
    }
}
