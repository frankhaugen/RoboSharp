# Execution model

This document summarizes **how** RoboSharp IL runs: frames, instruction pointer, and evaluation stack, without restating every opcode (see [il-instruction-set.md](il-instruction-set.md)).

## Instruction-first

Execution is always positioned at a concrete `(functionIndex, instructionIndex)` inside a loaded `RoboProgram`. Control flow (`Jump`, `JumpIfFalse`, `Call`, `Return`) updates that position (and the call stack) according to opcode semantics.

## Call stack

Each activation has a **frame** holding at least:

- `FunctionIndex`
- Instruction pointer (`Ip`)
- `Locals` array sized to the compiled function’s local slot count

On `Call`, arguments are taken from the evaluation stack, a new frame is pushed, parameters are copied into the leading local slots, and execution continues at the callee entry.

On `Return`, the callee frame is popped; if there is a return value, it is pushed for the caller (unless void).

## Evaluation stack

Operands for arithmetic, calls, and builtins are pushed/popped according to each opcode’s stack effect. Stack underflow is a fault ([v1-runtime-spec.md](v1-runtime-spec.md) §11).

**Current implementation:** one physical stack shared across frames (`RoboInterpreterEngine`); **normative v1:** per-frame stacks. Hosts should treat “stack depth at pause” as an implementation detail unless/until the engine matches the per-frame model.

## World and builtins

Builtins run synchronously inside the opcode handler: they may read/write `RobotWorld` and write to stdout/stderr. They return structured outcomes; they must not throw for ordinary lesson failures ([v1-runtime-spec.md](v1-runtime-spec.md) §10).

## Run to end

Running to completion is repeated single steps until `Completed`, `Faulted`, or step limit exceeded ([v1-runtime-spec.md](v1-runtime-spec.md) §14).

## Related

- [interpreter.md](interpreter.md)
- [runtime-state.md](runtime-state.md)
- [error-handling.md](error-handling.md)
