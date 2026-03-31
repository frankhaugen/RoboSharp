namespace RoboSharp.Language.Syntax;

public sealed record AssignmentStatementSyntax(
    SyntaxToken Identifier,
    SyntaxToken EqualsToken,
    ExpressionSyntax Expression,
    SyntaxToken SemicolonToken) : StatementSyntax;