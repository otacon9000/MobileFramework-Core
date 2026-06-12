using System;
using UnityEngine;

namespace MobileFramework.Core.Managers.Input
{
    public enum SwipeDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    /// <summary>
    /// Input normalizzato per mobile: tap, swipe e pinch.
    /// Gli stati del Core disabilitano l'input fuori dal gameplay via InputEnabled.
    /// </summary>
    public interface IInputManager
    {
        bool InputEnabled { get; set; }

        /// <summary>Tap rilevato; posizione in screen space.</summary>
        event Action<Vector2> Tapped;

        /// <summary>Swipe rilevato; direzione e posizione di partenza in screen space.</summary>
        event Action<SwipeDirection, Vector2> Swiped;

        /// <summary>Pinch in corso; delta della distanza tra le due dita rispetto al frame precedente (pixel).</summary>
        event Action<float> Pinched;
    }
}
