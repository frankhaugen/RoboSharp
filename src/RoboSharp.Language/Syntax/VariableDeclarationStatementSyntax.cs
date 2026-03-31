namespace RoboSharp.Language.Syntax;

public sealed record VariableDeclarationStatementSyntax(
    TypeSyntax Type,
    SyntaxToken Identifier,
    SyntaxToken EqualsToken,
    ExpressionSyntax Initializer,
    SyntaxToken SemicolonToken) : StatementSyntax;