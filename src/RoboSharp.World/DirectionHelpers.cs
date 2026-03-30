namespace RoboSharp.World;

public static class DirectionHelpers
{
    public static GridPosition GetForwardOffset(Direction direction) =>
        direction switch
        {
            Direction.North => new GridPosition(0, -1),
            Direction.East => new GridPosition(1, 0),
            Direction.South => new GridPosition(0, 1),
            Direction.West => new GridPosition(-1, 0),
            _ => throw new InvalidOperationException(),
        };

    public static Direction TurnLeft(Direction direction) =>
        direction switch
        {
            Direction.North => Direction.West,
            Direction.West => Direction.South,
            Direction.South => Direction.East,
            Direction.East => Direction.North,
            _ => throw new InvalidOperationException(),
        };

    public static Direction TurnRight(Direction direction) =>
        direction switch
        {
            Direction.North => Direction.East,
            Direction.East => Direction.South,
            Direction.South => Direction.West,
            Direction.West => Direction.North,
            _ => throw new InvalidOperationException(),
        };

    /// <summary>Direction to the robot's left relative to its facing.</summary>
    public static Direction GetLeftRelative(Direction facing) => TurnLeft(facing);

    /// <summary>Direction to the robot's right relative to its facing.</summary>
    public static Direction GetRightRelative(Direction facing) => TurnRight(facing);
}
