using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed record BoundIfStatement(
    IfStatementSyntax Syntax,
    BoundExpression Condition,
    BoundStatement ThenStatement,
    BoundStatement? ElseStatement) : BoundStatement;