# Public API — Semantics project

```csharp
public interface IBinder
{
    SemanticModel Bind(
        CompilationUnitSyntax syntax,
        BuiltinFunctionProfile builtinProfile);
}
```

```csharp
public interface IBoundTreeSerializer
{
    string Serialize(BoundProgram program);
}
```

Lex/parse: [../language/public-api.md](../language/public-api.md). For v1, concrete types are fine if you want fewer interfaces.
