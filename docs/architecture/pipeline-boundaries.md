# Pipeline boundaries — who owns what

This document **freezes responsibility** between syntax, semantics, IL, world, and built-ins so implementations stay aligned. It generalizes ideas from the teaching mission; see [governance/mission.md](../governance/mission.md).

## End-to-end interaction model

```text
Syntax
  ↓
Bound semantic form
  ↓
IL lowering
  ↓
Interpreter dispatch
  ↓
Built-in runtime handler (or user function body)
  ↓
RobotWorld mutation (when applicable)
  ↓
Snapshot / render projection
```

The interpreter is a **plain C# state machine** over fake IL, not CLR emission. Built-ins run as handlers against execution state and world contracts.

## The key rule: syntax does not talk to the world

Syntax expresses declarations, statements, expressions, and **calls**. The parser does **not** know about lessons, profiles, or grid state.

- `move()` parses as a normal **call expression**.
- **Semantic analysis** decides whether `move` resolves to a user function or a profile **built-in**.
- **IL generation** emits **`Call`** for user functions or **`CallBuiltin`** for built-ins (see below).
- **Runtime** dispatches to the handler, which may mutate **`RobotWorld`**.
- **UI** sees **snapshots**, not live mutable engine internals.

## Freeze: `Call` vs `CallBuiltin`

| Callee kind | IL form |
| ----------- | ------- |
| User-defined function | `Call` (or equivalent direct call to function metadata) |
| Built-in from active profile | `CallBuiltin` (operand identifies built-in) |

Robot commands stay **ordinary calls** in source—no special parser syntax for `move` or `turnLeft`.

Examples in source (all parse as calls):

```text
move()
turnLeft()
frontIsClear()
print("x")
count(values)
takeLast(values)
```

Binding chooses user vs built-in; lowering chooses `Call` vs `CallBuiltin`.

## What each layer owns

### Syntax spec

- Grammar, precedence, statement/expression kinds, type syntax, parse recovery.

### Semantic spec

- Symbol resolution, built-in **availability** (profile), type checking, assignability, legality of calls/index/return.

### IL spec

- Opcode inventory, operand shapes, calling convention, evaluation stack / locals, function metadata, debug mapping hooks, meaning of **`CallBuiltin`**.

### World spec

- Grids/layers, actor state, movement / item / terrain rules, metrics hooks, **snapshot** format.

### Built-in spec

- Built-in ids, signatures, semantic meaning, **runtime handler** behavior (return value, world mutation, stdout/stderr).

## IL and world: explicit non-goals

- **IL never renders** — it only advances execution state.
- **IL does not inspect syntax** — source is gone after lowering except via debug metadata.
- **World mutations** go through **runtime handlers**, not parser or syntax visitors.
- **Snapshots are the UI contract** — hosts consume runtime/world snapshots, not ad hoc shared mutable graphs.

## World interaction (spec direction)

The interpreter affects the world only through **defined APIs** (e.g. `IRobotWorld` / built-in handlers). Layered storage (`TerrainGrid`, `ItemGrid`, `ActorGrid`, `ActorsById`) stays authoritative; rendering uses **projection** over snapshots. Details: [world model](../world/world-model.md), [world actions](../world/world-actions.md), [movement rules](../world/movement-rules.md), [render projection](../rendering/render-projection.md).

## Related

- [Syntax-to-IL lowering](../compiler/syntax-to-il-lowering.md)
- [IL instruction set (inventory)](../runtime/il-instruction-set.md)
- [Built-ins and profiles](../semantics/builtins-and-profiles.md)
- [Compilation pipeline](../compiler/compilation-pipeline.md)
