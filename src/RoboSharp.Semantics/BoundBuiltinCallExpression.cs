using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed record BoundBuiltinCallExpression(
    CallExpressionSyntax Syntax,
    TypeSymbol Type,
    BuiltinId Builtin,
    IReadOnlyList<BoundExpression> Arguments) : BoundExpression(Type);