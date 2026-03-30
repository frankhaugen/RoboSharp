# Source text model

Recommended core types:

```csharp
public sealed class SourceText
{
    public string Text { get; }
    public IReadOnlyList<TextLine> Lines { get; }
}
```

```csharp
public readonly record struct TextLine(
    int Start,
    int Length,
    int LineNumber);
```

```csharp
public readonly record struct TextSpan(
    int Start,
    int Length)
{
    public int End => Start + Length;
}
```

```csharp
public readonly record struct LinePosition(
    int Line,
    int Column);
```

```csharp
public readonly record struct LinePositionSpan(
    LinePosition Start,
    LinePosition End);
```

## Rules

- all syntax nodes and diagnostics should carry spans
- source mapping must be cheap
- lexer and parser should consume `SourceText`, not just raw strings

This supports Studio and artifact serialization.
