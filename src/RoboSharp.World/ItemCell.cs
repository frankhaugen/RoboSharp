namespace RoboSharp.World;

public readonly record struct ItemCell(ItemCellKind Kind)
{
    public bool HasItem => Kind is not ItemCellKind.None;
    public bool BlocksMovement => Kind is ItemCellKind.MovableBlock;
    public bool IsPickup => Kind is ItemCellKind.PowerUp or ItemCellKind.Key;
    public bool IsPushable => Kind is ItemCellKind.MovableBlock;
}
