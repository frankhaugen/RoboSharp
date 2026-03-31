using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed record BoundArrayCreationExpression(
    ArrayLiteralExpressionSyntax Syntax,
    ArrayTypeSymbol ArrayType,
    IReadOnlyList<BoundExpression> Elements) : BoundExpression(ArrayType);