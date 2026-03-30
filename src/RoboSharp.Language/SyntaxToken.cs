namespace RoboSharp.Language;

public sealed record SyntaxToken(
    SyntaxKind Kind,
    TextSpan Span,
    string Text,
    object? Value,
    IReadOnlyList<SyntaxTrivia> LeadingTrivia,
    IReadOnlyList<SyntaxTrivia> TrailingTrivia)
{
    public SyntaxToken(SyntaxKind kind, TextSpan span, string text, object? value = null)
        : this(kind, span, text, value, Array.Empty<SyntaxTrivia>(), Array.Empty<SyntaxTrivia>())
    {
    }
}
