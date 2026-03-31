namespace RoboSharp.Language.Syntax;

public sealed record ArrayTypeSyntax(
    TypeSyntax ElementType,
    SyntaxToken OpenBracketToken,
    SyntaxToken CloseBracketToken) : TypeSyntax;