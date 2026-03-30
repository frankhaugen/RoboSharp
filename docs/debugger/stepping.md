# Stepping (instruction-level debug)

Studio and Player debug sessions advance execution in **single-instruction** steps at the RoboSharp IL level. That matches the runtime contract in [../runtime/v1-runtime-spec.md](../runtime/v1-runtime-spec.md) (one `Step()` → one opcode).

## Granularity

- **Step Into:** Execute the next IL instruction. If that instruction is `Call` or `CallBuiltin`, the next pause is at the callee’s first instruction (or inside the builtin implementation boundary—hosts may treat builtins as atomic for teaching).
- **Step Over:** Run until the current stack frame would advance past the current IP without entering a **user** callee from this line, or until return from the current frame. v1 implementation may approximate this by stepping until IP/stack depth conditions match; exact policy should stay deterministic.
- **Step Out:** Run until the current frame returns to its caller (or program completes/faults).

## Continue, pause, stop, reset

- **Continue:** Repeated steps (or run-to-breakpoint) until breakpoint, fault, completion, or pause.
- **Pause:** Request cooperative break at next step boundary (no threads in v1 runtime; pause is trivially “before next instruction” once honored).
- **Stop:** End debug session; discard transient session state.
- **Reset:** Rebuild initial `ExecutionState` from loaded executable and world (see [debugger-architecture.md](debugger-architecture.md)).

## Alignment with specs

- Instruction pointer, frames, and stacks exposed in the UI should reflect the same model as [../runtime/execution-model.md](../runtime/execution-model.md) and snapshots in [../runtime/v1-runtime-spec.md](../runtime/v1-runtime-spec.md) §13.
- **Implementation note:** Current `RoboInterpreterEngine` uses a shared evaluation stack across frames; the v1 spec describes per-frame stacks as the normative target. Stepping UX should still be “one instruction at a time” regardless.

## Related

- [debugger-architecture.md](debugger-architecture.md)
- [breakpoints.md](breakpoints.md)
- [state-inspection.md](state-inspection.md)
- [../studio/output-and-state-panels.md](../studio/output-and-state-panels.md)
