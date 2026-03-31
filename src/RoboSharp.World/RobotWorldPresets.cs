namespace RoboSharp.World;

/// <summary>Named worlds for Studio / lessons — bordered boxes or <see cref="EmbeddedWorldLayouts"/>.</summary>
public static class RobotWorldPresets
{
    public const string Arena16Id = "arena-16";
    public const string Arena12Id = "arena-12";
    public const string Arena8Id = "arena-8";
    public const string GoalCornerId = "goal-corner";
    public const string AroundWallId = "around-wall";
    public const string CorridorMazeId = "corridor-maze";
    public const string OpenPlaygroundId = "open-playground";

    public static IReadOnlyList<(string Id, string DisplayName)> OrderedPresets { get; } =
    [
        (Arena8Id, "Small arena (8×6)"),
        (Arena12Id, "Medium arena (12×10)"),
        (Arena16Id, "Large arena (16×16)"),
        (GoalCornerId, "Reach the goal (corner)"),
        (AroundWallId, "Go around a wall"),
        (CorridorMazeId, "Corridor maze"),
        (OpenPlaygroundId, "Open playground + goal"),
    ];

    public static string GetDisplayName(string presetId) =>
        OrderedPresets.FirstOrDefault(p => string.Equals(p.Id, presetId, StringComparison.OrdinalIgnoreCase)).DisplayName
        ?? "Custom";

    public static RobotWorld Create(string presetId)
    {
        if (string.IsNullOrWhiteSpace(presetId))
            return RobotWorldFactory.CreateBorderedEmpty(12, 10);

        return presetId.ToLowerInvariant() switch
        {
            Arena8Id => RobotWorldFactory.CreateBorderedEmpty(8, 6),
            Arena12Id => RobotWorldFactory.CreateBorderedEmpty(12, 10),
            Arena16Id => RobotWorldFactory.CreateBorderedEmpty(16, 16),
            GoalCornerId => FromLayout(EmbeddedWorldLayouts.ReachGoalCorner, "Reach goal (corner)", "Get to the teal goal tile."),
            AroundWallId => FromLayout(EmbeddedWorldLayouts.AroundWall, "Around the wall", "Navigate around walls to the goal."),
            CorridorMazeId => FromLayout(EmbeddedWorldLayouts.CorridorMaze, "Corridor maze", "Follow corridors to the goal."),
            OpenPlaygroundId => FromLayout(EmbeddedWorldLayouts.OpenPlayground, "Open playground", "Lots of room to experiment."),
            _ => RobotWorldFactory.CreateBorderedEmpty(12, 10),
        };
    }

    private static RobotWorld FromLayout(IReadOnlyList<string> lines, string name, string? description)
    {
        var parsed = WorldLayoutParser.Parse(lines);
        var w = parsed.Terrain.GetLength(0);
        var h = parsed.Terrain.GetLength(1);
        var terrainCells = new TerrainCell[w, h];
        for (var y = 0; y < h; y++)
        {
            for (var x = 0; x < w; x++)
                terrainCells[x, y] = new TerrainCell(parsed.Terrain[x, y]);
        }

        var items = new ItemCell[w, h];
        var actors = new ActorCell[w, h];
        for (var y = 0; y < h; y++)
        {
            for (var x = 0; x < w; x++)
                items[x, y] = new ItemCell(ItemCellKind.None);
        }

        actors[parsed.RobotStart.X, parsed.RobotStart.Y] = new ActorCell(1);
        var actorState = new ActorState
        {
            Id = 1,
            Kind = ActorKind.Robot,
            Position = parsed.RobotStart,
            Direction = parsed.RobotFacing,
            InventoryCount = 0,
        };

        var meta = new WorldMetadata
        {
            Name = name,
            Description = description,
            Width = w,
            Height = h,
            PrimaryActorId = 1,
            PrimaryGoalPosition = parsed.PrimaryGoal,
        };

        return new RobotWorld
        {
            Terrain = new TerrainGrid(terrainCells),
            Items = new ItemGrid(items),
            Actors = new ActorGrid(actors),
            ActorsById = new Dictionary<int, ActorState> { [1] = actorState },
            Metadata = meta,
        };
    }
}
