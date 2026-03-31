namespace RoboSharp.Language.Syntax;

public sealed record IndexExpressionSyntax(
    ExpressionSyntax Target,
    SyntaxToken OpenBracketToken,
    ExpressionSyntax Index,
    SyntaxToken CloseBracketToken) : ExpressionSyntax;