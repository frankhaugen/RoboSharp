# Built-in profiles (teaching layer)

Built-ins are defined **once** in the language/semantics pipeline. **Profiles** select which of those functions are available for a given lesson or host context.

Do **not** duplicate built-in definitions per lesson.

Canonical ids and semantic shapes: [../semantics/builtins-and-profiles.md](../semantics/builtins-and-profiles.md) and the human catalog [../language/built-in-functions.md](../language/built-in-functions.md).

## Built-in definitions (reminder)

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

Stable `BuiltinId` bridges semantic binding, IL lowering, runtime dispatch, lesson gating, and help/autocomplete.

## Profile model

```csharp
public sealed record BuiltinFunctionProfile(
    string Name,
    string DisplayName,
    string Description,
    IReadOnlyDictionary<string, BuiltinFunctionDefinition> Functions);
```

Rules:

- keyed by function **name** for semantic lookup
- names unique within a profile
- profile entries reference canonical definitions only
- profile does **not** embed lesson goals or world data

## Profile provider

```csharp
public interface IBuiltinProfileProvider
{
    BuiltinFunctionProfile GetProfile(string profileName);
    IReadOnlyList<BuiltinFunctionProfile> GetAllProfiles();
}
```

Prefer **code-first** definitions in v1: not database-driven, not admin-configured at runtime.

## Suggested first profiles

### `BasicMovement`

- `move()`
- `turnLeft()`

### `MovementAndSensing`

- `move()`, `turnLeft()`, `frontIsClear()`

### `MovementAndOutput`

- `move()`, `turnLeft()`, `frontIsClear()`, `print(value)`

### `RobotCore`

- `move()`, `turnLeft()`, `turnRight()`, `pick()`, `drop()`
- `frontIsClear()`, `leftIsClear()`, `rightIsClear()`, `print(value)`

### `CollectionsIntro`

- `print(value)`, `count(array)`, `add(array, item)`, `getLast(array)`, `takeLast(array)`

### `FullV1`

All v1 built-ins.

That set is enough for a first content sequence.
