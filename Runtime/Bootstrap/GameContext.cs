using System;
using MobileFramework.Core.Events;
using MobileFramework.Core.Managers.Audio;
using MobileFramework.Core.Managers.Haptic;
using MobileFramework.Core.Managers.Input;
using MobileFramework.Core.Managers.Save;
using MobileFramework.Core.Managers.Settings;
using MobileFramework.Core.Managers.UI;

namespace MobileFramework.Core.Bootstrap
{
    /// <summary>
    /// Pacchetto di servizi consegnato a IMiniGame.Initialize. Espone solo
    /// interfacce, mai classi concrete: il gioco resta testabile con i Fake*
    /// e il Core può cambiare implementazione senza rompere i titoli.
    /// </summary>
    public sealed class GameContext
    {
        public IEventBus Events { get; }
        public IAudioManager Audio { get; }
        public IUIManager UI { get; }
        public ISaveSystem Save { get; }
        public IInputManager Input { get; }
        public IHapticManager Haptics { get; }
        public ISettingsManager Settings { get; }

        public GameContext(
            IEventBus events,
            IAudioManager audio,
            IUIManager ui,
            ISaveSystem save,
            IInputManager input,
            IHapticManager haptics,
            ISettingsManager settings)
        {
            Events = events ?? throw new ArgumentNullException(nameof(events));
            Audio = audio ?? throw new ArgumentNullException(nameof(audio));
            UI = ui ?? throw new ArgumentNullException(nameof(ui));
            Save = save ?? throw new ArgumentNullException(nameof(save));
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Haptics = haptics ?? throw new ArgumentNullException(nameof(haptics));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>Costruisce il contesto dai servizi già registrati nel locator.</summary>
        public static GameContext FromLocator(ServiceLocator locator)
        {
            return new GameContext(
                locator.Get<IEventBus>(),
                locator.Get<IAudioManager>(),
                locator.Get<IUIManager>(),
                locator.Get<ISaveSystem>(),
                locator.Get<IInputManager>(),
                locator.Get<IHapticManager>(),
                locator.Get<ISettingsManager>());
        }
    }
}
