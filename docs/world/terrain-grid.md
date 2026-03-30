# Terrain grid

Terrain is the base board and should stay small in v1.

## Cell kinds

```csharp
public enum TerrainCellKind
{
    Empty,
    Wall,
    Goal
}

public readonly record struct TerrainCell(TerrainCellKind Kind)
{
    public bool IsWalkable => Kind is not TerrainCellKind.Wall;
    public bool IsGoal => Kind is TerrainCellKind.Goal;
}
```

## Grid

```csharp
public sealed class TerrainGrid
{
    private readonly TerrainCell[,] _cells;

    public int Width => _cells.GetLength(0);
    public int Height => _cells.GetLength(1);

    public TerrainGrid(TerrainCell[,] cells) => _cells = cells;

    public TerrainCell Get(GridPosition position) => _cells[position.X, position.Y];
    public void Set(GridPosition position, TerrainCell value) => _cells[position.X, position.Y] = value;
}
```

## v1 scope

- `Empty`, `Wall`, `Goal` only.
- No ice, lava, teleports, conveyors, or one-way gates yet.

See [World model](world-model.md) for coordinates and layering.
