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
| [Documentation stubs — fill order](documentation-todo.md) | Checklist and suggested sequence for authoring the skeleton below |

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
│  ├─ io-abstractions.md
│  ├─ workspace-model.md
│  └─ runtime-hosts.md
│
├─ language/
│  ├─ language-overview.md
│  ├─ syntax.md
│  ├─ types.md
│  ├─ expressions.md
│  ├─ statements.md
│  ├─ functions.md
│  ├─ arrays.md
│  └─ built-in-functions.md
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