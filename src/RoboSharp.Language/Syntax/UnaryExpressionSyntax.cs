namespace RoboSharp.Language.Syntax;

public sealed record UnaryExpressionSyntax(
    SyntaxToken OperatorToken,
    ExpressionSyntax Operand) : ExpressionSyntax;