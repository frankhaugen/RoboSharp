namespace RoboSharp.Semantics;

public sealed class FunctionSymbol
{
    public FunctionSymbol(string name, TypeSymbol returnType, IReadOnlyList<ParameterSymbol> parameters)
    {
        Name = name;
        ReturnType = returnType;
        Parameters = parameters;
    }

    public string Name { get; }
    public TypeSymbol ReturnType { get; }
    public IReadOnlyList<ParameterSymbol> Parameters { get; }
}

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
