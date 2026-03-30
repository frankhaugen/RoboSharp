# Actor grid

The actor grid is **positional**; detailed actor data lives outside the grid.

## Actor cell

```csharp
public readonly record struct ActorCell(int ActorId)
{
    public static ActorCell Empty => new(0);
    public bool HasActor => ActorId != 0;
}
```

## Actor state

```csharp
public enum ActorKind
{
    Robot
}

public sealed class ActorState
{
    public required int Id { get; init; }
    public required ActorKind Kind { get; init; }
    public required GridPosition Position { get; set; }
    public required Direction Direction { get; set; }

    public int InventoryCount { get; set; }
}
```

## Grid

```csharp
public sealed class ActorGrid
{
    private readonly ActorCell[,] _cells;

    public int Width => _cells.GetLength(0);
    public int Height => _cells.GetLength(1);

    public ActorGrid(ActorCell[,] cells) => _cells = cells;

    public ActorCell Get(GridPosition position) => _cells[position.X, position.Y];
    public void Set(GridPosition position, ActorCell value) => _cells[position.X, position.Y] = value;
}
```

## Why actor-id indirection

Do not store full actor objects inside the grid cells.

- The grid answers **occupancy**.
- `ActorsById` answers **actor state**.

Benefits:

- simple positional queries
- future multiplayer without redesign
- richer actor metadata later
- cleaner snapshots
- easier diffing in debugger/UI

Direction and inventory already justify separating `ActorState` from the grid cell.

See [World model](world-model.md) for `RobotWorld` composition.
