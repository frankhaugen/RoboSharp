using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed record BoundLiteralExpression(
    LiteralExpressionSyntax Syntax,
    TypeSymbol Type,
    object Value) : BoundExpression(Type);