using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed record BoundBlockStatement(
    BlockStatementSyntax Syntax,
    IReadOnlyList<BoundStatement> Statements) : BoundStatement;