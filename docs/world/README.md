# RoboSharp.World documentation

Simulation and analysis for the grid world: terrain, items, actors, movement, snapshots, and derived metrics. **No** parsing, binding, IL dispatch, or UI rendering here—rendering is an adapter over snapshots ([`../rendering/`](../rendering/README.md)).

Authoritative architecture rules: [`AGENTS.md`](../../AGENTS.md).

| Topic | Document |
| ----- | -------- |
| Purpose, `RobotWorld`, coordinates, metadata, snapshots, file-format direction | [world-model.md](world-model.md) |
| Terrain layer | [terrain-grid.md](terrain-grid.md) |
| Item layer | [item-grid.md](item-grid.md) |
| Actor grid, `ActorState`, id indirection | [actor-grid.md](actor-grid.md) |
| `IRobotWorld` and action results | [world-actions.md](world-actions.md) |
| Walkability, push, pickup, sensing, direction math | [movement-rules.md](movement-rules.md) |
| Derived analysis and actor telemetry | [metrics-and-analysis.md](metrics-and-analysis.md) |
