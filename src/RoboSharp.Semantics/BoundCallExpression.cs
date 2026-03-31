using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed record BoundCallExpression(
    CallExpressionSyntax Syntax,
    TypeSymbol Type,
    FunctionSymbol Function,
    IReadOnlyList<BoundExpression> Arguments) : BoundExpression(Type);