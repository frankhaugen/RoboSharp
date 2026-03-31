using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed record BoundIndexExpression(
    IndexExpressionSyntax Syntax,
    TypeSymbol ElementType,
    BoundExpression Target,
    BoundExpression Index) : BoundExpression(ElementType);