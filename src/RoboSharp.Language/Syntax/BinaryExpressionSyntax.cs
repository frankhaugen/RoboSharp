namespace RoboSharp.Language.Syntax;

public sealed record BinaryExpressionSyntax(
    ExpressionSyntax Left,
    SyntaxToken OperatorToken,
    ExpressionSyntax Right) : ExpressionSyntax;