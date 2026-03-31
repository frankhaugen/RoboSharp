# Runtime state

**Runtime state** is everything the interpreter mutates while a program runs. Hosts either observe it live (during stepping) or copy it into **snapshots** for immutable UI ([v1-runtime-spec.md](v1-runtime-spec.md) §13).

## Logical components

| Area | Holds |
| ---- | ----- |
| **Program counter** | Current frame + IP |
| **Frames** | Locals, return linkage |
| **Evaluation stack** | Operand slots (`EvalSlot` in code: int, bool, double, string, array ref) |
| **Heap arrays** | Array id → integer elements (v1 teaching model) |
| **World** | `RobotWorld` — grids and actor state |
| **Output** | Stdout/stderr writers or buffers |
| **Statistics** | Step count (when tracked) |

## Initialization

On start ([v1-runtime-spec.md](v1-runtime-spec.md) §6):

- Validate executable/program (entry function, indices, builtins profile compatibility).
- Create the entry frame for the compiled entry function (lowered top-level statements), empty heap, cleared output, zeroed stats.

## Session boundary

A **session** binds state to one loaded executable + world + output sinks. Resetting debug clears transient state and re-initializes from the same or new inputs.

## Snapshots

For Studio, “state inspection” panes consume **snapshots**, not direct references to the mutable engine, so stepping does not race the UI ([../debugger/state-inspection.md](../debugger/state-inspection.md)).

## Related

- [execution-model.md](execution-model.md)
- [interpreter.md](interpreter.md)
- [../architecture/pipeline-boundaries.md](../architecture/pipeline-boundaries.md)
