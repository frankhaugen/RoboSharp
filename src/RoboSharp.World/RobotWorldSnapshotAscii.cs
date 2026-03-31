namespace RoboSharp.World;

/// <summary>ASCII grid from a snapshot (one character per cell) for terminal and simple web previews.</summary>
public static class RobotWorldSnapshotAscii
{
    public static string Format(RobotWorldSnapshot snapshot)
    {
        if (snapshot.Width <= 0 || snapshot.Height <= 0)
            return "(empty world)";

        var terrain = new TerrainCellKind[snapshot.Width, snapshot.Height];
        foreach (var t in snapshot.Tiles)
        {
            if (t.X >= 0 && t.X < snapshot.Width && t.Y >= 0 && t.Y < snapshot.Height)
                terrain[t.X, t.Y] = t.Terrain;
        }

        var rowBuffer = new char[snapshot.Width];
        var lines = new string[snapshot.Height];
        for (var y = 0; y < snapshot.Height; y++)
        {
            for (var x = 0; x < snapshot.Width; x++)
            {
                var actor = PrimaryActorAt(snapshot.Actors, x, y);
                rowBuffer[x] = actor is not null
                    ? DirectionGlyph(actor.Direction)
                    : TerrainGlyph(terrain[x, y]);
            }

            lines[y] = new string(rowBuffer);
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static ActorSnapshot? PrimaryActorAt(IReadOnlyList<ActorSnapshot> actors, int x, int y)
    {
        ActorSnapshot? pick = null;
        foreach (var a in actors)
        {
            if (a.X != x || a.Y != y)
                continue;
            if (a.Id == 1)
                return a;
            pick ??= a;
        }

        return pick;
    }

    private static char TerrainGlyph(TerrainCellKind k) =>
        k switch
        {
            TerrainCellKind.Wall => '#',
            TerrainCellKind.Goal => 'G',
            _ => '.',
        };

    private static char DirectionGlyph(Direction d) =>
        d switch
        {
            Direction.North => '^',
            Direction.East => '>',
            Direction.South => 'v',
            Direction.West => '<',
            _ => 'R',
        };
}
