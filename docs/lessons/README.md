# Lessons, goals, profiles, and content packs

This folder specifies the **educational layer** on top of the language, semantics, world, and hosts. It is cross-cutting: it consumes lower layers and does not redefine syntax, binding rules, or world simulation internals.

Authoritative repo rules remain in [`AGENTS.md`](../../AGENTS.md) (built-ins, profiles, lessons). If anything here disagrees with `AGENTS.md`, trust `AGENTS.md`.

## Why this layer exists

Without it, RoboSharp is mostly a visible compiler/runtime stack. With it, the product can answer:

- what the learner sees in a lesson
- which built-ins are available for this attempt
- which world loads
- what counts as success
- what hints and help are shown
- what panels are visible in Studio
- how lessons are ordered and linked

## Design goals

The lesson/content system should be:

- declarative and inspectable
- code-friendly and serializable
- easy to version and cheap to test
- not entangled with UI implementation details or parser/runtime internals

Core mental model:

- **profiles** gate capability (which built-ins exist for this lesson)
- **goals** evaluate outcomes (pass/fail and feedback)
- **lessons** compose profile + world + goals + help/hints + UI policy

## Big concepts

| Concept | Role |
| ------- | ---- |
| Built-in profile | Subset of globally defined built-ins available for a lesson |
| Lesson | Learner-facing unit of progression |
| Goal | Success/failure rules for a run |
| Hint | Structured “help me solve this” content |
| Help | “What tools/concepts exist here” (separate from hints) |
| World reference | Which world file backs the lesson |
| Content pack | Distributable bundle of lessons, worlds, and assets |
| Lesson session | One attempt: lesson + workspace + run + goal evaluation |

## Layering

Intended dependency direction:

```text
Language
  ↓
Built-in definitions (canonical)
  ↓
Profiles
  ↓
Goals
  ↓
Lessons
  ↓
Content packs
  ↓
Studio / Player / CLI
```

Rules of thumb:

- the parser does not know lessons
- the binder knows the **active built-in profile**
- the runtime knows the **active world**
- the goal evaluator inspects **finished** attempts (snapshots, output, metrics)
- Studio uses lesson metadata to shape UX

## Where the spec lives

| Topic | Document |
| ----- | -------- |
| Profile model, first profile names, provider seam | [builtin-profiles.md](builtin-profiles.md) |
| Goal families, evaluator contract, first goal types, source-shape goals | [goals-and-evaluation.md](goals-and-evaluation.md) |
| Hints, help, lesson record, UI policy, progression | [lesson-model.md](lesson-model.md) |
| Content packs, file layout, lesson sessions, metrics | [content-packs-sessions-and-metrics.md](content-packs-sessions-and-metrics.md) |
| JSON direction for lesson and pack files | [json-formats.md](json-formats.md) |
| Studio behavior | [../studio/lesson-profiles.md](../studio/lesson-profiles.md) |
| Player lesson mode | [../player/README.md](../player/README.md) |
| Workspace vs lesson responsibility | [../workspaces/lesson-metadata.md](../workspaces/lesson-metadata.md) |
| Built-in ids and semantic shape | [../semantics/builtins-and-profiles.md](../semantics/builtins-and-profiles.md), [../language/built-in-functions.md](../language/built-in-functions.md) |

## Packaging in `src/`

The draft suggested multiple projects (`RoboSharp.Builtins`, `RoboSharp.Lessons`, …) or a single `RoboSharp.Lessons` with nested concerns. The repository may start smaller (for example folding providers into `RoboSharp.Application` until boundaries harden). Keep the **concepts** stable even if project names shift.

## v1 choices to freeze

- built-ins defined once; profiles select subsets
- lessons reference profile by name and world by path
- goal evaluation after a run completes
- required vs optional goals
- structured help, hints, and UI policy on the lesson
- content packs are file-based (JSON), not a database

## Deliberately out of scope for v1

- adaptive difficulty, cloud accounts, procedural lessons
- teacher dashboards, leaderboards, AI-generated hints
- large curriculum graphs beyond simple next/previous/prerequisites

## Next spec after this

World file schema, lesson file schema, and validation rules should be nailed down so content authoring and tooling are implementation-ready. See [json-formats.md](json-formats.md) for direction.
