# Symbol model and scopes

## Symbol families

- type symbol
- variable symbol
- parameter symbol
- function symbol

## Recommended types

```csharp
public abstract record Symbol(string Name);

public sealed record VariableSymbol(string Name, TypeSymbol Type) : Symbol(Name);

public sealed record ParameterSymbol(string Name, TypeSymbol Type) : Symbol(Name);

public sealed record FunctionSymbol(
    string Name,
    TypeSymbol ReturnType,
    IReadOnlyList<ParameterSymbol> Parameters,
    FunctionKind Kind,
    BuiltinId? BuiltinId = null) : Symbol(Name);
```

```csharp
public enum FunctionKind
{
    UserDefined,
    BuiltIn,
    Synthetic
}
```

## Rules

- no overloads in v1
- names unique per scope
- built-ins cannot be shadowed by user code in v1 (important for lessons)

## Scopes

Lexical scoping only. Recommended scopes:

- global
- function
- block

```csharp
public sealed class Scope
{
    public Scope? Parent { get; }
    public Dictionary<string, VariableSymbol> Variables { get; }
    public Dictionary<string, FunctionSymbol> Functions { get; }
}
```

Enough for v1.
