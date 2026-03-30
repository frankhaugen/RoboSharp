# RoboSharp Studio documentation

Specifications for **RoboSharpStudio**, the desktop IDE host. Authoritative repo rules remain in [`AGENTS.md`](../../AGENTS.md).

| Topic | Document |
| ----- | -------- |
| Role of Studio, goals | [overview.md](overview.md) |
| Avalonia, DI, code-first UI | [technology-stack.md](technology-stack.md) |
| Illustrative solution shape (vs `AGENTS.md` layout) | [referenced-solution-shape.md](referenced-solution-shape.md) |
| Composition root, DI, domain sessions, core types | [composition-and-domain.md](composition-and-domain.md) |
| Shell tree, main window, navigation | [ide-layout.md](ide-layout.md) |
| Workspace over IO abstractions | [workspace-integration.md](workspace-integration.md) |
| Documents and source editor | [editor-behavior.md](editor-behavior.md) |
| Build and analysis modes inside Studio | [build-and-analysis.md](build-and-analysis.md) |
| Tokens, syntax, bound, IL panels | [inspection-panels.md](inspection-panels.md) |
| World view in Studio | [visualization.md](visualization.md) |
| stdout/stderr, metrics, runtime state panes | [output-and-state-panels.md](output-and-state-panels.md) |
| Lessons, profiles, goals | [lesson-profiles.md](lesson-profiles.md) |
| Menus and commands | [menus-and-commands.md](menus-and-commands.md) |
| Settings | [settings.md](settings.md) |
| Theming | [theming.md](theming.md) |
| Syntax / execution highlighting | [syntax-highlighting.md](syntax-highlighting.md) |
| Extensibility seams | [extensibility.md](extensibility.md) |
| Studio project modules | [project-modules.md](project-modules.md) |
| Testing Studio | [testing-strategy.md](testing-strategy.md) |
| Performance | [performance.md](performance.md) |
| Non-goals, MVP vs full v1 | [scope-mvp-and-non-goals.md](scope-mvp-and-non-goals.md) |
| Debugger (host integration) | [../debugger/debugger-architecture.md](../debugger/debugger-architecture.md), [../debugger/breakpoints.md](../debugger/breakpoints.md) |

Legacy single-file entry (redirect): [general-specs.md](general-specs.md).

## Implementation layout (`src/RoboSharp.Studio/`)

Code-first Avalonia host (no XAML): **`Composition/`** (DI + root provider), **`Pipeline/`** (lexer/parser inspection seam), **`Panels/`** (`IStudioPanel` tabs in pipeline order), **`Shell/`** (theme tokens, `MainWindow`, `StudioApp`), **`ViewModels/`**. Adding a teaching panel: implement `IStudioPanel`, register in `StudioServiceRegistration`.
