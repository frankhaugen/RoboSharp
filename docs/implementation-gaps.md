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
| Lex / parse / syntax tree | `RoboSharp.Language` | **Yes** | Lexer, parser, syntax model, diagnostics |
| Semantic analysis / bound tree | `RoboSharp.Semantics` | **No** | Spec in [semantics/](semantics/README.md); no product types yet |
| Fake IL / lowering | `RoboSharp.IL` | **No** | Spec stubs under [compiler/](compiler/) |
| Interpreter / stepping / faults | `RoboSharp.Runtime` | **No** | Spec stubs under [runtime/](runtime/) |
| Grid / actors / movement | `RoboSharp.World` | **No** | Substantive docs in [world/](world/README.md) |
| IO abstractions | `RoboSharp.IO` | **Yes** | Physical, in-memory, overlay filesystems |
| Workspace / projects / artifacts | `RoboSharp.Workspaces` | **No** | Design in [workspaces/](workspaces/README.md) |
| Compile orchestration | `RoboSharp.Toolchain` | **No** | Spec stubs under [toolchain/](toolchain/) |
| Host-agnostic use cases | `RoboSharp.Application` | **No** | No `docs/application/` tree yet |
| DI / composition helpers | `RoboSharp.Hosting` | **No** | — |

Until semantics, IL, runtime, and world exist, there is **no end-to-end compile-and-run** of user programs inside the product layers (only isolated language + IO work).

## Hosts and tooling

| Project | Code in `src/` today | Gap vs docs |
| ------- | -------------------- | ----------- |
| `RoboSharp.Studio` | **Partial** | Avalonia shell, pipeline **inspection** (tokens, syntax tree, diagnostics). Missing: real workspace/project model, binder/IL/runtime/world integration, full [debugger](debugger/debugger-architecture.md) (step kinds, breakpoints, synchronized panes), lesson/goals/content from [lessons/](lessons/README.md). |
| `RoboSharp.Player` | **Shell** | Entry point only; no lesson mode or `.roboexe` loop as specified in [player/README.md](player/README.md). |
| `RoboSharp.Web` | **Shell** | Entry point only; no Blazor teaching UI as implied by layout / `AGENTS.md`. |

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

These are **policy or skeleton** docs without matching implementation work tracked above:

- [governance/](governance/) pages are empty or one-line placeholders.
- Many [compiler/](compiler/), [runtime/](runtime/), and [toolchain/](toolchain/) pages are stubs or missing prose; they describe intended systems more than current code.

Use [documentation-todo.md](documentation-todo.md) for per-file doc status.

## Test projects

`tests/` mirrors many `RoboSharp.*` projects. **Meaningful product tests** today align with code that exists (notably Language and IO, plus architecture/dependency guards). Several projects (e.g. Semantics, Runtime, World) still hold **scaffold or placeholder** tests while their matching `src/` projects are empty—do not read those as evidence those layers are implemented.

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

- [Architecture overview](architecture.md)
- [Repository layout](repository-layout.md)
- [Documentation checklist](documentation-todo.md)
- [Lessons and content specification](lessons/README.md)
