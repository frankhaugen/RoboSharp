namespace RoboSharp.World;

public sealed record ActorSnapshot(
    int Id,
    ActorKind Kind,
    int X,
    int Y,
    Direction Direction,
    int InventoryCount);