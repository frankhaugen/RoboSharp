# Lesson definition: hints, help, UI policy, progression

## Hints

Hints should be **structured**, not a single opaque blob, so Studio can reveal them progressively.

```csharp
public sealed record LessonHint(
    string Id,
    string Title,
    string Content,
    HintLevel Level);
```

```csharp
public enum HintLevel
{
    Gentle,
    Strong,
    NearSolution
}
```

## Help (separate from hints)

Compact “what exists here” surface:

- allowed built-ins (often implied by profile)
- target concepts
- one or two short code examples

```csharp
public sealed record LessonHelp(
    IReadOnlyList<string> Concepts,
    IReadOnlyList<CodeExample> Examples,
    IReadOnlyList<BuiltinId> HighlightedBuiltins);
```

```csharp
public sealed record CodeExample(
    string Title,
    string Code);
```

**Hints** answer “help me solve this.” **Help** answers “what tools and ideas are in play.”

## Lesson shape (main record)

```csharp
public sealed record LessonDefinition(
    string Id,
    string Title,
    string Description,
    string BuiltinProfileName,
    string WorldFile,
    LessonGoalSet Goals,
    LessonHelp Help,
    IReadOnlyList<LessonHint> Hints,
    LessonUiPolicy UiPolicy,
    LessonProgression? Progression = null);
```

This is the center of the educational layer for authoring and tooling.

## Lesson UI policy

UX should be driven by data, not hard-coded shell behavior.

```csharp
public sealed record LessonUiPolicy(
    bool ShowTokensPanel,
    bool ShowSyntaxTreePanel,
    bool ShowBoundTreePanel,
    bool ShowIlPanel,
    bool ShowWorldPanel,
    bool ShowOutputPanel,
    bool ShowMetricsPanel,
    bool ShowGoalPanel,
    bool PauseAtEntryByDefault,
    bool AllowDebugging);
```

Examples:

- **Early lesson**: hide AST, bound tree, IL, metrics; show world, output, goals.
- **Compiler-focused lesson**: show syntax tree and IL.
- **Runtime lesson**: emphasize world, stdout/stderr, metrics, debugging.

## Progression

Keep linking simple for v1.

```csharp
public sealed record LessonProgression(
    string? NextLessonId,
    string? PreviousLessonId,
    IReadOnlyList<string> PrerequisiteLessonIds);
```

Avoid a heavy curriculum engine until the simple model is proven.

See also: [goals-and-evaluation.md](goals-and-evaluation.md), [json-formats.md](json-formats.md).
