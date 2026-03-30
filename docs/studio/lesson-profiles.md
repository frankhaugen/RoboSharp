# Lesson-aware Studio behavior

Studio should use **lesson metadata** end to end so the teaching layer is visible, not bolted on. The full data model lives under [../lessons/README.md](../lessons/README.md).

## Lesson metadata

The Studio should understand:

- lesson title and description
- active built-in **profile** (for semantic analysis and autocomplete)
- world file to load for the run
- **goals** (required and optional) and the last evaluation result
- structured **help** and **hints**
- **UI policy** (which inspection panels are shown, pause-at-entry, debugging allowed)

## Binding and tooling

- Choose the builtin profile for semantic analysis from the active lesson.
- Load the world referenced by the lesson before run/debug.
- After run or debug stop, run **goal evaluation** and surface results in the goal panel.

## Lesson-aware help

Show only the current lesson’s available built-ins and concepts (from profile + lesson help). Do not flood early learners with the full language surface.

Reveal **hints** progressively using their level (gentle → strong → near solution).

## Panel visibility

Respect `LessonUiPolicy`: hide advanced compiler/runtime panels for early lessons; expose syntax tree, IL, metrics, etc. when the lesson is about those topics.

## Goal panel

Should display:

- current objectives and check list (what was required vs what passed)
- completion state and messages (info / warning / error / success)
- optional hints and stretch goals

See [`AGENTS.md`](../../AGENTS.md) (built-ins and lesson rules).
