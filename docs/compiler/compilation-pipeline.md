# Compilation pipeline (teaching)

RoboSharp’s **compiler** is the stretch from **source text** to **fake IL** ready for the interpreter. It is split across projects and docs so each stage stays visible.

## Stages

```text
SourceText
  → Lexer / parser     → Syntax tree + parse diagnostics     (RoboSharp.Language)
  → Semantic analysis  → Bound tree + semantic diagnostics     (RoboSharp.Semantics)
  → IL generation      → Fake executable / IL blobs             (RoboSharp.IL)
```

Orchestration (open project, diagnostics aggregation, emit paths) lives in **`RoboSharp.Toolchain`** and host-facing flows in **`RoboSharp.Application`**—see [Architecture overview](../architecture.md).

## Bridge documents

| Topic | Document |
| ----- | -------- |
| **Normative v1 compiler contract** (phases, lowering model, artifacts, interfaces) | [v1-compiler-spec.md](v1-compiler-spec.md) |
| Lex / parse / syntax tree (host view) | [lexical-analysis.md](lexical-analysis.md), [parsing.md](parsing.md), [syntax-tree.md](syntax-tree.md) |
| Binding and diagnostics | [semantic-analysis.md](semantic-analysis.md), [diagnostics.md](diagnostics.md) |
| **Syntax → IL shape (examples)** | [syntax-to-il-lowering.md](syntax-to-il-lowering.md) |
| Opcode / operand model | [../runtime/il-instruction-set.md](../runtime/il-instruction-set.md) |
| Layer ownership (syntax vs semantics vs IL vs world) | [../architecture/pipeline-boundaries.md](../architecture/pipeline-boundaries.md) |

## Teaching note

Students should be able to open **each** artifact (tokens, tree, bound nodes, IL listing) in Studio or similar hosts without those concerns being merged into one opaque “compile” step.
