namespace RoboSharp.World;

public sealed record WorldTileSnapshot(
    int X,
    int Y,
    TerrainCellKind Terrain,
    ItemCellKind Item,
    int? ActorId);