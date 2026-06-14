# Changelog

All notable changes to this package are documented here.
This project follows [Keep a Changelog](https://keepachangelog.com/) and
[Semantic Versioning](https://semver.org/).

## [1.0.2] - 2026-06-13
### Fixed
- The package now ships its `.meta` files. Without them, importing from a Git URL
  failed to resolve the `MobileFramework.Core` assembly (CS0246/CS0234) and the Test
  Runner did not detect the package tests.
- `SaveSystem`: `SaveKey` and `DataVersion` are no longer serialized into the payload
  (`[JsonIgnore]`). They previously polluted the saved JSON and broke schema migration
  in `MigrateFrom`. Metadata now lives only in the save envelope.
- PlayMode tests: `TestMiniGame` is now `public`, resolving CS0052 (inconsistent
  accessibility with `LifecycleTestBase.MiniGame`).

### Changed
- Author email and repository URL (`otacon9000`) corrected in `package.json` / docs.
- `package.json` version aligned to the published tag.

## [1.0.0] - 2026-06-12
### Added
- First stable release.
- `GameManager` with an extensible state machine and `StateTransitionValidator`.
- `AudioManager` with a zero-GC `AudioSource` pool.
- `SaveSystem` with versioning, automatic migration, and an anti-tamper checksum.
- `UIManager` with a panel stack and overrides via `IUISlot`.
- Strongly-typed `EventBus` (struct-only).
- `LocalizationManager` with fallback, plurals, and RTL support.
- `InputManager` for touch / swipe / pinch.
- `HapticManager` for iOS/Android.
- `AppLifecycleHandler` with an OSInterrupt state.
- `ErrorHandler` with recovery to the menu.
- EditMode and PlayMode test suites.
