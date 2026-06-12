using System;
using MobileFramework.Core.Managers.Input;
using UnityEngine;

namespace MobileFramework.Tests.EditMode.Fakes
{
    /// <summary>IInputManager finto: i gesti si iniettano con i metodi Simulate*.</summary>
    public sealed class FakeInputManager : IInputManager
    {
        public bool InputEnabled { get; set; } = true;

        public event Action<Vector2> Tapped;
        public event Action<SwipeDirection, Vector2> Swiped;
        public event Action<float> Pinched;

        public void SimulateTap(Vector2 position)
        {
            if (InputEnabled)
            {
                Tapped?.Invoke(position);
            }
        }

        public void SimulateSwipe(SwipeDirection direction, Vector2 startPosition)
        {
            if (InputEnabled)
            {
                Swiped?.Invoke(direction, startPosition);
            }
        }

        public void SimulatePinch(float distanceDelta)
        {
            if (InputEnabled)
            {
                Pinched?.Invoke(distanceDelta);
            }
        }
    }
}
