namespace RoboSharp.Language.Syntax;

public sealed record ParenthesizedExpressionSyntax(
    SyntaxToken OpenParenToken,
    ExpressionSyntax Expression,
    SyntaxToken CloseParenToken) : ExpressionSyntax;