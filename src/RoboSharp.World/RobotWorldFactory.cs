namespace RoboSharp.World;

public static class RobotWorldFactory
{
    /// <summary>Empty interior, wall border, one robot at (1,1) facing East.</summary>
    public static RobotWorld CreateBorderedEmpty(int width, int height, int primaryActorId = 1)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 3);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 3);

        var terrain = new TerrainCell[width, height];
        var items = new ItemCell[width, height];
        var actors = new ActorCell[width, height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var isBorder = x == 0 || y == 0 || x == width - 1 || y == height - 1;
                terrain[x, y] = new TerrainCell(isBorder ? TerrainCellKind.Wall : TerrainCellKind.Empty);
                items[x, y] = new ItemCell(ItemCellKind.None);
                actors[x, y] = ActorCell.Empty;
            }
        }

        var start = new GridPosition(1, 1);
        actors[start.X, start.Y] = new ActorCell(primaryActorId);

        var actorState = new ActorState
        {
            Id = primaryActorId,
            Kind = ActorKind.Robot,
            Position = start,
            Direction = Direction.East,
            InventoryCount = 0,
        };

        var dict = new Dictionary<int, ActorState> { [primaryActorId] = actorState };

        var meta = new WorldMetadata
        {
            Name = "Empty",
            Width = width,
            Height = height,
            PrimaryActorId = primaryActorId,
        };

        return new RobotWorld
        {
            Terrain = new TerrainGrid(terrain),
            Items = new ItemGrid(items),
            Actors = new ActorGrid(actors),
            ActorsById = dict,
            Metadata = meta,
        };
    }
}
