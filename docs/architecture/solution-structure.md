# Solution structure

How the **repository and solution** organize projects under `src/` and `tests/`.

## Solution file

`RoboSharp.slnx` is the SDK-style XML solution listing:

- All C# projects (`src/*`, `tests/*`)
- Documentation folders under `docs/` (nested paths appear as separate top-level folders for IDE clarity — see [../repository-layout.md](../repository-layout.md))

Regenerate project membership by committing with `core.hooksPath` set to `.githooks`, or follow [../build.md](../build.md) if hooks are off.

## Source projects (intent)

| Project | Responsibility |
| ------- | -------------- |
| `RoboSharp.Language` | Lexer, parser, syntax, parse diagnostics |
| `RoboSharp.Semantics` | Symbols, binding, bound tree, semantic diagnostics |
| `RoboSharp.IL` | Opcodes, program model, lowering |
| `RoboSharp.Runtime` | Interpreter, execution results |
| `RoboSharp.World` | Grid world model and rules |
| `RoboSharp.IO` | Storage abstractions |
| `RoboSharp.Workspaces` | Projects, sessions, artifact paths |
| `RoboSharp.Toolchain` | Compile pipeline, serialization |
| `RoboSharp.Application` | Host-agnostic use cases |
| `RoboSharp.Hosting` | Composition helpers |
| `RoboSharp.Player` | Artifact runtime host |
| `RoboSharp.Studio` | IDE host (Avalonia) |
| `RoboSharp.Web` | Blazor host (when present) |

## Test projects

Mirror `src/` with `*.Tests` and `RoboSharp.Architecture.Tests` for dependency/package guards.

## Related

- [../repository-layout.md](../repository-layout.md)
- [dependency-rules.md](dependency-rules.md)
- [../diagrams/project-references.md](../diagrams/project-references.md)
