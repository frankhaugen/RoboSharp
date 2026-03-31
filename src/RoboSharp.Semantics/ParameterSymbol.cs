namespace RoboSharp.Semantics;

public sealed class ParameterSymbol
{
    public ParameterSymbol(string name, TypeSymbol type, int slotIndex)
    {
        Name = name;
        Type = type;
        SlotIndex = slotIndex;
    }

    public string Name { get; }
    public TypeSymbol Type { get; }
    public int SlotIndex { get; }
}