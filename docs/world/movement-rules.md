# Movement rules

## Direction helpers

Use explicit helpers instead of magic integers:

```csharp
public static class DirectionHelpers
{
    public static GridPosition GetForwardOffset(Direction direction) => direction switch
    {
        Direction.North => new GridPosition(0, -1),
        Direction.East  => new GridPosition(1, 0),
        Direction.South => new GridPosition(0, 1),
        Direction.West  => new GridPosition(-1, 0),
        _ => throw new UnreachableException()
    };

    public static Direction TurnLeft(Direction direction) => direction switch
    {
        Direction.North => Direction.West,
        Direction.West  => Direction.South,
        Direction.South => Direction.East,
        Direction.East  => Direction.North,
        _ => throw new UnreachableException()
    };

    public static Direction TurnRight(Direction direction) => direction switch
    {
        Direction.North => Direction.East,
        Direction.East  => Direction.South,
        Direction.South => Direction.West,
        Direction.West  => Direction.North,
        _ => throw new UnreachableException()
    };
}
```

## Effective walkability (v1)

A move is legal only if all relevant layers agree. Canonical rule:

```csharp
public bool IsWalkable(GridPosition position)
{
    var terrain = Terrain.Get(position);
    var item = Items.Get(position);
    var actor = Actors.Get(position);

    return terrain.IsWalkable
        && !item.BlocksMovement
        && !actor.HasActor;
}
```

Interpretation:

- Wall blocks; empty and goal terrain allow (per terrain rules).
- Movable block blocks unless push handling applies.
- Pickup items do not block.
- Another actor on the tile blocks.

## Push semantics

Push logic is explicit and separate from normal walkability.

If `move()` targets a tile with `MovableBlock`:

1. Inspect the tile beyond the block.
2. Verify the beyond tile is enterable for the block.
3. Move the block in `ItemGrid`.
4. Move the actor in `ActorGrid`.

This stays cleaner with three layers than with a single merged cell hierarchy. Push can be optional in the first implementation, but the data model should support it from the start.

## Pickup semantics (v1)

Keep pickup intentionally simple:

- `pick()` succeeds only if the current tile has a pickup item.
- Successful pickup removes the item from `ItemGrid`.
- Actor `InventoryCount` increments.
- `drop()` decrements inventory and places a default dropped item only if the target/current tile has no item.

Tighter v1 option: model only an integer inventory count, not typed held items yet—enough for teaching state change.

## Sensing

Built-ins such as `frontIsClear()` are **pure queries** over world state:

```csharp
bool IsFrontClear(int actorId);
bool IsLeftClear(int actorId);
bool IsRightClear(int actorId);
```

They inspect adjacent tiles using the same movement logic **without** mutating the world. Turning affects sensing; sensing is state-dependent; movement legality stays predictable.

See [World actions API](world-actions.md) for the surface those queries sit on.
