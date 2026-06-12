# Changelog

Tutte le modifiche rilevanti al pacchetto sono documentate qui.
Il formato segue [Keep a Changelog](https://keepachangelog.com/) e [Semantic Versioning](https://semver.org/).

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
