namespace RoboSharp.Language.Syntax;

public sealed record WhileStatementSyntax(
    SyntaxToken WhileKeyword,
    SyntaxToken OpenParenToken,
    ExpressionSyntax Condition,
    SyntaxToken CloseParenToken,
    StatementSyntax Body) : StatementSyntax;