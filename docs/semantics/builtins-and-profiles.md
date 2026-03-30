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

## IL lowering: user `Call` vs `CallBuiltin`

At IL generation time:

- **User-defined functions** lower to a normal **`Call`** (or equivalent) into function metadata the interpreter loads.
- **Built-ins** resolved under the active profile lower to **`CallBuiltin`**, with an operand that identifies the built-in (exact encoding in the IL spec).

Source always uses ordinary **call syntax** for both; the binder distinguishes user vs built-in. The runtime dispatches `CallBuiltin` to a handler that may mutate [`RobotWorld`](../world/world-model.md), write to stdout/stderr, or return a value—per built-in spec.

See [Pipeline boundaries](../architecture/pipeline-boundaries.md) and [Syntax-to-IL lowering](../compiler/syntax-to-il-lowering.md).
