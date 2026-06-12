using System;
using System.Collections.Generic;
using MobileFramework.Core.Utilities;

namespace MobileFramework.Core.Bootstrap
{
    /// <summary>
    /// Registro centrale di tutti i servizi del framework. È l'unico Singleton
    /// dell'intera codebase: tutto il resto si registra qui e si risolve da qui.
    /// </summary>
    public sealed class ServiceLocator : Singleton<ServiceLocator>
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public int Count => _services.Count;

        /// <summary>Registra un servizio per il tipo T (tipicamente l'interfaccia I*).</summary>
        public void Register<T>(T service) where T : class
        {
            _services[typeof(T)] = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>Risolve un servizio. Lancia se non registrato: un servizio mancante è un errore di bootstrap.</summary>
        public T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            throw new InvalidOperationException(
                $"Servizio {typeof(T).Name} non registrato. Verificare l'ordine di init nel CoreBootstrapper.");
        }

        public bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var found))
            {
                service = (T)found;
                return true;
            }

            service = null;
            return false;
        }

        public bool IsRegistered<T>() where T : class => _services.ContainsKey(typeof(T));

        public void Unregister<T>() where T : class => _services.Remove(typeof(T));

        public void Clear() => _services.Clear();
    }
}
