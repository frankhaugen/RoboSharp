# Token model

Recommended token shape:

```csharp
public sealed record SyntaxToken(
    SyntaxKind Kind,
    TextSpan Span,
    string Text,
    object? Value,
    IReadOnlyList<SyntaxTrivia> LeadingTrivia,
    IReadOnlyList<SyntaxTrivia> TrailingTrivia);
```

Recommended trivia shape:

```csharp
public sealed record SyntaxTrivia(
    SyntaxKind Kind,
    TextSpan Span,
    string Text);
```

## Notes

- keep `Value` for parsed literal values
- keep `Text` too — inspectability matters
- leading/trailing trivia supports syntax coloring and faithful `.roboast.json` output

Kinds: [syntax-kinds-and-facts.md](syntax-kinds-and-facts.md). Lexer: [lexer.md](lexer.md).
