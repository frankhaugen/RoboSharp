# Built-ins and profiles

## Built-ins are not syntax

- built-ins are normal functions semantically
- built-in **availability** is profile-based
- parser does not special-case them

### Language project

Defines built-in ids, built-in signatures, and helpers to create builtin symbols ([../language/built-in-functions.md](../language/built-in-functions.md) catalog).

### Profile / host layer

A lesson/profile decides which built-ins are active. Grammar stays stable.

## Definitions

```csharp
public sealed record ParameterDefinition(
    string Name,
    TypeSymbol Type);

public sealed record BuiltinFunctionDefinition(
    BuiltinId Id,
    string Name,
    TypeSymbol ReturnType,
    IReadOnlyList<ParameterDefinition> Parameters);
```

```csharp
public enum BuiltinId
{
    Move,
    TurnLeft,
    TurnRight,
    Pick,
    Drop,
    FrontIsClear,
    LeftIsClear,
    RightIsClear,
    Print,
    Count,
    Add,
    GetLast,
    TakeLast
}
```

Human-readable list: [../language/built-in-functions.md](../language/built-in-functions.md).

## Profiles (teaching layer)

Semantics cares that binding respects **which** built-ins exist for the active profile. Naming, catalog of starter profiles, and `IBuiltinProfileProvider` are specified with the lesson system:

- [../lessons/builtin-profiles.md](../lessons/builtin-profiles.md)

At a glance, a profile is a named bundle of references to canonical definitions:

```csharp
public sealed record BuiltinFunctionProfile(
    string Name,
    string DisplayName,
    string Description,
    IReadOnlyDictionary<string, BuiltinFunctionDefinition> Functions);
```
