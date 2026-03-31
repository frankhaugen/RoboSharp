namespace RoboSharp.World;

public sealed record RobotWorldSnapshot(
    int Width,
    int Height,
    IReadOnlyList<WorldTileSnapshot> Tiles,
    IReadOnlyList<ActorSnapshot> Actors);
