using MobileFramework.Core.Contracts;
using MobileFramework.Core.Events;
using MobileFramework.Core.Localization;
using MobileFramework.Core.Managers.Audio;
using MobileFramework.Core.Managers.Haptic;
using MobileFramework.Core.Managers.Input;
using MobileFramework.Core.Managers.Lifecycle;
using MobileFramework.Core.Managers.Save;
using MobileFramework.Core.Managers.Settings;
using MobileFramework.Core.Managers.UI;
using MobileFramework.Core.Robustness;
using MobileFramework.Core.StateMachine;
using MobileFramework.Core.StateMachine.States;
using MobileFramework.Core.Utilities;
using UnityEngine;

namespace MobileFramework.Core.Bootstrap
{
    /// <summary>
    /// Punto d'ingresso del framework. Vive sulla scena Core (scena 0),
    /// crea e registra tutti i servizi nell'ordine corretto, costruisce il
    /// GameContext e avvia la state machine da BootState.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    [DisallowMultipleComponent]
    public sealed class CoreBootstrapper : MonoBehaviour
    {
        [Header("Manager su scena (creati automaticamente se assenti)")]
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private InputManager inputManager;

        [Header("Configurazione")]
        [SerializeField] private string defaultLanguage = "en";

        private ErrorHandler _errorHandler;
        private bool _ownsServices;

        public GameManager GameManager { get; private set; }
        public GameContext Context { get; private set; }
        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            if (ServiceLocator.Instance.TryGet<GameManager>(out _))
            {
                Debug.LogWarning("[CoreBootstrapper] Bootstrapper duplicato, distruggo questa istanza.");
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            InitializeServices();
        }

        private void InitializeServices()
        {
            var locator = ServiceLocator.Instance;

            // 1. Bus eventi: serve a tutti gli altri servizi.
            var events = new EventBus();
            locator.Register<IEventBus>(events);
            locator.Register(events); // tipo concreto, usato solo dal tooling di debug

            // 2. Persistenza e preferenze.
            var saveSystem = new SaveSystem();
            locator.Register<ISaveSystem>(saveSystem);

            var settings = new SettingsManager(saveSystem);
            settings.Load();
            locator.Register<ISettingsManager>(settings);

            // 3. Manager su scena.
            audioManager = EnsureComponent(audioManager);
            uiManager = EnsureComponent(uiManager);
            inputManager = EnsureComponent(inputManager);
            locator.Register<IAudioManager>(audioManager);
            locator.Register<IUIManager>(uiManager);
            locator.Register<IInputManager>(inputManager);

            var haptics = new HapticManager();
            locator.Register<IHapticManager>(haptics);

            // 4. Localizzazione.
            var localization = new LocalizationManager(events);
            var language = string.IsNullOrEmpty(settings.Language) ? defaultLanguage : settings.Language;
            localization.LoadLanguage(language);
            Loc.Bind(localization);
            locator.Register(localization);

            // 5. Utility runtime.
            var coroutineRunner = gameObject.AddComponent<CoroutineRunner>();
            locator.Register(coroutineRunner);

            // 6. Contesto e state machine.
            Context = GameContext.FromLocator(locator);
            uiManager.Initialize(Context);

            GameManager = new GameManager(Context);
            GameManager.RegisterStates(
                new BootState(),
                new MainMenuState(),
                new LoadingState(),
                new PlayingState(),
                new PausedState(),
                new OSInterruptState(),
                new GameOverState(),
                new RewardedState(),
                new UnloadingState());
            locator.Register(GameManager);

            // 7. Robustezza e ciclo di vita app.
            _errorHandler = new ErrorHandler(GameManager);
            _errorHandler.Initialize();
            gameObject.AddComponent<AppLifecycleHandler>().Initialize(GameManager);

            ApplySettings(settings, audioManager, haptics, localization);
            settings.SettingsChanged += () => ApplySettings(settings, audioManager, haptics, localization);

            _ownsServices = true;
            IsInitialized = true;
        }

        private static void ApplySettings(
            ISettingsManager settings,
            IAudioManager audio,
            IHapticManager haptics,
            LocalizationManager localization)
        {
            audio.MusicEnabled = settings.MusicEnabled;
            audio.SfxEnabled = settings.SfxEnabled;
            haptics.HapticsEnabled = settings.HapticsEnabled;

            if (!string.IsNullOrEmpty(settings.Language) && localization.CurrentLanguage != settings.Language)
            {
                localization.LoadLanguage(settings.Language);
            }
        }

        private T EnsureComponent<T>(T existing) where T : Component
        {
            if (existing != null)
            {
                return existing;
            }

            var found = GetComponentInChildren<T>(true);
            return found != null ? found : gameObject.AddComponent<T>();
        }

        private void Start()
        {
            if (IsInitialized)
            {
                GameManager.ChangeState<BootState>();
            }
        }

        private void Update()
        {
            if (IsInitialized)
            {
                GameManager.Tick(Time.deltaTime);
            }
        }

        /// <summary>Il gioco registra qui la sua implementazione di IMiniGame, prima di lasciare il MainMenu.</summary>
        public void RegisterMiniGame(IMiniGame miniGame)
        {
            GameManager.RegisterMiniGame(miniGame);
        }

        private void OnDestroy()
        {
            if (!_ownsServices)
            {
                return;
            }

            _errorHandler?.Dispose();
            ServiceLocator.Instance.Clear();
            ServiceLocator.ResetInstance();
        }
    }
}
