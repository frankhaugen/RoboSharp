namespace RoboSharp.Language.Syntax;

public sealed record GlobalStatementSyntax(StatementSyntax Statement) : MemberSyntax;

public sealed record FunctionDeclarationSyntax(
    TypeSyntax ReturnType,
    SyntaxToken Identifier,
    ParameterListSyntax Parameters,
    BlockStatementSyntax Body) : MemberSyntax;

public sealed record ParameterListSyntax(
    SyntaxToken OpenParenToken,
    IReadOnlyList<ParameterSyntax> Parameters,
    IReadOnlyList<SyntaxToken> Commas,
    SyntaxToken CloseParenToken) : SyntaxNode;

public sealed record ParameterSyntax(TypeSyntax Type, SyntaxToken Identifier) : SyntaxNode;
