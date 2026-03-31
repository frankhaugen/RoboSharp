namespace RoboSharp.Language.Syntax;

public sealed record ReturnStatementSyntax(
    SyntaxToken ReturnKeyword,
    ExpressionSyntax? Expression,
    SyntaxToken SemicolonToken) : StatementSyntax;