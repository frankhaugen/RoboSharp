# Interpreter

The **interpreter** executes `RoboProgram` (from `RoboExecutable`) as a plain C# loop over fake IL. It does not JIT to CLR IL or use reflection for user code.

## Public entry points

- **`RoboInterpreter`:** Run-to-completion API over a program, world, and `TextWriter` targets for stdout/stderr.
- **`RoboInterpreterSession`:** Holds engine state for stepping / sessions (used when hosts need pause boundaries).

Shared execution logic lives in `RoboInterpreterEngine` (internal): initialize, then `ExecuteNext` for one instruction.

## Contract (summary)

Matches [v1-runtime-spec.md](v1-runtime-spec.md):

- **Deterministic:** No threads, timers, or hidden randomness in v1.
- **Result-driven:** Normal failures are `RuntimeFault` / `ExecutionResult`, not thrown exceptions for user-level mistakes (some defensive `InvalidOperationException` paths may still map to faults in the engine).

## One step

Each `ExecuteNext`:

1. Bounds-check the current instruction pointer.
2. Fetch the current `Instruction`.
3. Dispatch on `RoboOpcode`.
4. Mutate evaluation stack, locals, heap arrays, world, or writers as defined for that opcode ([il-instruction-set.md](il-instruction-set.md)).
5. Return `null` to continue, `ExecutionResult.Completed` when the call stack is drained, or `ExecutionResult.Failed` on fault.

## Implementation vs v1 spec

The v1 spec describes **one evaluation stack per call frame**. The current engine uses a **single shared stack** with frame metadata; behavior remains stepable and teaching-friendly, but deep stack inspection UIs should be aware of the difference when showing “operand stack per frame.”

## Related

- [v1-runtime-spec.md](v1-runtime-spec.md)
- [execution-model.md](execution-model.md)
- [il-instruction-set.md](il-instruction-set.md)
- [../debugger/stepping.md](../debugger/stepping.md)
