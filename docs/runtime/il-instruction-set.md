# IL instruction set (fake executable)

RoboSharp IL is a **teaching** instruction set: compact, inspectable, executed by a **C# interpreter** (not emitted to CLR IL). This page captures the **intended inventory and execution shape**; exact encodings and metadata formats belong in `RoboSharp.IL` when implemented.

## What the IL spec must freeze

- Opcode set and operand model
- Stack vs local slot behavior (evaluation stack + locals per frame)
- Function calling convention (arguments, return)
- Array operations
- Control flow (`Jump`, `JumpIfFalse`, loops)
- **`CallBuiltin`** — operand identifies built-in; dispatch table at runtime
- Debug / source mapping expectations (line/span hooks for stepping)

## Execution model (implied)

- Instruction pointer
- Call stack (frames)
- Heap (e.g. arrays)
- Attachment to **robot world** and stdout/stderr sinks
- Optional **statistics** / metrics for teaching

## Opcode shape (directional)

The set is intentionally small, along the lines of:

- `PushConstant`
- `LoadLocal` / `StoreLocal`
- Arithmetic / comparison opcodes
- `Jump`, `JumpIfFalse`
- `Call` (user function)
- `CallBuiltin`
- `Return`
- Array: `NewArray`, `ArrayGet`, `ArraySet` (names illustrative)

Final names and operand layouts are **TBD** until the IL project is implemented; lowering examples live in [syntax-to-IL lowering](../compiler/syntax-to-il-lowering.md).

## Boundaries

- IL does **not** render or read syntax trees directly (except via debug metadata).
- World side effects occur through **runtime dispatch** (e.g. built-in handlers), not from the IL file format alone.

See [Pipeline boundaries](../architecture/pipeline-boundaries.md).

## Related

- [Interpreter](interpreter.md)
- [Execution model](execution-model.md)
- [Syntax-to-IL lowering](../compiler/syntax-to-il-lowering.md)
