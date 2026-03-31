using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed record BoundUnaryExpression(
    UnaryExpressionSyntax Syntax,
    TypeSymbol Type,
    BoundExpression Operand) : BoundExpression(Type);