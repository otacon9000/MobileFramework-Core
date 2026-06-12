using System;
using System.Collections.Generic;

namespace MobileFramework.Core.Events
{
    /// <summary>
    /// Bus eventi tipizzato. Il vincolo <c>where T : struct</c> impone a compile time
    /// la regola del framework: gli eventi sono sempre struct, mai classi.
    /// </summary>
    public interface IEventBus
    {
        void Subscribe<T>(Action<T> handler) where T : struct;
        void Unsubscribe<T>(Action<T> handler) where T : struct;
        void Emit<T>(in T evt) where T : struct;

        /// <summary>Rimuove tutti i listener. Chiamato da UnloadingState a fine sessione.</summary>
        void Clear();

        int CountSubscribers<T>() where T : struct;
    }

    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();

        /// <summary>Hook di osservazione per la tooling di debug (EventBusMonitor). Boxa l'evento: non usare in produzione.</summary>
        public event Action<Type, object> EventEmitted;

        public void Subscribe<T>(Action<T> handler) where T : struct
        {
            if (handler == null)
            {
                return;
            }

            if (!_subscribers.TryGetValue(typeof(T), out var list))
            {
                list = new List<Delegate>();
                _subscribers[typeof(T)] = list;
            }

            list.Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            if (handler != null && _subscribers.TryGetValue(typeof(T), out var list))
            {
                list.Remove(handler);
            }
        }

        public void Emit<T>(in T evt) where T : struct
        {
            EventEmitted?.Invoke(typeof(T), evt);

            if (!_subscribers.TryGetValue(typeof(T), out var list) || list.Count == 0)
            {
                return;
            }

            // Snapshot: i listener aggiunti/rimossi durante l'emissione (incluso Clear)
            // non alterano il giro corrente e non invalidano l'iterazione.
            var snapshot = list.ToArray();
            for (var i = 0; i < snapshot.Length; i++)
            {
                ((Action<T>)snapshot[i]).Invoke(evt);
            }
        }

        public void Clear() => _subscribers.Clear();

        public int CountSubscribers<T>() where T : struct
        {
            return _subscribers.TryGetValue(typeof(T), out var list) ? list.Count : 0;
        }
    }
}
