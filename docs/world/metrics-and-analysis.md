# Metrics and analysis

## Derived matrices (non-authoritative)

The world model supports **derived analysis**, but results must **not** be stored as authoritative world state.

Distinction:

- **Terrain matrix** — base world definition.
- **Item / actor matrices** — live tactical state.
- **Derived matrices** — computed on demand.

Examples:

- walkability
- distance-to-goal
- visit heatmap
- cost map

Do not mix analytics into the world core types.

## Actor telemetry

Seed shape for per-actor metrics:

```csharp
public sealed class ActorMetrics
{
    public int StepsMoved { get; set; }
    public int TurnsMade { get; set; }
    public int FailedMoveAttempts { get; set; }

    public HashSet<GridPosition> VisitedPositions { get; } = [];
    public List<GridPosition> RouteHistory { get; } = [];
}
```

Attach this to actor runtime state or keep a parallel metrics store keyed by actor id. Use cases include route efficiency scoring, revisit detection, lesson feedback, and comparison against an optimal route.

See [World model](world-model.md) for snapshots and layering.
