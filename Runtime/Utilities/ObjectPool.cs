using System;
using System.Collections.Generic;

namespace MobileFramework.Core.Utilities
{
    /// <summary>Pool generico senza GC sul percorso caldo: Get/Release riusano le istanze.</summary>
    public sealed class ObjectPool<T> where T : class
    {
        private readonly Stack<T> _inactive;
        private readonly Func<T> _factory;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;

        public int CountInactive => _inactive.Count;

        public ObjectPool(Func<T> factory, Action<T> onGet = null, Action<T> onRelease = null, int prewarm = 0)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _onGet = onGet;
            _onRelease = onRelease;
            _inactive = new Stack<T>(prewarm);

            for (var i = 0; i < prewarm; i++)
            {
                _inactive.Push(_factory());
            }
        }

        public T Get()
        {
            var item = _inactive.Count > 0 ? _inactive.Pop() : _factory();
            _onGet?.Invoke(item);
            return item;
        }

        public void Release(T item)
        {
            if (item == null)
            {
                return;
            }

            _onRelease?.Invoke(item);
            _inactive.Push(item);
        }

        public void Clear() => _inactive.Clear();
    }
}
