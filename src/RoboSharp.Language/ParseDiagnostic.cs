namespace RoboSharp.Language;

/// <summary>
/// A parse-time diagnostic (syntax errors and recovery hints).
/// </summary>
public sealed record ParseDiagnostic(TextSpan Span, string Message);
