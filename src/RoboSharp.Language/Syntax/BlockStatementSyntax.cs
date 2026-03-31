namespace RoboSharp.Language.Syntax;

public sealed record BlockStatementSyntax(
    SyntaxToken OpenBraceToken,
    IReadOnlyList<StatementSyntax> Statements,
    SyntaxToken CloseBraceToken) : StatementSyntax;