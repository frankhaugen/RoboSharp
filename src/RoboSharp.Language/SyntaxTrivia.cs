namespace RoboSharp.Language;

public sealed record SyntaxTrivia(SyntaxKind Kind, TextSpan Span, string Text);
