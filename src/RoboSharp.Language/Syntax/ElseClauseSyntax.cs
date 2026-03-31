namespace RoboSharp.Language.Syntax;

public sealed record ElseClauseSyntax(
    SyntaxToken ElseKeyword,
    StatementSyntax Statement) : SyntaxNode;