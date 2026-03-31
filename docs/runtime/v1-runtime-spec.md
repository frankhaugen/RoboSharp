# RoboSharp v1 runtime specification

Normative runtime contract: load, execution state, stepping, faults, snapshots, and host interfaces.

**Implementation (partial):** `RoboInterpreter` (run-to-end), `RoboInterpreterSession` / `RoboInterpreterEngine` (single shared evaluation stack across frames; not yet per-frame stacks as in §5 below). Structured faults: `RuntimeFault`. JSON load of executables: `RoboExecutableJsonSerializer` in Toolchain.

**Related:** [il-instruction-set](il-instruction-set.md), [interpreter](interpreter.md), [execution-model](execution-model.md), [pipeline boundaries](../architecture/pipeline-boundaries.md).

---

## 1. Purpose

The runtime loads a compiled `RoboExecutable` and executes it through a plain C# interpreter loop.

It is:

* deterministic
* stepable
* snapshot-friendly
* result-driven, not exception-driven

That direction is already established in the runtime and execution-model docs.  

---

## 2. Runtime load pipeline

```text
Load .roboexe
→ Validate manifest
→ Validate builtin/world compatibility
→ Build initial ExecutionState
→ Push entry frame
→ Run Step / Continue loop
→ Emit snapshots / stdout / stderr
```

---

## 3. Executable validation rules

Before execution, runtime must validate:

* executable format/version
* language version
* entry function exists
* function entry indices are valid
* instruction indices are valid
* builtin ids are known
* required builtin profile is satisfied
* required world kind is satisfied

This matches the earlier `.roboexe` contract. 

---

## 4. Execution state

The interpreter operates on a single `ExecutionState` containing:

* instruction pointer
* call stack
* heap
* world
* statistics
* stdout/stderr log

This matches the established runtime model.  

---

## 5. Frame layout

Execution frames are slot-based, not symbol-based.

```csharp
public sealed class CallFrame
{
    public required int FunctionId { get; init; }
    public required int ReturnAddress { get; init; }
    public required Value[] Locals { get; init; }
    public required Stack<Value> EvaluationStack { get; init; }
}
```

Rules:

* one evaluation stack per frame
* locals array length equals compiled `LocalCount`
* parameters are copied into the first local slots on call

This follows the stronger runtime execution design already proposed. 

---

## 6. Initial execution state

On start:

* instruction pointer = entry function entry instruction
* heap empty
* statistics zeroed
* stdout/stderr empty
* call stack contains one frame for the **entry** function (the lowered top-level program body, implementation name `TopLevel`)

That entry frame has:

* zero parameters
* locals array sized to compiled local count
* empty evaluation stack
* return address unused

---

## 7. Step semantics

Each `Step()` does exactly one instruction.

The step sequence is:

1. validate instruction pointer
2. fetch current instruction
3. execute opcode handler
4. mutate state
5. update instruction pointer unless changed by control flow
6. increment statistics
7. return `StepResult`

This matches the instruction-first execution model already defined for the debugger/runtime. 

---

## 8. Execution statuses

```csharp
public enum ExecutionStatus
{
    Running,
    Completed,
    Faulted
}
```

Runtime faults are returned as structured data, not thrown exceptions. That is already a core design rule. 

---

## 9. Opcode execution contract

Each opcode handler must:

* validate required stack shape
* validate operand types if necessary
* perform state change
* return `StepResult`

### Examples

#### `PushConstant`

Push constant value onto current frame stack.

#### `LoadLocal`

Push local slot value.

#### `StoreLocal`

Pop top stack value into local slot.

#### `Add`

Pop two numeric values, push result.

#### `Jump`

Set instruction pointer to target.

#### `JumpIfFalse`

Pop bool; jump if false, otherwise continue.

#### `Call`

Pop arguments, create new frame, jump to callee entry.

#### `CallBuiltin`

Invoke builtin runtime handler.

#### `Return`

Pop optional return value, pop frame, either:

* push return value to caller and resume caller
* or complete execution if returning from entry frame

#### `NewArray`

Allocate new heap array, push array reference.

#### `ArrayGet`

Pop index and array ref, push element.

#### `ArraySet`

Pop value, index, array ref, assign.

#### `ArrayAdd`

Pop value and array ref, append.

#### `ArrayLength`

Pop array ref, push current count.

#### `ArrayGetLast`

Pop array ref, push last element.

#### `ArrayTakeLast`

Pop array ref, remove and push last element.

The exact opcode set is consistent with the existing IL/file-format direction. 

---

## 10. Builtin invocation contract

Built-ins are invoked only through `CallBuiltin`.

Built-ins may:

* read or mutate world state
* read or mutate output streams
* return a value
* emit stderr warnings/faults

Built-ins must never throw for expected user-program failures.

They return structured results instead. That is already established in the C# execution model. 

---

## 11. Runtime fault taxonomy

Minimum v1 fault codes:

* `InvalidInstruction`
* `InvalidFunction`
* `InvalidBuiltin`
* `InvalidLocalSlot`
* `StackUnderflow`
* `InvalidArrayReference`
* `ArrayIndexOutOfBounds`
* `EmptyArray`
* `StepLimitExceeded`

World-related issues that are considered non-fatal lesson/runtime messages may go to stderr instead of faulting, depending on builtin policy.

Example policy:

* blocked `move()` → stderr, continue
* invalid array access → stderr + fault

This matches the output-system direction. 

---

## 12. Stdout / stderr contract

The runtime contains two structured output streams:

* `StdOut` = program output
* `StdErr` = runtime messages / warnings / diagnostics

Each output line records instruction pointer metadata. This was already explicitly specified. 

---

## 13. Snapshot contract

After each pause-worthy execution point, the runtime may capture a `RuntimeSnapshot`.

Minimum snapshot contains:

* instruction pointer
* current instruction text
* stack frames
* locals
* heap arrays
* world snapshot
* statistics
* stdout
* stderr
* fault if any

Snapshots are immutable copies for UI/debugging, not live state views. That is already a core design rule. 

---

## 14. Run-to-end contract

`RunToEnd` repeatedly calls `Step` until one of:

* completed
* faulted
* step limit exceeded

`StepLimitExceeded` should be a real dedicated fault code in v1.

---

## 15. Determinism rules

Runtime must not use:

* threads
* timers
* random behavior unless explicitly modeled
* external IO during execution

This rule already exists in the runtime/debugger direction and should be hardened.  

---

## 16. Runtime interfaces

```csharp
public interface IInterpreter
{
    StepResult Step(ExecutionState state, RoboExecutable executable);
    RunResult RunToEnd(ExecutionState state, RoboExecutable executable, int maxSteps);
    RuntimeSnapshot CaptureSnapshot(ExecutionState state, RoboExecutable executable);
}
```

```csharp
public interface IRuntimeHost
{
    ValueTask<RunResult> RunAsync(
        RoboExecutable executable,
        RuntimeLaunchOptions options,
        CancellationToken cancellationToken = default);
}
```

