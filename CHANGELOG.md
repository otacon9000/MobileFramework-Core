# Changelog

Tutte le modifiche rilevanti al pacchetto sono documentate qui.
Il formato segue [Keep a Changelog](https://keepachangelog.com/) e [Semantic Versioning](https://semver.org/).

## [1.0.2] - 2026-06-13
### Fixed
- Il pacchetto ora include i file `.meta`: senza di essi Unity non risolveva l'assembly `MobileFramework.Core` (errori CS0246/CS0234) e il Test Runner non rilevava i test quando importato da Git URL.
- `SaveSystem`: `SaveKey` e `DataVersion` non vengono più serializzati nel payload (`[JsonIgnore]`). Prima inquinavano il JSON salvato e facevano fallire la migrazione in `MigrateFrom`.
- Test PlayMode: `TestMiniGame` reso `public` per risolvere CS0052 (accessibilità incoerente con `LifecycleTestBase.MiniGame`).

### Changed
- Email dell'autore aggiornata in `package.json`.

## [1.0.0] - 2026-06-12
### Added
- Prima release stabile
- GameManager con state machine estensibile e StateTransitionValidator
- AudioManager con pool di AudioSource senza GC
- SaveSystem con versioning, migrazione automatica e checksum anti-manomissione
- UIManager con stack pannelli e override via IUISlot
- EventBus tipizzato (solo struct)
- LocalizationManager con fallback, plurali e supporto RTL
- InputManager touch/swipe/pinch
- HapticManager iOS/Android
- AppLifecycleHandler con stato OSInterrupt
- ErrorHandler con recovery verso il menu
- Suite test EditMode e PlayMode
