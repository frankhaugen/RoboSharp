namespace RoboSharp.Language.Syntax;

public sealed record ParameterSyntax(TypeSyntax Type, SyntaxToken Identifier) : SyntaxNode;