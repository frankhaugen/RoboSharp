namespace RoboSharp.Language.Syntax;

public sealed record ExpressionStatementSyntax(
    ExpressionSyntax Expression,
    SyntaxToken SemicolonToken) : StatementSyntax;