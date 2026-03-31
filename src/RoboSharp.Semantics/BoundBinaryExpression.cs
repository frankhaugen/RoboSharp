using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed record BoundBinaryExpression(
    BinaryExpressionSyntax Syntax,
    TypeSymbol Type,
    BoundExpression Left,
    BoundExpression Right) : BoundExpression(Type);