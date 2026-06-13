# MobileFramework-Core

Pacchetto UPM condiviso da tutti i giochi mobile dello studio.
Regola fondamentale: **il Core non sa nulla dei giochi. I giochi sanno tutto del Core.**

## Installazione

Package Manager → *Add package from git URL*:

```
https://github.com/otacon9000/MobileFramework-Core.git#v1.0.2
```

## Avvio rapido

1. Crea una scena `Core.unity` con un GameObject e il componente `CoreBootstrapper` (scena 0 in Build Settings).
2. Implementa `IMiniGame` nel tuo gioco e registralo:

```csharp
public sealed class FlappyMiniGame : IMiniGame { /* ... */ }

// in uno script di avvio del gioco:
ServiceLocator.Instance.Get<GameManager>(); // i servizi sono già attivi
FindFirstObjectByType<CoreBootstrapper>().RegisterMiniGame(new FlappyMiniGame());
```

3. Implementa `IGameSaveData` (o estendi `VersionedSaveData`) per i salvataggi.
4. Premi Play: il flusso è `Boot → MainMenu → Loading → Playing`.

## Architettura in breve

- **ServiceLocator** — registro centrale dei servizi; nessun Singleton nel codice di gioco.
- **GameContext** — pacchetto di interfacce (`IAudioManager`, `IUIManager`, `ISaveSystem`, …) consegnato a `IMiniGame.Initialize`.
- **EventBus** — bus tipizzato; gli eventi sono sempre `struct`. `UnloadingState` chiama `Clear()`.
- **GameManager** — macchina a stati; le transizioni valide sono definite in `StateTransitionValidator`.
- **SaveSystem** — JSON (Newtonsoft) + checksum SHA-256; migrazione automatica via `IGameSaveData.MigrateFrom`.

Vedi `SPEC.md` per la struttura completa e le regole di nomenclatura.

## Test

Window → General → Test Runner: suite EditMode (logica pura) e PlayMode (lifecycle e UI stack).
