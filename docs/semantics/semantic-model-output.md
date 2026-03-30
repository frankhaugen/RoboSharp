# Semantic output

Recommended model:

```csharp
public sealed record SemanticModel(
    CompilationUnitSyntax Syntax,
    BoundProgram? Program,
    IReadOnlyList<Diagnostic> Diagnostics,
    Scope GlobalScope);
```

If errors exist, IL generation does not proceed.

`CompilationUnitSyntax` comes from `RoboSharp.Language`. `BoundProgram` and binding details are `RoboSharp.Semantics`.
