namespace RoboSharp.Language.Syntax;

public sealed record CompilationUnitSyntax(
    IReadOnlyList<MemberSyntax> Members,
    SyntaxToken EndOfFileToken) : SyntaxNode;
