namespace RoboSharp.World;

public sealed class ActorGrid
{
    private readonly ActorCell[,] _cells;

    public ActorGrid(ActorCell[,] cells)
    {
        ArgumentNullException.ThrowIfNull(cells);
        _cells = cells;
    }

    public int Width => _cells.GetLength(0);
    public int Height => _cells.GetLength(1);

    public ActorCell Get(GridPosition position) => _cells[position.X, position.Y];

    public void Set(GridPosition position, ActorCell value) => _cells[position.X, position.Y] = value;
}
