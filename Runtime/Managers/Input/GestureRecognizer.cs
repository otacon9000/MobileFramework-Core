using System;
using UnityEngine;

namespace MobileFramework.Core.Managers.Input
{
    /// <summary>
    /// Riconoscimento gesti da eventi pointer grezzi. Classe pura: l'InputManager
    /// la alimenta da Update, i test la pilotano direttamente.
    /// </summary>
    public sealed class GestureRecognizer
    {
        /// <summary>Durata massima (secondi) e movimento massimo (pixel) perché un tocco sia un tap.</summary>
        public float TapMaxDuration = 0.25f;
        public float TapMaxMovement = 20f;

        /// <summary>Distanza minima (pixel) perché un rilascio sia uno swipe.</summary>
        public float SwipeMinDistance = 80f;

        public event Action<Vector2> TapDetected;
        public event Action<SwipeDirection, Vector2> SwipeDetected;
        public event Action<float> PinchDetected;

        private Vector2 _startPosition;
        private float _startTime;
        private bool _tracking;
        private float _lastPinchDistance = -1f;

        public void PointerDown(Vector2 position, float time)
        {
            _startPosition = position;
            _startTime = time;
            _tracking = true;
        }

        public void PointerUp(Vector2 position, float time)
        {
            if (!_tracking)
            {
                return;
            }

            _tracking = false;
            var delta = position - _startPosition;

            if (time - _startTime <= TapMaxDuration && delta.magnitude <= TapMaxMovement)
            {
                TapDetected?.Invoke(_startPosition);
            }
            else if (delta.magnitude >= SwipeMinDistance)
            {
                SwipeDetected?.Invoke(DirectionFromDelta(delta), _startPosition);
            }
        }

        public void PointerCancel() => _tracking = false;

        /// <summary>Da chiamare ogni frame con due dita attive, passando la distanza corrente tra le dita.</summary>
        public void Pinch(float currentDistance)
        {
            _tracking = false; // due dita: nessun tap/swipe in corso

            if (_lastPinchDistance >= 0f)
            {
                var delta = currentDistance - _lastPinchDistance;
                if (!Mathf.Approximately(delta, 0f))
                {
                    PinchDetected?.Invoke(delta);
                }
            }

            _lastPinchDistance = currentDistance;
        }

        public void PinchEnded() => _lastPinchDistance = -1f;

        public static SwipeDirection DirectionFromDelta(Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                return delta.x > 0f ? SwipeDirection.Right : SwipeDirection.Left;
            }

            return delta.y > 0f ? SwipeDirection.Up : SwipeDirection.Down;
        }
    }
}
