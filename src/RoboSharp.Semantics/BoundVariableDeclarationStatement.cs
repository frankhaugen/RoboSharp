using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed record BoundVariableDeclarationStatement(
    VariableDeclarationStatementSyntax Syntax,
    LocalSymbol Symbol,
    BoundExpression Initializer) : BoundStatement;