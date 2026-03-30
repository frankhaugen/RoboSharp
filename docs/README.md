# RoboSharp documentation

This folder is the home for human-oriented documentation: how the repo is organized, how to build and test it, and how the teaching pipeline fits together.

The **authoritative** rules for architecture, dependencies, and agent behavior live in the repository root as [`AGENTS.md`](../AGENTS.md). When docs and `AGENTS.md` disagree, trust `AGENTS.md`.

## Contents

| Document | Purpose |
| -------- | ------- |
| [Build and test](build.md) | SDK, restore, tests, diagram generation, `RoboSharp.slnx`, git hooks |
| [Diagrams](diagrams/README.md) | Generated Mermaid graphs (projects, NuGet, layers) |
| [Repository layout](repository-layout.md) | Solution structure, projects, and dependency direction |
| [NuGet and packages](nuget.md) | Central package management, allowed packages, feed policy |
| [Architecture overview](architecture.md) | High-level pipeline and layer responsibilities (summary only) |
| [Solution structure](architecture/solution-structure.md) | `RoboSharp.slnx`, `src/` projects; see also [dependency-rules](architecture/dependency-rules.md), [dependency-injection](architecture/dependency-injection.md), [runtime-hosts](architecture/runtime-hosts.md) |
| [Compiler v1 spec](compiler/v1-compiler-spec.md) | Normative compiler phases, lowering model, artifact intent |
| [Compiler topics](compiler/compilation-pipeline.md) | Pipeline overview; [IL generation](compiler/il-generation.md), [diagnostics](compiler/diagnostics.md) |
| [Runtime v1 spec](runtime/v1-runtime-spec.md) | Load, stepping, faults, snapshots (contract vs `RoboSharp.Runtime`) |
| [Runtime topics](runtime/interpreter.md) | Interpreter, [execution model](runtime/execution-model.md), [state](runtime/runtime-state.md), [errors](runtime/error-handling.md), [stdout/stderr](runtime/standard-output.md) |
| [Toolchain v1 spec](toolchain/v1-toolchain-spec.md) | Studio vs Player, build lifecycle, CLI shape |
| [Toolchain topics](toolchain/build-process.md) | [Project format](toolchain/project-format.md), build/clean, [artifacts](toolchain/artifact-layout.md), [.roboexe JSON](toolchain/roboexe-format.md) |
| [Debugger topics](debugger/debugger-architecture.md) | [Stepping](debugger/stepping.md), [state inspection](debugger/state-inspection.md), [metrics view](debugger/metrics-view.md) |
| [Mission (teaching goals)](governance/mission.md) | Why RoboSharp exists; [design principles](governance/design-principles.md); [dependency policy](governance/dependency-policy.md); [implementation order](governance/implementation-order.md) |
| [Pipeline boundaries](architecture/pipeline-boundaries.md) | Syntax, semantics, IL, world, built-in contracts (end-to-end model) |
| [IO layer](io/README.md) | `RoboSharp.IO`: filesystem abstractions, physical/in-memory/overlay |
| [Workspaces](workspaces/README.md) | `RoboSharp.Workspaces`: projects, sessions, artifacts on top of IO |
| [IO ↔ workspace boundary](architecture/io-workspace-overview.md) | How IO and workspaces divide responsibility (start here) |
| [RoboSharp.Language](language/README.md) | Tokens, lexer, parser, syntax tree (`docs/language/`) |
| [RoboSharp.Semantics](semantics/README.md) | Symbols, binding, bound tree, semantic diagnostics (`docs/semantics/`) |
| [RoboSharp.World](world/README.md) | Terrain, items, actors, movement, snapshots (`docs/world/`) |
| [Rendering adapters](rendering/README.md) | Projectors and renderers over world snapshots (`docs/rendering/`) |
| [Lessons, goals, and content packs](lessons/README.md) | Profiles, goals, lesson definitions, packs, JSON direction (`docs/lessons/`) |
| [RoboSharp.Player](player/README.md) | Compiled-artifact host; lesson run mode (`docs/player/`) |
| [RoboSharp Studio](studio/README.md) | IDE host specifications (topic index; former `general-specs.md`) |
| [Implementation gaps](implementation-gaps.md) | What is specified vs implemented in `src/` (missing pipeline, hosts, lessons) |
| [Missing specs](missing-specs.md) | Topics that still lack a full written spec (artifacts, debugger, lessons runtime, etc.) |
| [Documentation stubs — fill order](documentation-todo.md) | Checklist and suggested sequence for authoring the skeleton below |
| [Agent workflows](agents/README.md) | Repeatable agent tasks (e.g. syncing the documentation checklist with `tools/doc-checklist.ps1`) |

## Contributing to docs

Prefer short, accurate pages over long essays. Link to `AGENTS.md` for policy instead of duplicating it. Use relative links between docs files.

## Docs initial skeleton

```plaintext
docs/
│
├─ architecture/
│  ├─ solution-structure.md
│  ├─ dependency-rules.md
│  ├─ dependency-injection.md
│  ├─ io-workspace-overview.md
│  ├─ io-abstractions.md
│  ├─ workspace-model.md
│  └─ runtime-hosts.md
│
├─ io/
│  └─ (README + principles, abstractions, storage topics — see folder)
│
├─ workspaces/
│  └─ (README + project/session/artifact topics — see folder)
│
├─ language/
│  └─ (README + overview, syntax, lexer/parser/AST topics — see folder)
│
├─ semantics/
│  └─ (README + types, symbols, binding, diagnostics — see folder)
│
├─ compiler/
│  ├─ compilation-pipeline.md
│  ├─ lexical-analysis.md
│  ├─ parsing.md
│  ├─ syntax-tree.md
│  ├─ semantic-analysis.md
│  ├─ diagnostics.md
│  └─ il-generation.md
│
├─ runtime/
│  ├─ il-instruction-set.md
│  ├─ interpreter.md
│  ├─ execution-model.md
│  ├─ runtime-state.md
│  ├─ error-handling.md
│  └─ standard-output.md
│
├─ world/
│  ├─ world-model.md
│  ├─ terrain-grid.md
│  ├─ item-grid.md
│  ├─ actor-grid.md
│  ├─ world-actions.md
│  ├─ movement-rules.md
│  └─ metrics-and-analysis.md
│
├─ lessons/
│  ├─ README.md
│  ├─ builtin-profiles.md
│  ├─ goals-and-evaluation.md
│  ├─ lesson-model.md
│  ├─ content-packs-sessions-and-metrics.md
│  └─ json-formats.md
│
├─ player/
│  └─ README.md
│
├─ rendering/
│  ├─ render-projection.md
│  ├─ ascii-renderer.md
│  └─ sprite-renderer.md
│
├─ toolchain/
│  ├─ project-format.md
│  ├─ roboexe-format.md
│  ├─ build-process.md
│  └─ artifact-layout.md
│
├─ debugger/
│  ├─ debugger-architecture.md
│  ├─ breakpoints.md
│  ├─ stepping.md
│  ├─ state-inspection.md
│  └─ metrics-view.md
│
├─ studio/
│  ├─ ide-layout.md
│  ├─ editor-behavior.md
│  ├─ syntax-highlighting.md
│  ├─ lesson-profiles.md
│  └─ visualization.md
│
└─ governance/
   ├─ mission.md
   ├─ design-principles.md
   ├─ dependency-policy.md
   └─ implementation-order.md
```

Suggested sequence for filling these stubs: [documentation-todo.md](documentation-todo.md).