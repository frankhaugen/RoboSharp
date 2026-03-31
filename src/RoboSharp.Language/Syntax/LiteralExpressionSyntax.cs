namespace RoboSharp.Language.Syntax;

public sealed record LiteralExpressionSyntax(SyntaxToken LiteralToken) : ExpressionSyntax;

public sealed record NameExpressionSyntax(SyntaxToken IdentifierToken) : ExpressionSyntax;

public sealed record UnaryExpressionSyntax(
    SyntaxToken OperatorToken,
    ExpressionSyntax Operand) : ExpressionSyntax;

public sealed record BinaryExpressionSyntax(
    ExpressionSyntax Left,
    SyntaxToken OperatorToken,
    ExpressionSyntax Right) : ExpressionSyntax;

public sealed record ParenthesizedExpressionSyntax(
    SyntaxToken OpenParenToken,
    ExpressionSyntax Expression,
    SyntaxToken CloseParenToken) : ExpressionSyntax;

public sealed record CallExpressionSyntax(
    ExpressionSyntax Callee,
    SyntaxToken OpenParenToken,
    IReadOnlyList<ExpressionSyntax> Arguments,
    IReadOnlyList<SyntaxToken> ArgumentCommas,
    SyntaxToken CloseParenToken) : ExpressionSyntax;

public sealed record ArrayLiteralExpressionSyntax(
    SyntaxToken OpenBracketToken,
    IReadOnlyList<ExpressionSyntax> Elements,
    IReadOnlyList<SyntaxToken> ElementCommas,
    SyntaxToken CloseBracketToken) : ExpressionSyntax;

public sealed record IndexExpressionSyntax(
    ExpressionSyntax Target,
    SyntaxToken OpenBracketToken,
    ExpressionSyntax Index,
    SyntaxToken CloseBracketToken) : ExpressionSyntax;
