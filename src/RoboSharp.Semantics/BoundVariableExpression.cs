using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed record BoundVariableExpression(
    NameExpressionSyntax Syntax,
    LocalSymbol Symbol) : BoundExpression(Symbol.Type);