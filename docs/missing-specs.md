# Areas not yet fully specified

This page lists **product or documentation gaps** where `docs/` or `AGENTS.md` imply work, but there is **no complete, implementable spec** yet (or only stubs). It pairs with [implementation-gaps.md](implementation-gaps.md), which tracks **code vs intent**.

**Normative v1 drafts** for compiler / runtime / toolchain: [compiler/v1-compiler-spec.md](compiler/v1-compiler-spec.md), [runtime/v1-runtime-spec.md](runtime/v1-runtime-spec.md), [toolchain/v1-toolchain-spec.md](toolchain/v1-toolchain-spec.md).

**Baseline prose** now exists for toolchain flow ([toolchain/project-format.md](toolchain/project-format.md), [toolchain/build-process.md](toolchain/build-process.md), [toolchain/artifact-layout.md](toolchain/artifact-layout.md), [toolchain/roboexe-format.md](toolchain/roboexe-format.md)), compiler IL/diagnostics ([compiler/il-generation.md](compiler/il-generation.md), [compiler/diagnostics.md](compiler/diagnostics.md)), runtime interpreter contract ([runtime/interpreter.md](runtime/interpreter.md) and siblings), debugger stepping/inspection ([debugger/stepping.md](debugger/stepping.md), [debugger/state-inspection.md](debugger/state-inspection.md), [debugger/metrics-view.md](debugger/metrics-view.md)), architecture pointers ([architecture/solution-structure.md](architecture/solution-structure.md), [architecture/dependency-rules.md](architecture/dependency-rules.md), [architecture/dependency-injection.md](architecture/dependency-injection.md), [architecture/runtime-hosts.md](architecture/runtime-hosts.md)), and governance summaries ([governance/dependency-policy.md](governance/dependency-policy.md), [governance/implementation-order.md](governance/implementation-order.md)). Remaining gaps below are **deeper** than those index pages.

## Pipeline and artifacts

| Topic | Gap |
| ----- | --- |
| **`.robo` project file schema** | [toolchain/project-format.md](toolchain/project-format.md) describes fields; **exact on-disk schema and version negotiation** are not frozen in a parser implementation. |
| **Binary `.roboexe`** | [toolchain/roboexe-format.md](toolchain/roboexe-format.md) documents v1 JSON; **compact binary** layout and validation are not specified. |
| **Workspace runtime acceptance tests** | [workspaces/](workspaces/README.md) contracts exist, but there is no single spec with **acceptance criteria** for “load project → resolve documents → emit artifacts” end-to-end. |

## Semantics and compiler (documentation depth)

| Topic | Gap |
| ----- | --- |
| **Bound tree IR** | [semantics/binding-and-bound-tree.md](semantics/binding-and-bound-tree.md) is still a stub; full public shape of bound nodes is not specified. |
| **Semantic edge cases** | Conversions, control-flow analysis, array rules mix full pages with stubs; no single **v1 binder rejection checklist**. |

## Runtime (documentation depth)

| Topic | Gap |
| ----- | --- |
| **Spec vs engine** | [runtime/interpreter.md](runtime/interpreter.md) notes **shared vs per-frame stack**; resolving this in code and updating [v1-runtime-spec.md](runtime/v1-runtime-spec.md) is still open. |

## World and data

| Topic | Gap |
| ----- | --- |
| **World file loading** | [world/world-model.md](world/world-model.md) sketches JSON; no validated schema, loader seam (IO vs world), or unified error model. |
| **Push / advanced movement** | [world/movement-rules.md](world/movement-rules.md) specifies push; current `RoboSharp.World` implements v1 walkability **without** push. |
| **Metrics and analysis** | [world/metrics-and-analysis.md](world/metrics-and-analysis.md) is not fully tied to concrete public APIs in code. |

## Lessons, profiles, hosts

| Topic | Gap |
| ----- | --- |
| **Lesson runtime** | [lessons/](lessons/README.md) specifies JSON direction; no in-repo **lesson engine** beyond minimal profile providers. |
| **Application layer** | [AGENTS.md](../AGENTS.md) names `RoboSharp.Application`; there is no `docs/application/` index yet. |
| **Player / Web** | [player/README.md](player/README.md) and Blazor intent are thinner than [studio/](studio/README.md). |

## Related

- [Implementation gaps vs `src/`](implementation-gaps.md)
- [Documentation checklist](documentation-todo.md)
- [AGENTS.md](../AGENTS.md)
