# AGENTS.md

## Mission

RoboSharp exists to teach how programming languages and runtimes work by making the whole pipeline visible.

The product is a teaching compiler and programming environment built around a very small C#-inspired language that controls a robot on a grid. Source code is lexed, parsed into a real syntax tree, compiled into visible fake IL, and executed by an interpreter so learners can see code, structure, instructions, runtime state, and world behavior without hidden magic.

When making decisions, prefer clarity, observability, determinism, and teaching value over convenience, cleverness, or feature breadth.

## Documentation

Human-oriented documentation (build, repository layout, diagrams, and topic stubs) lives under [`docs/`](docs/README.md). Start at [`docs/README.md`](docs/README.md). If anything in `docs/` disagrees with this file, follow **AGENTS.md**.

## Non-negotiable architectural rules

- Keep the project agnostic regarding UI and host choices.
- Keep the system highly inspectable. Intermediate representations and runtime state must stay easy to surface in tooling.
- Keep the runtime deterministic.
- Keep the implementation explicit and readable.
- Use dependency injection heavily for composition and runtime seams.
- Do not introduce interface sprawl. Prefer concrete types and sealed types by default. Add interfaces only at true architectural seams.
- Do not add dependencies outside:
  - the .NET BCL
  - `Microsoft.Extensions.*`
  - `TUnit` for tests
  - **Avalonia** (`Avalonia`, `Avalonia.Desktop`, `Avalonia.AvaloniaEdit`, `Avalonia.Themes.*`, `Avalonia.Fonts.*`) for **`RoboSharp.Studio` only**, as the approved code-first desktop host UI (see `docs/studio/technology-stack.md` and `docs/nuget.md`)
- Do not introduce third-party frameworks, helper libraries, ORMs, serializers, UI frameworks, test helpers, mocking libraries, analyzers, or utility packages beyond the allowed set (the Avalonia line above is the explicit Studio exception).

## Solution intent

The solution should support a teaching-oriented pipeline with clear separation between language work, compilation, runtime execution, world state, IO/workspace handling, and optional hosts such as Studio, Player, or web UI.

The preferred overall flow is:

```text
Source
→ Lexer
→ Parser
→ Syntax Tree
→ Semantic Analysis
→ Bound Tree
→ IL Generation
→ Interpreter
→ Runtime Snapshot / Robot World / stdout / stderr
```

## Solution layout guidance

Use a layout that stays flexible across console, Studio, Player, Blazor Server, WPF, or Avalonia hosts.

Recommended project grouping:

```text
RoboSharp.slnx

src/
  RoboSharp.Language/
  RoboSharp.Semantics/
  RoboSharp.IL/
  RoboSharp.Runtime/
  RoboSharp.World/
  RoboSharp.IO/
  RoboSharp.Workspaces/
  RoboSharp.Toolchain/
  RoboSharp.Application/
  RoboSharp.Hosting/
  RoboSharp.Player/
  RoboSharp.Studio/
  RoboSharp.Web/

tests/
  RoboSharp.Language.Tests/
  RoboSharp.Semantics.Tests/
  RoboSharp.IL.Tests/
  RoboSharp.Runtime.Tests/
  RoboSharp.World.Tests/
  RoboSharp.IO.Tests/
  RoboSharp.Workspaces.Tests/
  RoboSharp.Toolchain.Tests/
  RoboSharp.Application.Tests/
  RoboSharp.Architecture.Tests/
```

Keep names aligned with responsibilities. Avoid “Core”, “Common”, “Shared”, or “Utils” dumping grounds.

## Project responsibilities

### `RoboSharp.Language`
Contains syntax-only language concerns:

- tokens
- lexer
- parser
- syntax nodes
- syntax tree
- source spans
- parse diagnostics

No runtime behavior. No UI behavior. No file system behavior.

### `RoboSharp.Semantics`
Contains meaning and type analysis:

- symbol model
- scope model
- binding
- semantic diagnostics
- bound tree
- type conversion rules
- built-in symbol availability from profiles

### `RoboSharp.IL`
Contains the fake executable model:

- instructions
- opcodes
- operands
- compiled function metadata
- debug symbol metadata
- IL lowering
- IL serialization models

### `RoboSharp.Runtime`
Contains interpreter execution:

- execution state
- frames
- evaluation stack
- heap-backed arrays
- stdout / stderr
- step execution
- snapshots
- structured runtime faults

No exception-driven control flow for normal program/runtime failures.

### `RoboSharp.World`
Contains world simulation and analysis:

- `TerrainGrid`
- `ItemGrid`
- `ActorGrid`
- actor state
- movement rules
- world snapshots
- route/lesson analysis helpers
- render projection models

Keep rendering itself out of the core world model.

### `RoboSharp.IO`
Contains storage abstractions and implementations.

This layer owns where bytes live.

Use it for:

- file abstractions
- directory abstractions
- physical file system implementation
- in-memory file system implementation
- overlay/composite file system implementation
- stream and serialization entry points

Data storage must go through IO abstractions rather than directly through raw paths scattered across the solution.

### `RoboSharp.Workspaces`
Contains project/workspace semantics.

This layer owns:

- project discovery
- document enumeration
- obj/bin layout
- active source sets
- project file loading
- artifact path conventions

A workspace is built over an IO/filesystem abstraction. A workspace must not decide physical vs virtual storage.

### `RoboSharp.Toolchain`
Contains orchestration of build stages:

- compile pipeline
- artifact generation
- project build entry points
- package/load compiled programs

### `RoboSharp.Application`
Contains application-level orchestration and use cases shared by hosts.

Use this for:

- run/debug flows
- lesson/profile loading
- view-model-friendly facades
- host-facing orchestration services

### `RoboSharp.Hosting`
Optional host composition root helpers.

Use this for DI registration extensions and host bootstrapping that can be reused by multiple frontends.

### `RoboSharp.Player`
Compiled artifact runtime host.

### `RoboSharp.Studio`
Local IDE host.

### `RoboSharp.Web`
Blazor Server host.

Keep host projects thin.

## Dependency direction

Prefer this direction:

```text
Hosts
  ↓
Application / Toolchain / Workspaces
  ↓
Runtime / World / IL / Semantics / Language / IO
```

Rules:

- `Language` knows nothing about higher layers.
- `Semantics` depends on `Language` only when needed for syntax input and diagnostics.
- `IL` depends on semantic/bound models, not UI.
- `Runtime` depends on IL and World contracts/models.
- `Workspaces` depends on IO, not the other way around.
- Hosts depend inward; inner layers must not depend on hosts.
- UI projects must not leak UI types into inner projects.

## DI rules

Use `Microsoft.Extensions.DependencyInjection` everywhere composition is needed.

Guidance:

- Register services in composition roots.
- Prefer constructor injection.
- Prefer explicit registrations over scanning.
- Prefer options objects and explicit settings records over ambient statics.
- Keep service graphs understandable.
- Use interfaces at system seams, for example:
  - filesystem/workspace boundaries
  - build pipeline boundaries
  - runtime host boundaries
  - rendering adapters
  - lesson/profile providers
- Prefer concrete types internally within a subsystem.
- Do not create an interface for every class.
- Do not use service locator patterns.
- Do not hide core flows behind magic extension chains.

## Interface guidance

Default stance:

- concrete class first
- sealed unless extension is intentional
- record/readonly record struct where data shape benefits

Add interfaces only when one of these is true:

- multiple implementations are expected and useful
- a host seam must be isolated
- in-memory vs physical behavior must vary
- a runtime service genuinely benefits from substitution in tests
- the abstraction simplifies the architecture more than it obscures it

Do not add interfaces only to satisfy style habits.

## IO and workspace rules

Data is stored through the IO/workspace abstractions.

Rules:

- Do not hardcode path manipulation throughout the solution.
- Do not let language/runtime layers talk directly to `File`, `Directory`, `FileInfo`, or raw path strings.
- Keep physical vs in-memory vs overlay behavior below the workspace layer.
- Keep project/document/artifact layout above the IO layer.
- Prefer typed file and directory abstractions over primitive obsession.
- Where physical implementations are used, `DirectoryInfo`, `FileInfo`, and stream-based APIs are preferred over ad hoc string path logic.

## UI rules

### General

UI is replaceable. Core logic must remain UI-agnostic.

### Blazor

If Blazor is used, it must be true Blazor using Server-Side Interactive.

Rules:

- Use Blazor components.
- Do not mix in Razor Pages or MVC-style hybrid patterns.
- Keep UI state driven by application services and immutable snapshots.
- Treat the web UI as a host over the same application/runtime services as any desktop host.

### WPF or Avalonia

If WPF or Avalonia is used:

- use C# syntax exclusively
- no XAML
- use DI
- keep view construction explicit and composable
- do not let code-behind become the application architecture

### Rendering

Rendering is an adapter over world snapshots, not part of the interpreter.

Keep support open for:

- ASCII/headless rendering
- sprite/grid rendering
- debug projection rendering

## Testing rules

Use `TUnit` only.

Rules:

- All test projects live under `tests/`.
- Mirror the source project layout closely.
- Prefer focused unit tests around parser, binder, lowering, runtime stepping, world rules, IO behavior, and workspace behavior.
- Add integration tests for end-to-end compile and run flows.
- Add snapshot-style verification by asserting structured objects and text explicitly, not by introducing extra snapshot libraries.
- Add architecture tests that enforce dependency rules and dependency/package restrictions.
- Test the in-memory and physical IO/workspace implementations with the same behavioral expectations where practical.
- Test through public behavior first.
- Avoid mocks unless the seam is real and valuable. Prefer in-memory implementations and simple fakes written in-project.

Suggested categories:

- lexer/parser tests
- semantic/binding tests
- IL lowering tests
- interpreter step tests
- runtime fault/stdout/stderr tests
- world movement and route-analysis tests
- workspace and artifact-layout tests
- project/build pipeline tests
- UI-agnostic application flow tests
- architecture/dependency guard tests

## Runtime rules

- The interpreter executes RoboSharp IL in plain C#.
- Do not compile RoboSharp IL to CLR IL.
- Normal runtime failures must be structured results, faults, and stderr output; not thrown exceptions.
- Keep snapshots immutable for UI consumption.
- Keep stdout and stderr separate.
- Keep instruction-level stepping central to the design.

## Built-in and lesson rules

- Built-ins are profile-provided capabilities, not globally assumed forever.
- Parser/grammar stay stable; profile gating belongs in semantic analysis and host/application behavior.
- Keep lesson progression explicit and data-driven.

## File and artifact rules

Keep a real-feeling toolchain.

Preferred file roles:

- `.robosharp` = project file
- `.robo` = source files
- `.roboexe` = compiled fake executable
- `obj/` = intermediate outputs
- `bin/` = compiled outputs

Do not collapse source into the project file by default.

## Coding style rules

- Target modern .NET.
- Prefer clear, explicit, idiomatic C#.
- Favor small types with sharp responsibilities.
- Prefer immutable data shapes where practical.
- Avoid reflection-heavy designs.
- Avoid base-class-heavy frameworks.
- Avoid hidden behavior.
- Keep public APIs intentional.
- Keep domain naming didactic and honest.

## What to avoid

- accidental framework lock-in
- unnecessary interfaces
- service locator patterns
- UI logic in compiler/runtime layers
- path-string soup
- static global mutable state
- exception-driven runtime semantics
- package creep
- “shared utilities” buckets with unclear ownership
- hiding the teaching pipeline behind abstractions that obscure the stages

## Agent instructions

When changing this repository:

1. Protect the teaching mission first.
2. Preserve inspectability and determinism.
3. Keep the solution layered and host-agnostic.
4. Prefer DI-based composition with restrained interface use.
5. Route persistence through IO/workspace abstractions.
6. Keep host/UI projects thin.
7. Reject new dependencies unless they are BCL, `Microsoft.Extensions.*`, `TUnit`, or Avalonia for `RoboSharp.Studio` only as documented.
8. Reject XAML.
9. Reject Blazor host patterns that are not true Server-Side Interactive Blazor.
10. Prefer concrete, testable, modern C# over cleverness.
11. Refresh generated diagram docs and `RoboSharp.slnx` by **committing** with `core.hooksPath` set to `.githooks` (pre-commit runs the generators and re-stages outputs). Do not run the hook scripts manually unless hooks are off or you are debugging them; see [`docs/build.md`](docs/build.md).
