namespace RoboSharp.Semantics;

public sealed class LocalSymbol
{
    public LocalSymbol(string name, TypeSymbol type, int slotIndex)
    {
        Name = name;
        Type = type;
        SlotIndex = slotIndex;
    }

    public string Name { get; }
    public TypeSymbol Type { get; }
    public int SlotIndex { get; }
}