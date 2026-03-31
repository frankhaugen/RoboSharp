namespace RoboSharp.Language.Syntax;

public sealed record FunctionDeclarationSyntax(
    TypeSyntax ReturnType,
    SyntaxToken Identifier,
    ParameterListSyntax Parameters,
    BlockStatementSyntax Body) : MemberSyntax;