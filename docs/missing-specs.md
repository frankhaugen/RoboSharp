# Areas not yet fully specified

This page lists **product or documentation gaps** where `docs/` or `AGENTS.md` imply work, but there is **no complete, implementable spec** yet (or only stubs). It pairs with [implementation-gaps.md](implementation-gaps.md), which tracks **code vs intent**.

**Normative v1 drafts** for compiler / runtime / toolchain **do** exist as focused pages: [compiler/v1-compiler-spec.md](compiler/v1-compiler-spec.md), [runtime/v1-runtime-spec.md](runtime/v1-runtime-spec.md), [toolchain/v1-toolchain-spec.md](toolchain/v1-toolchain-spec.md). This file is for what is **still** underspecified *after* those documents.

## Pipeline and artifacts

| Topic | Gap |
| ----- | --- |
| **`.roboexe` format** | [toolchain/roboexe-format.md](toolchain/roboexe-format.md) is still thin; v1 **JSON** interchange exists in code (`RoboExecutableJsonSerializer`); binary layout and manifest validation are not frozen. |
| **Project / build integration** | [toolchain/build-process.md](toolchain/build-process.md), [toolchain/project-format.md](toolchain/project-format.md), [toolchain/artifact-layout.md](toolchain/artifact-layout.md) lack end-to-end prose tying `.robosharp` → compile → `bin`/`obj`. |
| **Workspace runtime** | [workspaces/](workspaces/README.md) contracts exist, but there is no single “load project, resolve documents, emit artifacts” spec with acceptance criteria. |

## Semantics and compiler (documentation depth)

| Topic | Gap |
| ----- | --- |
| **Bound tree IR** | [semantics/binding-and-bound-tree.md](semantics/binding-and-bound-tree.md) is a stub; public shape of bound nodes beyond the current binder is not specified. |
| **IL generation doc** | [compiler/il-generation.md](compiler/il-generation.md) is empty; lowering is partly covered in [compiler/syntax-to-il-lowering.md](compiler/syntax-to-il-lowering.md) and code (`IlLowerer`). |
| **Semantic edge cases** | Conversions, control-flow analysis, and array rules in [semantics/](semantics/README.md) mix “Yes” pages with stubs; no single v1 checklist for what the binder must reject. |

## Runtime (documentation depth)

| Topic | Gap |
| ----- | --- |
| **Interpreter contract** | [runtime/interpreter.md](runtime/interpreter.md), [runtime/execution-model.md](runtime/execution-model.md), [runtime/runtime-state.md](runtime/runtime-state.md), [runtime/error-handling.md](runtime/error-handling.md), [runtime/standard-output.md](runtime/standard-output.md) are empty or thin; behavior is only partly described in [runtime/il-instruction-set.md](runtime/il-instruction-set.md) and `RoboInterpreter`. |
| **Stepping / snapshots for debug** | No frozen contract for instruction-level snapshots beyond general intent in [debugger/debugger-architecture.md](debugger/debugger-architecture.md). |

## World and data

| Topic | Gap |
| ----- | --- |
| **World file loading** | [world/world-model.md](world/world-model.md) sketches JSON; no validated schema, loader location (IO vs world), or error model. |
| **Push / advanced movement** | [world/movement-rules.md](world/movement-rules.md) specifies push; current `RoboSharp.World` implements v1 walkability without push. |
| **Metrics and analysis** | [world/metrics-and-analysis.md](world/metrics-and-analysis.md) is not tied to concrete APIs in code. |

## Lessons, profiles, hosts

| Topic | Gap |
| ----- | --- |
| **Lesson runtime** | [lessons/](lessons/README.md) specifies JSON direction and models; no in-repo lesson engine or profile provider beyond `FullBuiltinProfileProvider`. |
| **Application layer** | [AGENTS.md](../AGENTS.md) names `RoboSharp.Application`; there is no `docs/application/` index for run/debug facades. |
| **Hosting** | Composition roots for multi-host DI are not documented beyond [architecture/dependency-injection.md](architecture/dependency-injection.md) (empty). |
| **Player / Web** | [player/README.md](player/README.md) and Blazor intent are shells; no UI or session spec at the same depth as Studio. |
| **Studio debugger** | [studio/](studio/README.md) panels exist; [debugger/stepping.md](debugger/stepping.md), [debugger/state-inspection.md](debugger/state-inspection.md), [debugger/metrics-view.md](debugger/metrics-view.md) are placeholders. |

## Governance stubs

| Document | Status |
| -------- | ------ |
| [governance/dependency-policy.md](governance/dependency-policy.md) | Empty / pointer only |
| [governance/implementation-order.md](governance/implementation-order.md) | Empty / pointer only |

## Related

- [Implementation gaps vs `src/`](implementation-gaps.md)
- [Documentation checklist](documentation-todo.md)
- [AGENTS.md](../AGENTS.md)
