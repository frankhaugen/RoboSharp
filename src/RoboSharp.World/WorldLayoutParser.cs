namespace RoboSharp.World;

/// <summary>Parses ASCII world layouts into terrain and start positions (premade maps in source).</summary>
public static class WorldLayoutParser
{
    public sealed record LayoutResult(
        TerrainCellKind[,] Terrain,
        GridPosition RobotStart,
        Direction RobotFacing,
        GridPosition? PrimaryGoal);

    /// <summary>
    /// Characters: <c>#</c> wall, <c>.</c> or space floor, <c>G</c> goal, <c>@</c> robot (faces East).
    /// Lines are trimmed of trailing spaces; width is the longest line (others padded with floor).
    /// </summary>
    public static LayoutResult Parse(IReadOnlyList<string> lines)
    {
        var trimmed = lines.Select(l => l.TrimEnd()).Where(l => l.Length > 0).ToList();
        if (trimmed.Count == 0)
            throw new ArgumentException("Layout must contain at least one non-empty line.", nameof(lines));

        var height = trimmed.Count;
        var width = trimmed.Max(l => l.Length);
        var terrain = new TerrainCellKind[width, height];
        GridPosition? robot = null;
        GridPosition? goal = null;
        var facing = Direction.East;

        for (var y = 0; y < height; y++)
        {
            var line = trimmed[y];
            for (var x = 0; x < width; x++)
            {
                var ch = x < line.Length ? line[x] : '.';
                switch (ch)
                {
                    case '#':
                        terrain[x, y] = TerrainCellKind.Wall;
                        break;
                    case 'G':
                    case 'g':
                        terrain[x, y] = TerrainCellKind.Goal;
                        goal ??= new GridPosition(x, y);
                        break;
                    case '@':
                        terrain[x, y] = TerrainCellKind.Empty;
                        robot = new GridPosition(x, y);
                        break;
                    case '.':
                    case ' ':
                        terrain[x, y] = TerrainCellKind.Empty;
                        break;
                    default:
                        terrain[x, y] = TerrainCellKind.Empty;
                        break;
                }
            }
        }

        if (robot is null)
            throw new ArgumentException("Layout must contain '@' for the robot start position.", nameof(lines));

        return new LayoutResult(terrain, robot.Value, facing, goal);
    }
}
