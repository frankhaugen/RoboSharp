# World model

The World subsystem defines the in-memory environment that RoboSharp programs affect.

## Purpose

It is responsible for:

- board layout
- actor positions and facing
- items and obstacles
- movement legality
- pickup/push interactions
- world snapshots for UI/debugging
- derived analysis inputs such as walkability and route scoring

It is **not** responsible for:

- parsing `.robo`
- semantic analysis
- IL execution dispatch
- lesson gating
- UI rendering itself

The runtime mutates the world. The debugger inspects snapshots of the world. The renderer projects snapshots of the world.

## Design goals

The World layer must be:

- deterministic
- fully in-memory
- easy to snapshot
- easy to render in both ASCII and sprites
- easy to analyze for route quality
- future-safe for richer actor state
- decoupled from UI technology

The single-grid “everything is one cell type” model is rejected in favor of **three positional layers** (`TerrainGrid`, `ItemGrid`, `ActorGrid`).

## Core rule

The world is a **state model**, not a render model.

- The interpreter mutates `RobotWorld`.
- The UI never binds to live mutable world internals.
- The renderer consumes `RobotWorldSnapshot`.

## Canonical world shape (v1)

```csharp
public sealed class RobotWorld
{
    public required TerrainGrid Terrain { get; init; }
    public required ItemGrid Items { get; init; }
    public required ActorGrid Actors { get; init; }

    public required Dictionary<int, ActorState> ActorsById { get; init; }

    public required WorldMetadata Metadata { get; init; }
}
```

This gives:

- positional lookup via grids
- richer actor state via `ActorsById`
- straightforward rendering composition
- room for future multiplayer-style growth

## Coordinate system

Use one coordinate system everywhere.

```csharp
public readonly record struct GridPosition(int X, int Y);

public enum Direction
{
    North,
    East,
    South,
    West
}
```

Rules:

- `(0,0)` is top-left
- `X` increases to the right
- `Y` increases downward
- all three grids use identical dimensions
- all world APIs operate in the same coordinate space

## World metadata

Keep metadata separate from grid state.

```csharp
public sealed class WorldMetadata
{
    public required string Name { get; init; }
    public string? Description { get; init; }

    public string? LessonId { get; init; }
    public GridPosition? PrimaryGoalPosition { get; init; }

    public int Width { get; init; }
    public int Height { get; init; }

    public int? PrimaryActorId { get; init; }
}
```

v1 metadata should cover identity, dimensions, lesson/world linkage, default actor reference, and optional goal hints. Do not put mutable runtime counters in metadata.

## Snapshot model

Snapshots are the representation exposed to UI and debugger layers (not live `RobotWorld` mutation).

```csharp
public sealed record WorldTileSnapshot(
    int X,
    int Y,
    TerrainCellKind Terrain,
    ItemCellKind Item,
    int? ActorId);

public sealed record ActorSnapshot(
    int Id,
    ActorKind Kind,
    int X,
    int Y,
    Direction Direction,
    int InventoryCount);

public sealed record RobotWorldSnapshot(
    int Width,
    int Height,
    IReadOnlyList<WorldTileSnapshot> Tiles,
    IReadOnlyList<ActorSnapshot> Actors);
```

Prefer this flat tile list over pushing raw 2D arrays into UI layers.

## Runtime mutability

The interpreter mutates the live `RobotWorld`. The UI and renderer do not. Debugging is snapshot-based; rendering is a projection; replay and deterministic stepping stay tractable.

## World file schema (direction)

The next world-adjacent spec should define the on-disk format. For v1, JSON is a reasonable default:

```json
{
  "format": "RoboSharpWorld",
  "version": 1,
  "worldKind": "GridWorld",
  "name": "Intro Maze",
  "width": 8,
  "height": 6,
  "terrain": [
    "########",
    "#......#",
    "#..G...#",
    "#......#",
    "#......#",
    "########"
  ],
  "items": [
    { "kind": "Key", "x": 3, "y": 2 },
    { "kind": "MovableBlock", "x": 4, "y": 3 }
  ],
  "actors": [
    { "id": 1, "kind": "Robot", "x": 1, "y": 1, "direction": "East", "inventoryCount": 0 }
  ],
  "metadata": {
    "lessonId": "lesson-01",
    "primaryActorId": 1
  }
}
```

Loading and validation belong in IO/workspace or a dedicated serialization slice; the core world types above should anticipate this shape.

## Project placement

Recommended layout (names may vary; see [`AGENTS.md`](../../AGENTS.md)):

- **`RoboSharp.World`** — grids, cells, actor state, metadata, world API, snapshots, movement/push/pick rules, derived analysis helpers
- **Serialization** (separate concern) — world JSON schema, load/save, validation
- **Rendering adapters** — `RenderTile`, projector, ASCII/sprite renderers (see [`../rendering/`](../rendering/README.md))

## v1 decisions (frozen recommendations)

- Use `TerrainGrid`, `ItemGrid`, `ActorGrid`.
- Use `ActorCell(int ActorId)` rather than a cell subtype hierarchy.
- One actor per tile in v1; one item slot per tile in v1.
- Terrain: `Empty`, `Wall`, `Goal`.
- Item kinds: `None`, `PowerUp`, `Key`, `MovableBlock`.
- Rendering: snapshot + projector only (see rendering docs).
- Derived analysis: computed on demand, not stored as authoritative state.

## Related

- [Terrain grid](terrain-grid.md)
- [Item grid](item-grid.md)
- [Actor grid](actor-grid.md)
- [World actions API](world-actions.md)
- [Movement rules](movement-rules.md)
- [Metrics and analysis](metrics-and-analysis.md)
- [Render projection](../rendering/render-projection.md)
