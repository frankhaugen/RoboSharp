using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed class SemanticModel
{
    public SemanticModel(CompilationUnitSyntax syntaxTree, BoundCompilationUnit root, IReadOnlyList<SemanticDiagnostic> diagnostics)
    {
        SyntaxTree = syntaxTree;
        Root = root;
        Diagnostics = diagnostics;
    }

    public CompilationUnitSyntax SyntaxTree { get; }
    public BoundCompilationUnit Root { get; }
    public IReadOnlyList<SemanticDiagnostic> Diagnostics { get; }

    public bool HasErrors => Diagnostics.Count > 0;
}
