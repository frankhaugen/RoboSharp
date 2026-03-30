namespace RoboSharp.Language.Syntax;

public sealed record PrimitiveTypeSyntax(SyntaxToken Keyword) : TypeSyntax;

public sealed record ArrayTypeSyntax(
    TypeSyntax ElementType,
    SyntaxToken OpenBracketToken,
    SyntaxToken CloseBracketToken) : TypeSyntax;
