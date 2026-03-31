using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed record BoundExpressionStatement(
    ExpressionStatementSyntax Syntax,
    BoundExpression Expression) : BoundStatement;