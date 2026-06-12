using System;
using System.Collections.Generic;
using MobileFramework.Core.Bootstrap;
using MobileFramework.Core.Events;
using UnityEngine;

namespace MobileFramework.Debugging
{
    /// <summary>
    /// Ultimi N eventi emessi sul bus. Si aggancia all'hook EventEmitted del
    /// EventBus concreto (registrato dal CoreBootstrapper accanto a IEventBus).
    /// </summary>
    public sealed class EventBusMonitor : MonoBehaviour
    {
        private const int MaxEvents = 20;

        private readonly Queue<string> _events = new Queue<string>(MaxEvents);
        private EventBus _bus;

        [SerializeField] private bool visible = true;

        private void OnEnable()
        {
            if (ServiceLocator.Instance.TryGet<EventBus>(out _bus))
            {
                _bus.EventEmitted += OnEventEmitted;
            }
        }

        private void OnDisable()
        {
            if (_bus != null)
            {
                _bus.EventEmitted -= OnEventEmitted;
                _bus = null;
            }
        }

        private void OnEventEmitted(Type eventType, object payload)
        {
            if (_events.Count >= MaxEvents)
            {
                _events.Dequeue();
            }

            _events.Enqueue($"[{Time.realtimeSinceStartup:F1}] {eventType.Name}: {payload}");
        }

        private void OnGUI()
        {
            if (!visible)
            {
                return;
            }

            GUILayout.BeginArea(new Rect(Screen.width - 460, Screen.height - 330, 450, 320), GUI.skin.box);
            GUILayout.Label($"EVENT BUS — ultimi {_events.Count} eventi");

            foreach (var entry in _events)
            {
                GUILayout.Label(entry);
            }

            GUILayout.EndArea();
        }
    }
}
