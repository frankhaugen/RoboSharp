using System.Text;
using RoboSharp.World;

namespace RoboSharp.Studio.Shell;

/// <summary>ASCII grid for the left “Karel” pane — walls, floor, goal, robot facing.</summary>
public static class KarelWorldAsciiFormatter
{
    public static string Format(RobotWorldSnapshot snapshot)
    {
        var grid = new char[snapshot.Width, snapshot.Height];
        foreach (var t in snapshot.Tiles)
        {
            grid[t.X, t.Y] = t.Terrain switch
            {
                TerrainCellKind.Wall => '#',
                TerrainCellKind.Goal => '*',
                TerrainCellKind.Empty => '.',
                _ => '.',
            };
        }

        foreach (var a in snapshot.Actors)
        {
            if (a.X < 0 || a.Y < 0 || a.X >= snapshot.Width || a.Y >= snapshot.Height)
                continue;
            grid[a.X, a.Y] = a.Direction switch
            {
                Direction.North => '^',
                Direction.East => '>',
                Direction.South => 'v',
                Direction.West => '<',
                _ => '?',
            };
        }

        var sb = new StringBuilder();
        for (var y = 0; y < snapshot.Height; y++)
        {
            for (var x = 0; x < snapshot.Width; x++)
                sb.Append(grid[x, y]);
            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }
}
