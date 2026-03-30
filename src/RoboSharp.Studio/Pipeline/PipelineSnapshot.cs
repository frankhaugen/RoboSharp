using RoboSharp.Language;

namespace RoboSharp.Studio.Pipeline;

/// <summary>
/// Immutable slice of the compiler front-end for Studio panels (lexer → parser).
/// </summary>
public sealed record PipelineSnapshot(
    string Source,
    IReadOnlyList<SyntaxToken> Tokens,
    SyntaxTree SyntaxTree,
    IReadOnlyList<ParseDiagnostic> Diagnostics);
