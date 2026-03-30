# Metrics view (routes and analysis)

The debugger / Studio **metrics** (or “route”) view connects **runtime progress** to **world analysis** helpers: it is an adapter over world and session data, not part of the core interpreter loop.

## Purpose

- Show paths, distances, or lesson-specific scores derived from world state and execution history.
- Help learners see *why* a program succeeded or failed relative to lesson goals ([../lessons/goals-and-evaluation.md](../lessons/goals-and-evaluation.md)).

## Data sources

Conceptual inputs (see [../world/metrics-and-analysis.md](../world/metrics-and-analysis.md)):

- Latest world snapshot from the paused or completed run
- Optional trace of robot moves / turns if the host records them
- Lesson metadata (goal definitions) from workspace / content packs

The world layer owns **pure** analysis functions over snapshots; the debugger host owns **when** to call them and how to format results.

## Non-goals (v1)

- The metrics view is not a second execution engine.
- It must not introduce nondeterministic scoring unless the lesson spec explicitly defines randomness (v1 world/runtime stay deterministic by default).

## Related

- [debugger-architecture.md](debugger-architecture.md) — “route/metrics pane” sync on pause
- [../studio/visualization.md](../studio/visualization.md)
- [../world/metrics-and-analysis.md](../world/metrics-and-analysis.md)
