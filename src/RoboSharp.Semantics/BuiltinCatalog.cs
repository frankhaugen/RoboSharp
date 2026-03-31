namespace RoboSharp.Semantics;

public sealed record BuiltinSignature(BuiltinId Id, TypeSymbol ReturnType, IReadOnlyList<TypeSymbol> ParameterTypes);

public static class BuiltinCatalog
{
    private static readonly Dictionary<string, BuiltinSignature> ByName = CreateMap();

    private static Dictionary<string, BuiltinSignature> CreateMap()
    {
        var void_ = PrimitiveTypeSymbol.Void;
        var int_ = PrimitiveTypeSymbol.Int;
        var bool_ = PrimitiveTypeSymbol.Bool;
        var str = PrimitiveTypeSymbol.String;
        var intArr = new ArrayTypeSymbol(int_);

        return new Dictionary<string, BuiltinSignature>(StringComparer.Ordinal)
        {
            ["move"] = new BuiltinSignature(BuiltinId.Move, void_, []),
            ["turnLeft"] = new BuiltinSignature(BuiltinId.TurnLeft, void_, []),
            ["turnRight"] = new BuiltinSignature(BuiltinId.TurnRight, void_, []),
            ["pick"] = new BuiltinSignature(BuiltinId.Pick, void_, []),
            ["drop"] = new BuiltinSignature(BuiltinId.Drop, void_, []),
            ["frontIsClear"] = new BuiltinSignature(BuiltinId.FrontIsClear, bool_, []),
            ["leftIsClear"] = new BuiltinSignature(BuiltinId.LeftIsClear, bool_, []),
            ["rightIsClear"] = new BuiltinSignature(BuiltinId.RightIsClear, bool_, []),
            ["print"] = new BuiltinSignature(BuiltinId.Print, void_, [int_]), // relaxed to multiple types at bind time
            ["count"] = new BuiltinSignature(BuiltinId.Count, int_, [intArr]),
            ["add"] = new BuiltinSignature(BuiltinId.Add, void_, [intArr, int_]),
            ["getLast"] = new BuiltinSignature(BuiltinId.GetLast, int_, [intArr]),
            ["takeLast"] = new BuiltinSignature(BuiltinId.TakeLast, int_, [intArr]),
        };
    }

    public static bool TryGet(string name, out BuiltinSignature signature) => ByName.TryGetValue(name, out signature!);

    /// <summary>All names that appear in the canonical map (for lesson help / UI).</summary>
    public static IReadOnlyList<string> AllBuiltinNames { get; } = ByName.Keys.OrderBy(static n => n, StringComparer.Ordinal).ToArray();

    /// <summary>Resolves <c>print</c> overload by first argument type.</summary>
    public static bool TryResolvePrint(TypeSymbol argType, out BuiltinSignature signature)
    {
        signature = new BuiltinSignature(BuiltinId.Print, PrimitiveTypeSymbol.Void, [argType]);
        return argType is PrimitiveTypeSymbol or ArrayTypeSymbol;
    }
}

public sealed class FullBuiltinProfileProvider : IBuiltinProfileProvider
{
    public bool IsAvailable(BuiltinId id) => true;
}
