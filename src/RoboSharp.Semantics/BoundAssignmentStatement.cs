using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed record BoundAssignmentStatement(
    AssignmentStatementSyntax Syntax,
    LocalSymbol Symbol,
    BoundExpression Expression) : BoundStatement;