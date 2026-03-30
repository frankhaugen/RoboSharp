# Architecture overview

RoboSharp is a **teaching** compiler and runtime: every stage of the pipeline is meant to be visible and inspectable. This page is a short map; layer boundaries and non-negotiable rules are defined in [`AGENTS.md`](../AGENTS.md).

## Pipeline (intent)

```text
Source
→ Lexer / Parser → Syntax tree
→ Semantic analysis → Bound tree
→ IL generation
→ Interpreter
→ Runtime snapshots, robot world, stdout/stderr
```

## Layers (names only)

| Area | Project(s) | Responsibility (one line) |
| ---- | ---------- | --------------------------- |
| Syntax | `RoboSharp.Language` | Tokens, parse tree, diagnostics—no runtime or IO |
| Meaning | `RoboSharp.Semantics` | Symbols, binding, bound tree |
| Executable model | `RoboSharp.IL` | Fake IL, metadata, lowering |
| Execution | `RoboSharp.Runtime` | Interpreter, stepping, faults, stdout/stderr |
| Simulation | `RoboSharp.World` | Grid, actors, movement rules |
| Storage | `RoboSharp.IO` | File/directory abstractions |
| Projects | `RoboSharp.Workspaces` | Workspace layout over IO |
| Build | `RoboSharp.Toolchain` | Compile pipeline orchestration |
| Use cases | `RoboSharp.Application` | Host-agnostic application services |
| Composition | `RoboSharp.Hosting` | DI registration helpers |
| Hosts | `RoboSharp.Player`, `RoboSharp.Studio`, `RoboSharp.Web` | Thin entry points |

## Where to go next

- Policy and testing philosophy: [`AGENTS.md`](../AGENTS.md)
- Language vs semantics detail: [RoboSharp.Language](language/README.md), [RoboSharp.Semantics](semantics/README.md)
- Building and artifacts: [Build and test](build.md)
- Packages: [NuGet and packages](nuget.md)
