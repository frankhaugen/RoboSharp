namespace RoboSharp.Language.Syntax;

public sealed record ArrayLiteralExpressionSyntax(
    SyntaxToken OpenBracketToken,
    IReadOnlyList<ExpressionSyntax> Elements,
    IReadOnlyList<SyntaxToken> ElementCommas,
    SyntaxToken CloseBracketToken) : ExpressionSyntax;