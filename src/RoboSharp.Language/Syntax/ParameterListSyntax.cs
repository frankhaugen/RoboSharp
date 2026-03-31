namespace RoboSharp.Language.Syntax;

public sealed record ParameterListSyntax(
    SyntaxToken OpenParenToken,
    IReadOnlyList<ParameterSyntax> Parameters,
    IReadOnlyList<SyntaxToken> Commas,
    SyntaxToken CloseParenToken) : SyntaxNode;