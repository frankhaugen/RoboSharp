using RoboSharp.Language;

namespace RoboSharp.Semantics;

public sealed class SemanticDiagnostic
{
    public SemanticDiagnostic(TextSpan span, string message)
    {
        Span = span;
        Message = message;
    }

    public TextSpan Span { get; }
    public string Message { get; }
}
