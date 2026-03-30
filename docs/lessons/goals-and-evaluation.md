# Goals and evaluation

## Core rule

A lesson is not passed only because “the program ran.” It passes when a **goal evaluator** says so, using explicit, testable rules over finished attempts.

Goals may consider:

- final world state
- runtime status (success vs fault)
- stdout/stderr
- metrics (steps, failed moves, route quality)
- optional **source-shape** constraints (see below)

## Goal categories

### World-state goals

Based on final world, actor, and item state.

### Output goals

Based on stdout/stderr content or patterns.

### Metrics goals

Based on route efficiency, failed moves, instruction count, etc.

### Source-shape goals

Optional checks over syntax/bound model (not raw text).

### Composite goals

Logical combination of smaller goals.

## Evaluator contract

```csharp
public interface IGoalEvaluator
{
    GoalEvaluationResult Evaluate(GoalEvaluationContext context);
}
```

```csharp
public sealed record GoalEvaluationContext(
    LessonDefinition Lesson,
    RuntimeSnapshot FinalSnapshot,
    RunResult RunResult,
    SourceAnalysisSnapshot SourceAnalysis,
    LessonMetrics Metrics);
```

```csharp
public sealed record GoalEvaluationResult(
    bool IsSuccess,
    IReadOnlyList<GoalMessage> Messages,
    IReadOnlyList<GoalCheckResult> Checks);
```

## Result model

```csharp
public sealed record GoalMessage(
    GoalMessageSeverity Severity,
    string Message);
```

```csharp
public enum GoalMessageSeverity
{
    Info,
    Warning,
    Error,
    Success
}
```

```csharp
public sealed record GoalCheckResult(
    string CheckId,
    string Title,
    bool Passed,
    string? Message = null);
```

Studio can show what was required, what passed, what failed, and why.

## Recommended first goal types

### `ReachGoalTileGoal`

Primary actor ends on a goal tile.

```csharp
public sealed record ReachGoalTileGoal(
    int ActorId) : ILessonGoal;
```

### `EndAtPositionGoal`

Actor ends at an exact coordinate (optional facing).

```csharp
public sealed record EndAtPositionGoal(
    int ActorId,
    GridPosition Position,
    Direction? Direction = null) : ILessonGoal;
```

### `PickupItemGoal`

Actor collects items of a kind.

```csharp
public sealed record PickupItemGoal(
    int ActorId,
    ItemCellKind ItemKind,
    int RequiredCount) : ILessonGoal;
```

### `StdOutContainsGoal`

Program output contains expected text.

```csharp
public sealed record StdOutContainsGoal(
    string ExpectedText) : ILessonGoal;
```

### `MaxStepsGoal`

Stay under a step (or instruction) limit.

```csharp
public sealed record MaxStepsGoal(
    int MaxSteps) : ILessonGoal;
```

### `NoRuntimeErrorsGoal`

No hard runtime fault; optionally disallow stderr warnings.

```csharp
public sealed record NoRuntimeErrorsGoal(
    bool AllowWarnings = true) : ILessonGoal;
```

### `CompositeGoal`

```csharp
public sealed record CompositeGoal(
    IReadOnlyList<ILessonGoal> Goals,
    CompositeGoalMode Mode);
```

```csharp
public enum CompositeGoalMode
{
    All,
    Any
}
```

## Semantics

Goals must **not** mutate anything. They inspect a finished attempt so they stay deterministic, testable, reusable, and re-runnable for explanations.

## Hard vs soft goals

- **Hard goals**: required for completion.
- **Soft goals**: stretch targets (e.g. “use at most 12 steps”).

Example: hard = reach the goal; soft = avoid failed move attempts.

```csharp
public sealed record LessonGoalSet(
    IReadOnlyList<ILessonGoal> RequiredGoals,
    IReadOnlyList<ILessonGoal> OptionalGoals);
```

## Source-shape goals

Use sparingly. Prefer world-state goals unless the pedagogy is explicitly about program structure.

Examples:

- must use `while`
- must not call `turnRight`
- must declare an array
- must call `print`
- must define a function

Inspect syntax/bound tree, not raw text.

```csharp
public sealed record MustUseSyntaxKindGoal(
    SyntaxKind Kind) : ILessonGoal;

public sealed record MustCallFunctionGoal(
    string FunctionName) : ILessonGoal;

public sealed record MustNotCallFunctionGoal(
    string FunctionName) : ILessonGoal;
```

Do not turn every lesson into “match the teacher’s style.” Use these when structure **is** the learning objective.

Types such as `LessonDefinition`, `LessonMetrics`, and `ILessonGoal` are illustrative; names may evolve in code.
