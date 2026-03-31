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