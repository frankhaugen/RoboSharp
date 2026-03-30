# Mission — what RoboSharp is for

RoboSharp is an **educational programming environment**: it teaches how programming languages and runtimes work by making every stage of the pipeline visible. Policy detail remains in [`AGENTS.md`](../../AGENTS.md); this page states **product intent** in one place.

## 1. Teach how languages actually work

The system is a **didactic language + runtime**. Students do not only run code—they see how it becomes execution.

The pipeline is explicit:

```text
Source code
  → Syntax tree
  → Fake IL
  → Interpreter execution
```

Parsing, trees, compilation to a teaching IL, and runtime execution are all inspectable so learners can relate this to how languages like C# are implemented (simplified).

## 2. A simplified C#-inspired surface

The language feels familiar to C# users but drops heavy features: no classes, generics, access modifiers, rich type system, or large standard library. It keeps functions, parameters, basic types, arrays, control flow, and a small built-in set so cognitive load stays low while structure stays real.

## 3. Robot world as the execution environment

Programs drive a **virtual grid**. Typical calls include `move()`, `turnLeft()`, `isFrontClear()`. The world lives in memory and is **separate from rendering**: the runtime mutates state; UIs show **snapshots**. Layered grids (terrain, items, actors) keep semantics clear and extensible.

## 4. Learning through simulation

Lessons use concrete goals (reach a goal, avoid obstacles, collect items, improve paths). Visual feedback is immediate. The platform can support analysis (e.g. path quality, wasted steps, turns) for smarter feedback.

## 5. Observable runtime state

Execution is **inspectable**: stack, variables, allocations, world state, steps, metrics (moves, turns). The runtime favors **immutable snapshots** for hosts instead of exposing mutable engine guts, preserving determinism and teaching clarity.

## 6. Fake compilation mirroring real toolchains

Sources and projects map conceptually to `.robo` / `.robosharp`; output maps to a **fake executable** (`.roboexe`). That parallels `C# → IL → runtime` in a form students can open and trace.

## 7. A teaching IDE (Studio)

The desktop host (**Studio**) is built for the pipeline: editor, inspection panes, world view, metrics, logs—**without** folding those concerns into the compiler or world cores. See [`../studio/README.md`](../studio/README.md).

## 8. Architecture as curriculum

Major subsystems mirror professional stacks (language, compiler, runtime, world, IO/workspace, hosts) so the **shape of the repo** is itself teachable—see [`../architecture.md`](../architecture.md).

## 9. Capability profiles for lessons

The runtime can load **profiles** that restrict which built-ins exist, enabling staged lessons (e.g. only `move`/`turnLeft` first, then sensing). See [`../lessons/builtin-profiles.md`](../lessons/builtin-profiles.md) and [`../semantics/builtins-and-profiles.md`](../semantics/builtins-and-profiles.md).

## 10. Technology-agnostic presentation

World state is projected for **ASCII, sprites, web, or desktop** through adapters—not inside the interpreter. Rendering is a consumer of snapshots.

## 11. Extensibility

Layered world and runtime abstractions allow future work (richer mechanics, scoring, analytics, extra syntax layers) without collapsing boundaries.

## In one sentence

RoboSharp is a **teaching platform that exposes how languages work** by letting students write small C#-like programs that control a robot on a grid while **inspecting each stage** from source through IL to runtime and world state.

## Where this connects

- [Design principles](design-principles.md) — non-negotiable pipeline boundaries
- [Pipeline boundaries (technical)](../architecture/pipeline-boundaries.md) — who owns syntax, semantics, IL, world, built-ins
- [Lessons and content](../lessons/README.md)
- [Implementation gaps vs `src/`](../implementation-gaps.md)
