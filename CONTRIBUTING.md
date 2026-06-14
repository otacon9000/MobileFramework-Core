# Contributing to MobileFramework-Core

Thanks for helping improve the framework. Core is shared by every Otaforge title,
so changes here ripple across all games — read this before opening a PR.

## Opening an Issue

Before filing, search [existing issues](https://github.com/otacon9000/MobileFramework-Core/issues)
to avoid duplicates. A good issue includes:

- **Type**: bug, feature request, or question.
- **Unity version** and **package version** (e.g. `6000.0`, `v1.0.2`).
- **For bugs**: steps to reproduce, expected vs. actual behavior, and the full
  Console output (stack trace included). A minimal repro project helps a lot.
- **For features**: the problem you're solving, not just the solution you have in mind.

Label the issue if you can; otherwise a maintainer will triage it.

## Submitting a Pull Request

1. **Open an issue first** for anything beyond a trivial fix, so the approach can be
   agreed on before you write code.
2. **Branch from `main`**: `fix/<short-desc>` or `feat/<short-desc>`.
3. Keep the PR focused — one logical change. Split unrelated work.
4. **Add or update tests.** Bug fixes need a regression test; features need coverage.
   All EditMode and PlayMode tests must be green.
5. Update `CHANGELOG.md` under an `## [Unreleased]` section and, if the public API
   changes, the relevant docs.
6. Make sure the package still imports cleanly from a Git URL (commit the `.meta`
   files Unity generates — a missing `.meta` breaks assembly resolution downstream).
7. Open the PR against `main` with a clear description and a link to the issue.

A maintainer reviews, may request changes, then squash-merges.

## Code Conventions

These apply everywhere, without exception:

| Item | Convention | Example |
|---|---|---|
| Interfaces | `I` + PascalCase | `IMiniGame`, `ISaveSystem` |
| Managers | PascalCase + `Manager` | `AudioManager` |
| States | PascalCase + `State` | `PlayingState` |
| Events (struct) | PascalCase + `Event` | `GameOverEvent` |
| UI panels | PascalCase + `Panel` | `HUDPanel` |
| Save data | PascalCase + `SaveData` | `FlappySaveData` |
| Assemblies | `MobileFramework.` + name | `MobileFramework.Core` |
| Tests | system name + `Tests` | `EventBusTests` |

Architectural rules:

- **Core never references a game.** No `using` toward game assemblies, no concrete
  game types, no gameplay logic in `Runtime/`.
- **Events are always `struct`** (the bus enforces `where T : struct`).
- **No singletons** except `ServiceLocator`. Services are resolved through it or
  injected via `GameContext` (which exposes interfaces only — keeps games testable
  with the `Fake*` doubles).
- **JSON goes through `JsonSerializer`** (the Newtonsoft wrapper). Do not call
  `JsonConvert` / `JsonUtility` directly. Declarative attributes like `[JsonIgnore]`
  on data classes are fine.
- Dependencies flow one way: `Core ← Debug ← Editor ← Tests`. Never a cycle.
- Editor-only and debug-only code lives in its own assembly (`CoreEditor`,
  `CoreDebug`) and must not ship in release builds.

## Versioning

The package follows [Semantic Versioning](https://semver.org/). Given `MAJOR.MINOR.PATCH`:

- **PATCH** (`1.0.x`) — bug fixes, no public API change. Existing games keep working.
- **MINOR** (`1.x.0`) — new, backward-compatible features. Existing games keep working.
- **MAJOR** (`x.0.0`) — breaking changes to `IMiniGame`, `IGameSaveData`, or
  `GameContext`. Existing games must be updated.

Release steps (maintainers):

1. Move `## [Unreleased]` entries into a dated `## [x.y.z]` section in `CHANGELOG.md`.
2. Bump `version` in `package.json` to match.
3. Commit, then tag: `git tag -a vX.Y.Z -m "vX.Y.Z" && git push origin main --tags`.
4. Games pin the tag in their `manifest.json`
   (`...MobileFramework-Core.git#vX.Y.Z`) and update when ready.
