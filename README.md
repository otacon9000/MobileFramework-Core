# MobileFramework-Core

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Unity](https://img.shields.io/badge/Unity-6000.0-black?logo=unity)](https://unity.com/releases/unity-6)
[![Version](https://img.shields.io/badge/version-1.0.2-blue)](CHANGELOG.md)

A shared UPM package that provides the runtime backbone for Otaforge mobile games:
state machine, service locator, typed event bus, audio/UI/save/input/haptic/settings
managers, and localization. Core knows nothing about any game; games know everything about Core.

## Quick Install

Unity → **Package Manager → Add package from git URL**:

```
https://github.com/otacon9000/MobileFramework-Core.git#v1.0.2
```

Or add the dependency directly to `Packages/manifest.json`:

```json
"com.otaforge.mobileframework": "https://github.com/otacon9000/MobileFramework-Core.git#v1.0.2"
```

## Systems Overview

| System | Description | Status |
|---|---|---|
| `GameManager` + States | Extensible state machine (Boot → MainMenu → Loading → Playing → Paused → GameOver → Unloading + OSInterrupt/Rewarded), single point of control. | ✅ Stable |
| `ServiceLocator` | Central registry of all services; the only singleton in the framework. | ✅ Stable |
| `EventBus` | Strongly-typed bus, events are always `struct` (`where T : struct`). | ✅ Stable |
| `AudioManager` | Music + SFX with a zero-GC `AudioSource` pool. | ✅ Stable |
| `UIManager` | Panel stack with transitions; game panels override Core panels via `IUISlot`. | ✅ Stable |
| `SaveSystem` | JSON persistence with versioning, automatic `MigrateFrom`, and SHA-256 checksum. | ✅ Stable |
| `InputManager` | Normalized touch / swipe / pinch and gesture recognition. | ✅ Stable |
| `HapticManager` | Unified iOS/Android haptic patterns. | ✅ Stable |
| `SettingsManager` | Audio, language, and accessibility preferences (persisted). | ✅ Stable |
| `LocalizationManager` | Key → text with fallback, runtime language switch, plurals, RTL. | ✅ Stable |
| `AppLifecycleHandler` | `OnApplicationPause/Focus/Quit` → state machine (OSInterrupt). | ✅ Stable |
| `ErrorHandler` | Catches unhandled exceptions and recovers to the menu. | ✅ Stable |
| `JsonSerializer` | Single Newtonsoft access point for the whole framework. | ✅ Stable |
| `BinarySerializer` | Optional binary path for large saves. | 🧪 Experimental |
| Editor tooling | `FrameworkSetupWindow` to configure a new game project. | 🚧 Planned |

## Quick Start

Implement `IMiniGame` — the single contract every game must fulfill:

```csharp
using MobileFramework.Core.Bootstrap;
using MobileFramework.Core.Contracts;
using MobileFramework.Core.Managers.Save;

public sealed class MyMiniGame : IMiniGame
{
    public string GameId => "my_game";

    private GameContext _ctx;

    public void Initialize(GameContext context) => _ctx = context; // all Core services
    public void StartGame()  { /* spawn the level */ }
    public void PauseGame()  { /* freeze gameplay */ }
    public void ResumeGame() { /* unfreeze gameplay */ }
    public void Tick(float deltaTime) { /* per-frame loop, PlayingState only */ }

    public void EndGame(GameOverReason reason) =>
        _ctx.Save.Save(new MySaveData { bestScore = 42 });

    public void Cleanup() { /* release assets, ready to re-init */ }
}

// Versioned save data: SaveKey is the file name, bump DataVersion on schema changes.
public sealed class MySaveData : VersionedSaveData
{
    public override string SaveKey => "my_game";
    public override int DataVersion => 1;
    public int bestScore;
}
```

Register it from a startup script on your game scene, before leaving the main menu:

```csharp
using UnityEngine;
using MobileFramework.Core.Bootstrap;

FindFirstObjectByType<CoreBootstrapper>().RegisterMiniGame(new MyMiniGame());
```

Put a `CoreBootstrapper` on the Core scene (scene 0 in Build Settings) and press Play:
the flow runs `Boot → MainMenu → Loading → Playing`.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for issues, pull requests, code conventions,
and the versioning process.

## Game Template

Start a new title from the project template (placeholder — coming soon):
**https://github.com/otacon9000/MobileFramework-Template**

## License

[MIT](LICENSE) © Otaforge
