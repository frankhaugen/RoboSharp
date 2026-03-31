namespace RoboSharp.Semantics;

public sealed record BoundConversionExpression(
    BoundExpression Operand,
    TypeSymbol TargetType) : BoundExpression(TargetType);