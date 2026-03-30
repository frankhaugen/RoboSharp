namespace RoboSharp.World;

public sealed class TerrainGrid
{
    private readonly TerrainCell[,] _cells;

    public TerrainGrid(TerrainCell[,] cells)
    {
        ArgumentNullException.ThrowIfNull(cells);
        _cells = cells;
    }

    public int Width => _cells.GetLength(0);
    public int Height => _cells.GetLength(1);

    public TerrainCell Get(GridPosition position) => _cells[position.X, position.Y];

    public void Set(GridPosition position, TerrainCell value) => _cells[position.X, position.Y] = value;
}
