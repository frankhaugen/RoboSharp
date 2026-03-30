namespace RoboSharp.World;

public sealed record WorldTileSnapshot(
    int X,
    int Y,
    TerrainCellKind Terrain,
    ItemCellKind Item,
    int? ActorId);

public sealed record ActorSnapshot(
    int Id,
    ActorKind Kind,
    int X,
    int Y,
    Direction Direction,
    int InventoryCount);

public sealed record RobotWorldSnapshot(
    int Width,
    int Height,
    IReadOnlyList<WorldTileSnapshot> Tiles,
    IReadOnlyList<ActorSnapshot> Actors);
