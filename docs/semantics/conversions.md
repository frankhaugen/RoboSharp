# Conversion rules

Keep assignability simple.

**Allowed:**

- exact type match
- `integer` → `number`

**Disallowed:**

- `number` → `integer`
- arbitrary primitive mixing
- mismatched array element assignment
- truthiness conversions

## Seam

```csharp
public interface ITypeConversionService
{
    bool CanAssign(TypeSymbol target, TypeSymbol source);
}
```

A good seam even if the implementation is small.

Types: [type-system.md](type-system.md).
