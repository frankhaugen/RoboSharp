namespace RoboSharp.World;

/// <summary>Grid cell address; (0,0) is top-left, X right, Y down.</summary>
public readonly record struct GridPosition(int X, int Y);
