namespace RoboSharp.Language.Syntax;

public sealed record CallExpressionSyntax(
    ExpressionSyntax Callee,
    SyntaxToken OpenParenToken,
    IReadOnlyList<ExpressionSyntax> Arguments,
    IReadOnlyList<SyntaxToken> ArgumentCommas,
    SyntaxToken CloseParenToken) : ExpressionSyntax;