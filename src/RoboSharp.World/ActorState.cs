namespace RoboSharp.World;

public sealed class ActorState
{
    public required int Id { get; init; }
    public required ActorKind Kind { get; init; }
    public required GridPosition Position { get; set; }
    public required Direction Direction { get; set; }

    public int InventoryCount { get; set; }
}
