# Error handling (runtime)

RoboSharp distinguishes **structured faults** (stop execution, show in debugger) from **soft messages** (stderr, may continue) according to policy.

## Runtime faults

`RuntimeFault` (conceptually) carries:

- Human-readable message
- Function index and instruction index for the fault site

Taxonomy targets are listed in [v1-runtime-spec.md](v1-runtime-spec.md) §11 (`StackUnderflow`, `InvalidArrayReference`, `ArrayIndexOutOfBounds`, `StepLimitExceeded`, etc.).

Faults are returned inside `ExecutionResult.Failed` (or equivalent); they are not the normal way to report “move blocked” if the lesson treats that as a stderr warning.

## Exceptions inside the engine

The C# implementation may catch internal errors (e.g. `InvalidOperationException` from stack helpers) and convert them to a `RuntimeFault` so the host still gets a **result** object. This is infrastructure-only; user programs are not modeled as throwing.

## Builtin policy

Builtins should map expected failures (invalid args, blocked move) to stderr or fault per profile rules ([v1-runtime-spec.md](v1-runtime-spec.md) §10–11). World rules stay explicit and inspectable.

## Step limit

`RunToEnd` with a **max step** count prevents infinite loops from hanging the host. Exceeding the limit is a dedicated fault ([v1-runtime-spec.md](v1-runtime-spec.md) §14).

## Related

- [standard-output.md](standard-output.md)
- [interpreter.md](interpreter.md)
- [../compiler/diagnostics.md](../compiler/diagnostics.md) — compile-time vs runtime
