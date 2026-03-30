# Public API — Language project

Recommended seams for lexing and parsing:

```csharp
public interface ILexer
{
    IReadOnlyList<SyntaxToken> Lex(SourceText sourceText);
}
```

```csharp
public interface IParser
{
    SyntaxTree Parse(SourceText sourceText);
}
```

```csharp
public interface ISyntaxTreeSerializer
{
    string Serialize(CompilationUnitSyntax root);
}
```

For v1, these can be concrete-only if you want fewer interfaces; the boundary matters more than interface count.

Binding and semantic model: [../semantics/public-api.md](../semantics/public-api.md).
