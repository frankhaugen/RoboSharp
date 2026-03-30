# Render projection

Rendering must remain separate from live `RobotWorld` mutation. Consumers work from **`RobotWorldSnapshot`** (see [`../world/world-model.md`](../world/world-model.md)).

## Render tile

```csharp
public readonly record struct RenderTile(
    GridPosition Position,
    TerrainCellKind Terrain,
    ItemCellKind Item,
    int? ActorId,
    Direction? ActorDirection);
```

## Projector interface

```csharp
public interface IWorldRenderProjector
{
    RenderTile[,] Project(RobotWorldSnapshot snapshot);
}
```

Renderers consume `RenderTile[,]`, not live world internals.

## Layer priority (fixed)

When composing a single visual cell:

```text
Actor > Item > Terrain
```

Use the same order for ASCII, sprite rendering, IDE world view, and debugger overlays to avoid ambiguous “what wins visually?” behavior.

## Related

- [ASCII renderer](ascii-renderer.md)
- [Sprite renderer](sprite-renderer.md)
