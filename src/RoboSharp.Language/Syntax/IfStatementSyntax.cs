namespace RoboSharp.Language.Syntax;

public sealed record IfStatementSyntax(
    SyntaxToken IfKeyword,
    SyntaxToken OpenParenToken,
    ExpressionSyntax Condition,
    SyntaxToken CloseParenToken,
    StatementSyntax ThenStatement,
    ElseClauseSyntax? ElseClause) : StatementSyntax;