namespace RoboSharp.World;

public sealed class WorldMetadata
{
    public required string Name { get; init; }
    public string? Description { get; init; }

    public string? LessonId { get; init; }
    public GridPosition? PrimaryGoalPosition { get; init; }

    public int Width { get; init; }
    public int Height { get; init; }

    public int? PrimaryActorId { get; init; }
}
