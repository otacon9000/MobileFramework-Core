using System;

namespace MobileFramework.Core.Events
{
    /// <summary>Emesso dal GameManager a ogni transizione di stato. PreviousState è null al primo stato.</summary>
    public readonly struct GameStateChangedEvent
    {
        public readonly Type PreviousState;
        public readonly Type NewState;

        public GameStateChangedEvent(Type previousState, Type newState)
        {
            PreviousState = previousState;
            NewState = newState;
        }
    }
}
