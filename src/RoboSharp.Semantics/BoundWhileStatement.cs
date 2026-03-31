using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed record BoundWhileStatement(
    WhileStatementSyntax Syntax,
    BoundExpression Condition,
    BoundStatement Body) : BoundStatement;