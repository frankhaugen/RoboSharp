# Output and runtime state panels

## Output system

The Studio must visually separate:

- **Program Output** (`stdout`)
- **Runtime Messages** (`stderr`)

That matches the RoboSharp runtime model.

### stdout pane

For `print(...)` and intentional program output.

### stderr pane

For runtime warnings/fault messages, such as:

- divide by zero fallback
- blocked movement
- out-of-bounds array access
- empty-array operation

### Output timeline behavior

Each output line should retain instruction-pointer metadata. The Studio can then optionally show output “since last step.”

## Metrics / runtime state panels

The metrics view should stay didactic rather than profiler-like.

### Call Stack

Current frames, current frame highlighted.

### Locals

Current frame locals and values.

### Arrays / Heap

Array id, type, contents, references.

### Runtime statistics

- instructions executed
- arrays allocated
- current stack depth
- max stack depth

### Robot state

- position
- direction
- inventory
- world interaction values

### Route / lesson metrics

- visited tiles
- repeated tiles
- failed move attempts
- turn count
- shortest path comparison later

See [../runtime/standard-output.md](../runtime/standard-output.md) and [../debugger/metrics-view.md](../debugger/metrics-view.md).
