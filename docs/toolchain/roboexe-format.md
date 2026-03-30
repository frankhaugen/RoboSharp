# `.roboexe` format (compiled executable)

The **compiled artifact** for RoboSharp programs is `.roboexe`. v1 interchange in the repository is **JSON** text: human-readable, diff-friendly, and easy to inspect in teaching UIs.

## In-memory model

`RoboExecutable` wraps `RoboProgram`:

- `FormatVersion` (constant `CurrentFormatVersion` in code)
- `Program` — entry function index, string/number tables, compiled functions with `Instruction` lists (`RoboOpcode` + operands)

See `RoboSharp.IL` types for the authoritative shape.

## On-disk v1 (JSON)

`RoboExecutableJsonSerializer` in `RoboSharp.Toolchain` serializes to camelCase JSON:

- Top-level `formatVersion`, `entryFunctionIndex`, `stringTable`, `numberTable`, `functions[]`
- Each function: `name`, `parameterCount`, `localSlotCount`, `returnsVoid`, `instructions[]` with `op` as opcode name string and operand fields `a`, `b`, …

**Binary layout** is not frozen; any future compact encoding must keep a documented mapping to the same logical model.

## Load-time validation

Runtimes and Player must validate before execution ([v1-runtime-spec.md](../runtime/v1-runtime-spec.md) §3):

- Format version
- Entry function and instruction indices in range
- Builtin ids / profile compatibility (when enforced)
- World kind compatibility (when enforced)

## Related

- [artifact-layout.md](artifact-layout.md)
- [build-process.md](build-process.md)
- [v1-toolchain-spec.md](v1-toolchain-spec.md)
- [../runtime/v1-runtime-spec.md](../runtime/v1-runtime-spec.md)
