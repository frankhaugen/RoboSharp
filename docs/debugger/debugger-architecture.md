# Debugger architecture (Studio integration)

The Studio integrates a **snapshot-based** debugger aligned with RoboSharp runtime design.

## Debug model

The Studio should support:

- Step Into
- Step Over
- Step Out
- Continue
- Pause
- Stop
- Reset
- breakpoints
- source + IL synchronization
- call stack / locals / arrays / world / stdout / stderr

## Debug command bar

Must expose, at minimum:

- Run
- Debug
- Pause
- Stop
- Reset
- Step Into
- Step Over
- Step Out

## Debug visual synchronization

When paused:

- source span highlighted
- IL instruction highlighted
- call stack updated
- locals pane updated
- heap pane updated
- world pane updated
- stdout/stderr panes updated
- route/metrics pane updated

See [breakpoints.md](breakpoints.md), [../studio/output-and-state-panels.md](../studio/output-and-state-panels.md), and [stepping.md](stepping.md).
