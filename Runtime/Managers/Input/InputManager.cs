using System;
using UnityEngine;

namespace MobileFramework.Core.Managers.Input
{
    /// <summary>
    /// Implementazione di IInputManager: legge touch (e mouse in Editor) da Update
    /// e delega il riconoscimento al GestureRecognizer. Gli stati del Core
    /// disattivano InputEnabled fuori dal gameplay.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class InputManager : MonoBehaviour, IInputManager
    {
        public bool InputEnabled { get; set; } = true;

        public event Action<Vector2> Tapped;
        public event Action<SwipeDirection, Vector2> Swiped;
        public event Action<float> Pinched;

        public GestureRecognizer Recognizer { get; } = new GestureRecognizer();

        private void Awake()
        {
            Recognizer.TapDetected += position =>
            {
                if (InputEnabled)
                {
                    Tapped?.Invoke(position);
                }
            };
            Recognizer.SwipeDetected += (direction, position) =>
            {
                if (InputEnabled)
                {
                    Swiped?.Invoke(direction, position);
                }
            };
            Recognizer.PinchDetected += delta =>
            {
                if (InputEnabled)
                {
                    Pinched?.Invoke(delta);
                }
            };
        }

        private void Update()
        {
            if (UnityEngine.Input.touchCount >= 2)
            {
                var a = UnityEngine.Input.GetTouch(0);
                var b = UnityEngine.Input.GetTouch(1);
                Recognizer.Pinch(Vector2.Distance(a.position, b.position));
                return;
            }

            Recognizer.PinchEnded();

            if (UnityEngine.Input.touchCount == 1)
            {
                var touch = UnityEngine.Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        Recognizer.PointerDown(touch.position, Time.unscaledTime);
                        break;
                    case TouchPhase.Ended:
                        Recognizer.PointerUp(touch.position, Time.unscaledTime);
                        break;
                    case TouchPhase.Canceled:
                        Recognizer.PointerCancel();
                        break;
                }

                return;
            }

#if UNITY_EDITOR || UNITY_STANDALONE
            // Fallback mouse per testare in Editor senza device.
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                Recognizer.PointerDown(UnityEngine.Input.mousePosition, Time.unscaledTime);
            }
            else if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                Recognizer.PointerUp(UnityEngine.Input.mousePosition, Time.unscaledTime);
            }
#endif
        }
    }
}
