# Content packs, lesson sessions, and metrics

## Content pack purpose

A **content pack** is a distributable set of:

- lessons
- worlds
- optional assets (images, extra docs)
- optional sample solutions

This is how RoboSharp stays extensible without rewriting the host app.

## Content pack model

```csharp
public sealed record ContentPackDefinition(
    string Id,
    string Title,
    string Description,
    string Version,
    IReadOnlyList<LessonDefinition> Lessons,
    IReadOnlyList<string> WorldFiles);
```

Later extensions might add localized strings, teacher notes, or reference solutions. For v1, keep the model boring.

## Providers

```csharp
public interface IContentPackProvider
{
    IReadOnlyList<ContentPackDefinition> GetAvailablePacks();
    ContentPackDefinition GetPack(string packId);
}
```

A lesson-oriented facade can sit on top:

```csharp
public interface ILessonProvider
{
    LessonDefinition GetLesson(string lessonId);
    IReadOnlyList<LessonDefinition> GetLessons();
}
```

## File-based storage (v1)

Example layout:

```text
content/
  intro/
    pack.json
    lessons/
      lesson-01.json
      lesson-02.json
    worlds/
      intro-maze.world.json
      avoid-wall.world.json
    examples/
      lesson-01-example-01.robo
```

Do not move this into a database for v1.

## Lesson session purpose

A **lesson session** binds:

- one lesson definition
- one workspace / program attempt
- one runtime or debug session
- the latest goal evaluation (and run artifacts)

It is the bridge object Studio and Player care about during an attempt.

## Lesson session model

Illustrative shape (type names indicative):

```csharp
public sealed class LessonSession
{
    public required LessonDefinition Lesson { get; init; }
    public required IRoboWorkspace Workspace { get; init; }

    public GoalEvaluationResult? LastEvaluation { get; set; }
    public RunResult? LastRunResult { get; set; }
    public RuntimeSnapshot? LastSnapshot { get; set; }
    public LessonMetrics? LastMetrics { get; set; }
}
```

Keep **long-term progress persistence** separate; this object tracks the current working attempt.

## Lesson metrics

A compact summary of execution facts for goals and feedback.

```csharp
public sealed record LessonMetrics(
    int InstructionsExecuted,
    int ArraysAllocated,
    int MaxStackDepth,
    int StdOutLineCount,
    int StdErrLineCount,
    IReadOnlyDictionary<int, ActorMetricsSnapshot> ActorMetrics);
```

```csharp
public sealed record ActorMetricsSnapshot(
    int ActorId,
    int StepsMoved,
    int TurnsMade,
    int FailedMoveAttempts,
    int UniqueVisitedTiles,
    int TotalVisitedTiles);
```

That enables feedback such as: reached the goal; used 19 steps; shortest known route 11; attempted to move into a wall 3 times.

See also: [json-formats.md](json-formats.md), [../studio/lesson-profiles.md](../studio/lesson-profiles.md), [../player/README.md](../player/README.md).
