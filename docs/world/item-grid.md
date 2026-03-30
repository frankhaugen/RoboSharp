# Item grid

The item layer holds **non-actor** board content (pickups, pushable blocks, etc.).

## Cell kinds

```csharp
public enum ItemCellKind
{
    None,
    PowerUp,
    Key,
    MovableBlock
}

public readonly record struct ItemCell(ItemCellKind Kind)
{
    public bool HasItem => Kind is not ItemCellKind.None;
    public bool BlocksMovement => Kind is ItemCellKind.MovableBlock;
    public bool IsPickup => Kind is ItemCellKind.PowerUp or ItemCellKind.Key;
    public bool IsPushable => Kind is ItemCellKind.MovableBlock;
}
```

## Grid

```csharp
public sealed class ItemGrid
{
    private readonly ItemCell[,] _cells;

    public int Width => _cells.GetLength(0);
    public int Height => _cells.GetLength(1);

    public ItemGrid(ItemCell[,] cells) => _cells = cells;

    public ItemCell Get(GridPosition position) => _cells[position.X, position.Y];
    public void Set(GridPosition position, ItemCell value) => _cells[position.X, position.Y] = value;
}
```

## v1 rules

- At most one item slot per tile.
- `MovableBlock` blocks movement unless explicit push logic succeeds (see [Movement rules](movement-rules.md)).
- Pickup items do not block movement.
- Item behavior stays enum-based in v1, not polymorphic per cell type.

See [World model](world-model.md) for layering and [Movement rules](movement-rules.md) for push and pickup.
