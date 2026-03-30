namespace RoboSharp.World;

public sealed class ItemGrid
{
    private readonly ItemCell[,] _cells;

    public ItemGrid(ItemCell[,] cells)
    {
        ArgumentNullException.ThrowIfNull(cells);
        _cells = cells;
    }

    public int Width => _cells.GetLength(0);
    public int Height => _cells.GetLength(1);

    public ItemCell Get(GridPosition position) => _cells[position.X, position.Y];

    public void Set(GridPosition position, ItemCell value) => _cells[position.X, position.Y] = value;
}
