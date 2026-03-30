# Type system

Keep the type system tiny.

## Canonical symbols

```csharp
public abstract record TypeSymbol(string Name);

public sealed record PrimitiveTypeSymbol(string TypeName) : TypeSymbol(TypeName);

public sealed record ArrayTypeSymbol(TypeSymbol ElementType)
    : TypeSymbol($"{ElementType.Name}[]");
```

Well-known instances:

- `integer`
- `number`
- `string`
- `bool`

Single-dimensional `T[]`.

## Rules

- no user-defined types
- no nested arrays in v1 unless explicitly allowed later
- no nullable type system
- no implicit `object`

Surface syntax for types: [../language/types.md](../language/types.md).
