using RoboSharp.Language.Syntax;

namespace RoboSharp.Language;

/// <summary>
/// Parsed source: root syntax, text, and parse diagnostics.
/// </summary>
public sealed class SyntaxTree
{
    internal SyntaxTree(
        SourceText sourceText,
        CompilationUnitSyntax root,
        IReadOnlyList<ParseDiagnostic> diagnostics)
    {
        SourceText = sourceText;
        Root = root;
        Diagnostics = diagnostics;
    }

    public SourceText SourceText { get; }

    public CompilationUnitSyntax Root { get; }

    public IReadOnlyList<ParseDiagnostic> Diagnostics { get; }

    /// <summary>Parses <paramref name="sourceText"/> into a <see cref="CompilationUnitSyntax"/>.</summary>
    public static SyntaxTree Parse(SourceText sourceText)
    {
        ArgumentNullException.ThrowIfNull(sourceText);
        var tokens = Lexer.Tokenize(sourceText);
        var parser = new ParserCore(tokens, sourceText);
        return parser.Parse();
    }
}
