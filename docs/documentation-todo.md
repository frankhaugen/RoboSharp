# Documentation checklist

Working index for `docs/`: what exists, how complete it is, and how it lines up with **code in `src/`**. Suggested authoring order follows **teaching flow** (why → repo shape → language → compile → run → world → presentation → tooling), top to bottom within each section.

Many paths are also sketched in [README.md](README.md). Authoritative policy remains [AGENTS.md](../AGENTS.md).

For a **single view of missing product features** (pipeline stages, hosts, lessons, debugger) versus current `src/` code, see [implementation-gaps.md](implementation-gaps.md).

## How to read the columns

| Column | Meaning |
| ------ | ------- |
| **Have content** | **Yes** — enough prose to be useful as a doc (about 20+ non-empty lines, or a clearly complete shorter page). **Stub** — file exists but is thin, redirect-only, or outline-only. **No** — missing file, or empty placeholder (0 lines). |
| **Implemented** | **Yes** — the described subsystem has a real implementation under `src/` today. **Partial** — shell, layout, or cross-cutting pieces exist (e.g. solution layout, minimal host). **No** — project folder exists but no meaningful product code yet, or feature not present. **N/A** — meta docs (build, diagrams, governance pointers), not a product subsystem. |

Re-scan after large doc or code changes: line counts and `src/` contents drift.

**Maintaining this file:** Run `pwsh -File tools/doc-checklist.ps1 -Command All` from the repo root for a full pass, or **`-Shard S1` … `S5`** for disjoint slices. For parallel agent work, use [agents/documentation-checklist/parent-orchestration.md](agents/documentation-checklist/parent-orchestration.md) (map → reduce; one merge into this file).

## Entry and meta

| Document | Have content | Implemented |
| -------- | ------------ | ----------- |
| [README.md](README.md) | Yes | N/A |
| [implementation-gaps.md](implementation-gaps.md) | Yes | N/A |
| [acknowledgements.md](acknowledgements.md) | Yes | N/A |
| [missing-specs.md](missing-specs.md) | Yes | N/A |
| [docs-for-spreading.md](docs-for-spreading.md) | Yes | N/A |
| [build.md](build.md) | Yes | N/A |
| [repository-layout.md](repository-layout.md) | Yes | Partial |
| [nuget.md](nuget.md) | Yes | N/A |
| [architecture.md](architecture.md) | Yes | Partial |
| [diagrams/README.md](diagrams/README.md) | Stub | N/A |
| [diagrams/layer-map.md](diagrams/layer-map.md) | Yes | N/A |
| [diagrams/project-references.md](diagrams/project-references.md) | Yes | N/A |
| [diagrams/nuget-references.md](diagrams/nuget-references.md) | Yes | N/A |
| [diagrams/architecture/notes.md](diagrams/architecture/notes.md) | Stub | N/A |

## Governance

Policy detail lives in [AGENTS.md](../AGENTS.md); these pages are optional elaborations.

| Document | Have content | Implemented |
| -------- | ------------ | ----------- |
| [governance/mission.md](governance/mission.md) | Yes | N/A |
| [governance/design-principles.md](governance/design-principles.md) | Yes | N/A |
| [governance/dependency-policy.md](governance/dependency-policy.md) | No | N/A |
| [governance/implementation-order.md](governance/implementation-order.md) | No | N/A |

## Architecture

| Document | Have content | Implemented |
| -------- | ------------ | ----------- |
| [architecture/solution-structure.md](architecture/solution-structure.md) | No | Partial |
| [architecture/dependency-rules.md](architecture/dependency-rules.md) | No | N/A |
| [architecture/dependency-injection.md](architecture/dependency-injection.md) | No | N/A |
| [architecture/io-workspace-overview.md](architecture/io-workspace-overview.md) | Yes | Partial |
| [architecture/workspace-model.md](architecture/workspace-model.md) | Stub | No |
| [architecture/io-abstractions.md](architecture/io-abstractions.md) | Stub | Yes |
| [architecture/runtime-hosts.md](architecture/runtime-hosts.md) | No | Partial |
| [architecture/pipeline-boundaries.md](architecture/pipeline-boundaries.md) | Yes | N/A |

## IO layer (`docs/io/`)

Index: [io/README.md](io/README.md).

| Document | Have content | Implemented |
| -------- | ------------ | ----------- |
| [io/README.md](io/README.md) | Stub | Yes |
| [io/principles.md](io/principles.md) | Yes | Yes |
| [io/canonical-abstractions.md](io/canonical-abstractions.md) | Yes | Yes |
| [io/physical-storage.md](io/physical-storage.md) | Stub | Yes |
| [io/in-memory-storage.md](io/in-memory-storage.md) | Stub | Yes |
| [io/overlay-storage.md](io/overlay-storage.md) | Yes | Yes |
| [io/optional-storage-seam.md](io/optional-storage-seam.md) | Stub | Yes |
| [io/helpers-and-errors.md](io/helpers-and-errors.md) | Yes | Yes |

## Workspaces layer (`docs/workspaces/`)

Index: [workspaces/README.md](workspaces/README.md).

| Document | Have content | Implemented |
| -------- | ------------ | ----------- |
| [workspaces/README.md](workspaces/README.md) | Stub | No |
| [workspaces/principles.md](workspaces/principles.md) | Stub | No |
| [workspaces/kinds.md](workspaces/kinds.md) | Stub | No |
| [workspaces/contracts.md](workspaces/contracts.md) | Yes | No |
| [workspaces/project-loading.md](workspaces/project-loading.md) | Yes | No |
| [workspaces/artifact-layout.md](workspaces/artifact-layout.md) | Yes | No |
| [workspaces/sessions-and-documents.md](workspaces/sessions-and-documents.md) | Yes | No |
| [workspaces/configuration.md](workspaces/configuration.md) | Stub | No |
| [workspaces/lesson-metadata.md](workspaces/lesson-metadata.md) | Yes | No |
| [workspaces/build-pipeline-integration.md](workspaces/build-pipeline-integration.md) | Stub | No |
| [workspaces/temporary-workspace.md](workspaces/temporary-workspace.md) | Stub | No |
| [workspaces/studio-overlay-and-save.md](workspaces/studio-overlay-and-save.md) | Yes | No |
| [workspaces/concrete-types-and-di.md](workspaces/concrete-types-and-di.md) | Yes | No |
| [workspaces/non-goals-and-summary.md](workspaces/non-goals-and-summary.md) | Yes | No |

## Lessons and content (`docs/lessons/`)

Index: [lessons/README.md](lessons/README.md).

| Document | Have content | Implemented |
| -------- | ------------ | ----------- |
| [lessons/README.md](lessons/README.md) | Yes | No |
| [lessons/builtin-profiles.md](lessons/builtin-profiles.md) | Yes | No |
| [lessons/goals-and-evaluation.md](lessons/goals-and-evaluation.md) | Yes | No |
| [lessons/lesson-model.md](lessons/lesson-model.md) | Yes | No |
| [lessons/content-packs-sessions-and-metrics.md](lessons/content-packs-sessions-and-metrics.md) | Yes | No |
| [lessons/json-formats.md](lessons/json-formats.md) | Yes | No |

## Player (`docs/player/`)

| Document | Have content | Implemented |
| -------- | ------------ | ----------- |
| [player/README.md](player/README.md) | Stub | Partial |

## Language (`RoboSharp.Language`)

Index: [language/README.md](language/README.md).

| Document | Have content | Implemented |
| -------- | ------------ | ----------- |
| [language/README.md](language/README.md) | Yes | Yes |
| [language/language-overview.md](language/language-overview.md) | Yes | Yes |
| [language/syntax.md](language/syntax.md) | Yes | Yes |
| [language/functions.md](language/functions.md) | Yes | Yes |
| [language/statements.md](language/statements.md) | Stub | Yes |
| [language/expressions.md](language/expressions.md) | Stub | Yes |
| [language/types.md](language/types.md) | Stub | Yes |
| [language/arrays.md](language/arrays.md) | Stub | Yes |
| [language/built-in-functions.md](language/built-in-functions.md) | Yes | Yes |
| [language/source-model.md](language/source-model.md) | Yes | Yes |
| [language/syntax-kinds-and-facts.md](language/syntax-kinds-and-facts.md) | Yes | Yes |
| [language/tokens.md](language/tokens.md) | Yes | Yes |
| [language/lexer.md](language/lexer.md) | Yes | Yes |
| [language/parser.md](language/parser.md) | Yes | Yes |
| [language/syntax-tree.md](language/syntax-tree.md) | Yes | Yes |
| [language/public-api.md](language/public-api.md) | Yes | Yes |
| [language/project-layout.md](language/project-layout.md) | Yes | Yes |

## Semantics (`RoboSharp.Semantics`)

Index: [semantics/README.md](semantics/README.md).

| Document | Have content | Implemented |
| -------- | ------------ | ----------- |
| [semantics/README.md](semantics/README.md) | Yes | No |
| [semantics/overview.md](semantics/overview.md) | Yes | No |
| [semantics/type-system.md](semantics/type-system.md) | Yes | No |
| [semantics/symbols-and-scopes.md](semantics/symbols-and-scopes.md) | Yes | No |
| [semantics/builtins-and-profiles.md](semantics/builtins-and-profiles.md) | Yes | No |
| [semantics/binding-and-bound-tree.md](semantics/binding-and-bound-tree.md) | Stub | No |
| [semantics/conversions.md](semantics/conversions.md) | Stub | No |
| [semantics/control-flow-and-conditions.md](semantics/control-flow-and-conditions.md) | Stub | No |
| [semantics/arrays.md](semantics/arrays.md) | Stub | No |
| [semantics/operators.md](semantics/operators.md) | Yes | No |
| [semantics/diagnostics.md](semantics/diagnostics.md) | Yes | No |
| [semantics/semantic-model-output.md](semantics/semantic-model-output.md) | Stub | No |
| [semantics/public-api.md](semantics/public-api.md) | Stub | No |
| [semantics/project-layout.md](semantics/project-layout.md) | Yes | No |
| [semantics/summary.md](semantics/summary.md) | Yes | No |

## Compiler (cross-layer pipeline)

| Document | Have content | Implemented |
| -------- | ------------ | ----------- |
| [compiler/compilation-pipeline.md](compiler/compilation-pipeline.md) | Yes | Partial |
| [compiler/v1-compiler-spec.md](compiler/v1-compiler-spec.md) | Yes | Partial |
| [compiler/syntax-to-il-lowering.md](compiler/syntax-to-il-lowering.md) | Yes | No |
| [compiler/lexical-analysis.md](compiler/lexical-analysis.md) | Stub | Yes |
| [compiler/parsing.md](compiler/parsing.md) | Stub | Yes |
| [compiler/syntax-tree.md](compiler/syntax-tree.md) | Stub | Yes |
| [compiler/semantic-analysis.md](compiler/semantic-analysis.md) | Stub | No |
| [compiler/diagnostics.md](compiler/diagnostics.md) | No | Partial |
| [compiler/il-generation.md](compiler/il-generation.md) | No | Partial |

## Runtime (`RoboSharp.Runtime`)

| Document | Have content | Implemented |
| -------- | ------------ | ----------- |
| [runtime/il-instruction-set.md](runtime/il-instruction-set.md) | Yes | Partial |
| [runtime/v1-runtime-spec.md](runtime/v1-runtime-spec.md) | Yes | Partial |
| [runtime/interpreter.md](runtime/interpreter.md) | No | No |
| [runtime/execution-model.md](runtime/execution-model.md) | No | No |
| [runtime/runtime-state.md](runtime/runtime-state.md) | No | No |
| [runtime/error-handling.md](runtime/error-handling.md) | No | No |
| [runtime/standard-output.md](runtime/standard-output.md) | No | No |

## World (`RoboSharp.World`)

Index: [world/README.md](world/README.md).

| Document | Have content | Implemented |
| -------- | ------------ | ----------- |
| [world/README.md](world/README.md) | Stub | Partial |
| [world/world-model.md](world/world-model.md) | Yes | Partial |
| [world/terrain-grid.md](world/terrain-grid.md) | Yes | Partial |
| [world/item-grid.md](world/item-grid.md) | Yes | Partial |
| [world/actor-grid.md](world/actor-grid.md) | Yes | Partial |
| [world/world-actions.md](world/world-actions.md) | Yes | Partial |
| [world/movement-rules.md](world/movement-rules.md) | Yes | Partial |
| [world/metrics-and-analysis.md](world/metrics-and-analysis.md) | Yes | No |

## Rendering (adapters over world snapshots)

Index: [rendering/README.md](rendering/README.md).

| Document | Have content | Implemented |
| -------- | ------------ | ----------- |
| [rendering/README.md](rendering/README.md) | Stub | No |
| [rendering/render-projection.md](rendering/render-projection.md) | Yes | No |
| [rendering/ascii-renderer.md](rendering/ascii-renderer.md) | Stub | No |
| [rendering/sprite-renderer.md](rendering/sprite-renderer.md) | Stub | No |

## Toolchain (`RoboSharp.Toolchain`)

| Document | Have content | Implemented |
| -------- | ------------ | ----------- |
| [toolchain/project-format.md](toolchain/project-format.md) | No | No |
| [toolchain/roboexe-format.md](toolchain/roboexe-format.md) | No | No |
| [toolchain/build-process.md](toolchain/build-process.md) | No | Partial |
| [toolchain/v1-toolchain-spec.md](toolchain/v1-toolchain-spec.md) | Yes | Partial |
| [toolchain/artifact-layout.md](toolchain/artifact-layout.md) | No | No |

## Debugger

| Document | Have content | Implemented |
| -------- | ------------ | ----------- |
| [debugger/debugger-architecture.md](debugger/debugger-architecture.md) | Yes | No |
| [debugger/breakpoints.md](debugger/breakpoints.md) | Stub | No |
| [debugger/stepping.md](debugger/stepping.md) | No | No |
| [debugger/state-inspection.md](debugger/state-inspection.md) | No | No |
| [debugger/metrics-view.md](debugger/metrics-view.md) | No | No |

## Studio (`RoboSharp.Studio`)

Split from the former `general-specs.md`; some pages are stubs.

| Document | Have content | Implemented |
| -------- | ------------ | ----------- |
| [studio/README.md](studio/README.md) | Yes | Partial |
| [studio/overview.md](studio/overview.md) | Yes | Partial |
| [studio/technology-stack.md](studio/technology-stack.md) | Yes | Partial |
| [studio/avalonia-mcp.md](studio/avalonia-mcp.md) | Yes | N/A |
| [studio/referenced-solution-shape.md](studio/referenced-solution-shape.md) | Yes | Partial |
| [studio/composition-and-domain.md](studio/composition-and-domain.md) | Yes | Partial |
| [studio/ide-layout.md](studio/ide-layout.md) | Yes | Partial |
| [studio/workspace-integration.md](studio/workspace-integration.md) | Yes | Partial |
| [studio/editor-behavior.md](studio/editor-behavior.md) | Yes | Partial |
| [studio/build-and-analysis.md](studio/build-and-analysis.md) | Yes | Partial |
| [studio/inspection-panels.md](studio/inspection-panels.md) | Yes | Partial |
| [studio/visualization.md](studio/visualization.md) | Yes | Partial |
| [studio/output-and-state-panels.md](studio/output-and-state-panels.md) | Yes | Partial |
| [studio/lesson-profiles.md](studio/lesson-profiles.md) | Yes | Partial |
| [studio/menus-and-commands.md](studio/menus-and-commands.md) | Yes | Partial |
| [studio/settings.md](studio/settings.md) | Yes | Partial |
| [studio/theming.md](studio/theming.md) | Stub | Partial |
| [studio/syntax-highlighting.md](studio/syntax-highlighting.md) | Stub | Partial |
| [studio/extensibility.md](studio/extensibility.md) | Stub | Partial |
| [studio/project-modules.md](studio/project-modules.md) | Stub | Partial |
| [studio/testing-strategy.md](studio/testing-strategy.md) | Stub | Partial |
| [studio/performance.md](studio/performance.md) | Stub | Partial |
| [studio/scope-mvp-and-non-goals.md](studio/scope-mvp-and-non-goals.md) | Yes | Partial |
| [studio/general-specs.md](studio/general-specs.md) | Stub | Partial |

## Missing doc trees (in `AGENTS.md` / layout, not under `docs/` yet)

| Topic | Have content | Implemented |
| ----- | ------------ | ----------- |
| `RoboSharp.Application` — run/debug facades, host-facing orchestration | No (no `docs/application/`) | No |
| `RoboSharp.Hosting` — composition helpers | No | No |
| `RoboSharp.Web` — Blazor host | No | Partial |

Use [repository-layout.md](repository-layout.md) and [AGENTS.md](../AGENTS.md) until dedicated pages exist.
