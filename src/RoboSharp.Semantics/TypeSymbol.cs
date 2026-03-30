namespace RoboSharp.Semantics;

public abstract record TypeSymbol;

public enum PrimitiveTypeKind
{
    Void,
    Int,
    Bool,
    String,
    Number,
}

public sealed record PrimitiveTypeSymbol(PrimitiveTypeKind Kind) : TypeSymbol
{
    public static readonly PrimitiveTypeSymbol Void = new(PrimitiveTypeKind.Void);
    public static readonly PrimitiveTypeSymbol Int = new(PrimitiveTypeKind.Int);
    public static readonly PrimitiveTypeSymbol Bool = new(PrimitiveTypeKind.Bool);
    public static readonly PrimitiveTypeSymbol String = new(PrimitiveTypeKind.String);
    public static readonly PrimitiveTypeSymbol Number = new(PrimitiveTypeKind.Number);
}

public sealed record ArrayTypeSymbol(TypeSymbol Element) : TypeSymbol;
