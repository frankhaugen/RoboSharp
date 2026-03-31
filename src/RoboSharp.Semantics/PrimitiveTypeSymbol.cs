namespace RoboSharp.Semantics;

public sealed record PrimitiveTypeSymbol(PrimitiveTypeKind Kind) : TypeSymbol
{
    public static readonly PrimitiveTypeSymbol Void = new(PrimitiveTypeKind.Void);
    public static readonly PrimitiveTypeSymbol Int = new(PrimitiveTypeKind.Int);
    public static readonly PrimitiveTypeSymbol Bool = new(PrimitiveTypeKind.Bool);
    public static readonly PrimitiveTypeSymbol String = new(PrimitiveTypeKind.String);
    public static readonly PrimitiveTypeSymbol Number = new(PrimitiveTypeKind.Number);
}