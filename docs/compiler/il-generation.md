# IL generation (compiler phase)

This page situates **lowering and packaging** in the compiler: turning a valid bound program into `RoboProgram` / `RoboExecutable` artifacts. Normative detail lives in [v1-compiler-spec.md](v1-compiler-spec.md) (phases, slot rules, opcode patterns) and [syntax-to-il-lowering.md](syntax-to-il-lowering.md) (examples). Opcode names and operand layout are defined in [../runtime/il-instruction-set.md](../runtime/il-instruction-set.md).

## Role in the pipeline

IL generation runs **after** semantic analysis succeeds (no error-level diagnostics). It must not introduce new type judgments; it only emits instructions that match what binding already proved.

```text
BoundProgram
  → assign function ids, local slots, synthetic __main
  → emit Instruction stream per function
  → build constant / string / number tables
  → RoboProgram
  → wrap as RoboExecutable (format version)
```

See [compilation-pipeline.md](compilation-pipeline.md) for the full stage list.

## Responsibilities

- **Lowering:** Map bound statements and expressions to `RoboOpcode` sequences (branches, calls, builtins, arrays, locals).
- **Entry:** Ensure a single entry function index consistent with the toolchain (synthetic `__main` for top-level code).
- **Debug metadata:** When enabled, preserve instruction-to-source and local-slot-to-name mappings for `.robo.pdb.json` (see v1 compiler spec outputs).
- **Determinism:** Same bound input yields the same IL shape (no hidden nondeterminism).

## Implementation note

Today, lowering is implemented in `RoboSharp.IL` / `RoboSharp.Toolchain` via `IlLowerer` and related types, producing a `RoboProgram` wrapped by `RoboExecutable`. Literal pools and opcode spelling (`PushInt`, `PushBool`, etc.) follow `RoboOpcode` in code, which aligns conceptually with the v1 spec’s “push constant / load local” patterns.

## Related

- [v1-compiler-spec.md](v1-compiler-spec.md) — failure boundary, artifact list, interfaces
- [syntax-to-il-lowering.md](syntax-to-il-lowering.md) — teaching-oriented patterns
- [semantic-analysis.md](semantic-analysis.md) — what must be true before IL runs
- [../architecture/pipeline-boundaries.md](../architecture/pipeline-boundaries.md) — layer contracts
