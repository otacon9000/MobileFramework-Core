using System.Collections;
using UnityEngine;

namespace MobileFramework.Core.Utilities
{
    /// <summary>
    /// Permette alle classi non-MonoBehaviour (SaveSystem, ErrorHandler, stati)
    /// di avviare coroutine. Il CoreBootstrapper ne registra una nel ServiceLocator.
    /// </summary>
    public sealed class CoroutineRunner : MonoBehaviour
    {
        /// <summary>Crea un runner su un GameObject dedicato e persistente.</summary>
        public static CoroutineRunner Create(string name = "[CoroutineRunner]")
        {
            var go = new GameObject(name);
            DontDestroyOnLoad(go);
            return go.AddComponent<CoroutineRunner>();
        }

        public Coroutine Run(IEnumerator routine) => StartCoroutine(routine);

        public void Stop(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        public void StopAll() => StopAllCoroutines();
    }
}
