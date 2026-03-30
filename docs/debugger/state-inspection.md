# State inspection (snapshots)

When execution is **paused**, the host presents a **read-only snapshot** of runtime state so learners can correlate source, IL, data, and world without racing the interpreter.

## Minimum snapshot content

Aligned with [../runtime/v1-runtime-spec.md](../runtime/v1-runtime-spec.md) §13, a pause snapshot should allow the UI to show:

- Current instruction pointer (function index + instruction index) and a human-readable disassembly line
- Call stack (outer → inner frames)
- Per-frame locals (slot values with optional names from debug symbols)
- Heap-allocated arrays (identity, elements, length)
- World snapshot (terrain, items, actor pose as defined by `RoboSharp.World`)
- Cumulative statistics (step count, etc., if tracked)
- Stdout and stderr buffers as structured lines
- Active fault, if execution stopped on fault

## Immutability

Snapshots are **copies** for display. The UI must not mutate live interpreter state through a snapshot object. Refreshing the view means capturing a new snapshot after the next step or continue.

## Source and IL correlation

When `.robo.pdb.json` (or embedded equivalent) is present, map current IP to source spans for highlighting. Without symbols, fall back to IL-only inspection ([../toolchain/v1-toolchain-spec.md](../toolchain/v1-toolchain-spec.md) §9.2).

## Related

- [debugger-architecture.md](debugger-architecture.md)
- [stepping.md](stepping.md)
- [../studio/inspection-panels.md](../studio/inspection-panels.md)
- [../runtime/runtime-state.md](../runtime/runtime-state.md)
