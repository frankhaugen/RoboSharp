# Implementation gaps and missing features

This page is the **shared picture** of what the repository intends to build (see [`AGENTS.md`](../AGENTS.md) and the topic docs under `docs/`) versus what **exists in `src/` today**. It complements [documentation-todo.md](documentation-todo.md), which tracks **documentation** completeness and uses the same **Implemented** vocabulary per doc row.

**Refresh:** Re-scan `src/` after meaningful changes. From the repo root:

```bash
pwsh -File tools/doc-checklist.ps1 -Command SrcMetrics
```

Treat this file as **stale** if project file counts diverge sharply from reality; prefer updating the table below over guessing.

## Pipeline: where code is missing

The teaching pipeline in `AGENTS.md` is:

```text
Source → Lexer → Parser → Syntax tree → Semantic analysis → Bound tree
→ IL generation → Interpreter → Runtime / world / stdout / stderr
```

| Stage | Project | Code in `src/` today | Notes |
| ----- | ------- | -------------------- | ----- |
| Lex / parse / syntax tree | `RoboSharp.Language` | **Yes** | Lexer, parser, syntax model, diagnostics (`void` keyword for `main`) |
| Semantic analysis / bound tree | `RoboSharp.Semantics` | **Partial** | v1 binder, `BuiltinId`, profiles seam, bound nodes; not full spec in [semantics/](semantics/README.md) |
| Fake IL / lowering | `RoboSharp.IL` | **Partial** | `RoboOpcode`, `Instruction`, `IlLowerer` → `RoboProgram` |
| Interpreter / stepping / faults | `RoboSharp.Runtime` | **Partial** | `RoboInterpreter`, structured `RuntimeFault`; no stepping API yet |
| Grid / actors / movement | `RoboSharp.World` | **Partial** | Grids, `RobotWorld`, snapshots, primary-robot builtins; push not implemented |
| IO abstractions | `RoboSharp.IO` | **Yes** | Physical, in-memory, overlay filesystems |
| Workspace / projects / artifacts | `RoboSharp.Workspaces` | **Yes** | Load/save `.robosharp`, artifact layout, in-memory + physical workspaces |
| Compile orchestration | `RoboSharp.Toolchain` | **Partial** | `RoboSharpPipeline`, `RoboSharpCompiler`, JSON `.roboexe`; `WorkspaceBuildService` writes IL + exe from workspace sources |
| Host-agnostic use cases | `RoboSharp.Application` | **Partial** | `IRoboSharpExecutionService` (run source / JSON exe / build+run workspace); no lesson/profile layer yet |
| DI / composition helpers | `RoboSharp.Hosting` | **Partial** | `AddRoboSharpHosting()` composes workspaces + application services |

There is a **minimal end-to-end path**: parse → bind → lower → interpret with `RoboSharpPipeline` and a `RobotWorld` instance. **`RoboSharpCompiler`** exposes the same compile phases without running; **`RoboExecutable`** + **`RoboExecutableJsonSerializer`** provide a v1 JSON interchange for fake executables (teaching). **`RoboInterpreterSession`** supports instruction stepping and step limits per [runtime/v1-runtime-spec.md](runtime/v1-runtime-spec.md).

Gaps: `.robosharp` project load, workspace integration, binary `.roboexe`, full snapshot model, lesson/profile loading, and parity with per-frame evaluation stacks described in the v1 runtime spec.

## Hosts and tooling

| Project | Code in `src/` today | Gap vs docs |
| ------- | -------------------- | ----------- |
| `RoboSharp.Studio` | **Partial** | Avalonia shell, pipeline **inspection** (tokens, syntax tree, diagnostics). Missing: real workspace/project model, binder/IL/runtime/world integration, full [debugger](debugger/debugger-architecture.md) (step kinds, breakpoints, synchronized panes), lesson/goals/content from [lessons/](lessons/README.md). |
| `RoboSharp.Player` | **Partial** | Runs a v1 JSON `.roboexe` from disk with exit codes per [toolchain/v1-toolchain-spec.md](toolchain/v1-toolchain-spec.md) §11; lesson mode still unspecified in code. |
| `RoboSharp.Web` | **Partial** | `AddRoboSharpHosting()` + home-page pipeline smoke; full teaching UI still open. |

## Teaching / product layer (lessons, goals, packs)

The educational backbone is **specified** under [lessons/](lessons/README.md) (profiles, goals, lesson definitions, content packs, JSON direction) but **not implemented** as dedicated types or hosts. Nothing in `src/` yet provides:

- builtin profile providers for the binder
- goal evaluators or lesson sessions
- content pack loading
- world file format + loader tied to lessons

That is the largest **feature** gap relative to “teaching platform” intent.

## Debugger documentation vs implementation

[debugger/debugger-architecture.md](debugger/debugger-architecture.md) describes a full snapshot-based debugger (step into/over/out, continue, pause, breakpoints, stack/locals/world/metrics). Related pages [stepping.md](debugger/stepping.md), [state-inspection.md](debugger/state-inspection.md), and [metrics-view.md](debugger/metrics-view.md) are still **empty placeholders**—they are documentation gaps, and the **Studio runtime debugger** they describe is largely **not built** yet.

## Documentation-only areas

These are **policy or skeleton** docs without matching depth, or specs that still lag behind code:

- [governance/dependency-policy.md](governance/dependency-policy.md) and [governance/implementation-order.md](governance/implementation-order.md) are still thin.
- Many [compiler/](compiler/), [runtime/](runtime/), and [toolchain/](toolchain/) pages are stubs or missing prose relative to the implemented pipeline.
- A consolidated list of **missing specs** (not just missing code) lives in [missing-specs.md](missing-specs.md).

Use [documentation-todo.md](documentation-todo.md) for per-file doc status.

## Test projects

`tests/` mirrors `RoboSharp.*` projects. Each area has **behavioral tests** tied to the real pipeline (lexer/parser, binder, lowering, interpreter, toolchain, workspaces, world movement, application/hosting composition, solution layout, and allowed project-reference edges). Expand coverage as features grow; there is no separate TUnit demo suite in-repo.

## What this is not

- **Not a roadmap with dates** — priority order can follow `AGENTS.md` and teaching value; update this page when major layers land.
- **Not a substitute for tests** — use failing/passing tests and this doc together; they answer different questions.

## Recommended specification order (middle of the stack)

When writing specs to unblock implementation, a coherent order is:

1. **Fake IL instruction set and lowering** — Opcodes, operands, stack/locals, function metadata, `CallBuiltin`, plus syntax-to-IL examples ([compiler/syntax-to-il-lowering.md](compiler/syntax-to-il-lowering.md), [runtime/il-instruction-set.md](runtime/il-instruction-set.md)).
2. **Built-in and world interaction** — Built-in ids, signatures, profile availability, runtime side effects, mapping to `RobotWorld` ([semantics/builtins-and-profiles.md](semantics/builtins-and-profiles.md), [world/](world/README.md)).
3. **World state and snapshots** — Layered grids, movement/pickup/passability, snapshot and render projection contracts ([world/world-model.md](world/world-model.md), [rendering/render-projection.md](rendering/render-projection.md)).

Layer ownership summary: [architecture/pipeline-boundaries.md](architecture/pipeline-boundaries.md).

## Related links

- [Missing specs (doc gaps)](missing-specs.md)
- [Architecture overview](architecture.md)
- [Repository layout](repository-layout.md)
- [Documentation checklist](documentation-todo.md)
- [Lessons and content specification](lessons/README.md)
