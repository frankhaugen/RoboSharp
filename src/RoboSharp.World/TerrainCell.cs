namespace RoboSharp.World;

public readonly record struct TerrainCell(TerrainCellKind Kind)
{
    public bool IsWalkable => Kind is not TerrainCellKind.Wall;
    public bool IsGoal => Kind is TerrainCellKind.Goal;
}
