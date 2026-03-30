# World actions API

The runtime should talk to the world through a **small, explicit API** instead of reaching into grid internals directly.

## Interface

```csharp
public interface IRobotWorld
{
    bool IsInside(GridPosition position);

    TerrainCell GetTerrain(GridPosition position);
    ItemCell GetItem(GridPosition position);
    ActorCell GetActor(GridPosition position);

    bool CanMoveForward(int actorId);
    WorldActionResult MoveForward(int actorId);

    void TurnLeft(int actorId);
    void TurnRight(int actorId);

    bool IsFrontClear(int actorId);
    bool IsLeftClear(int actorId);
    bool IsRightClear(int actorId);
}
```

## Action results

Structured results keep normal control flow out of exceptions:

```csharp
public readonly record struct WorldActionResult(
    bool Success,
    string? Message = null);
```

Built-ins call these world operations; they do not manipulate `TerrainGrid` / `ItemGrid` / `ActorGrid` directly.

## Related

- [Movement rules](movement-rules.md) — passability, push, pickup, sensing, direction helpers
