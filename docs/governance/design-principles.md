# Design principles (pipeline integrity)

These principles complement [`AGENTS.md`](../../AGENTS.md) and [mission](mission.md). They exist so **syntax, semantics, IL, world, and built-ins stay composable** and teachable.

## Lock the pipeline stages together

Specifying syntax, IL, and world **in isolation** risks:

- syntax that does not lower cleanly
- IL too abstract to drive the world
- world operations leaking into parsing or binding

The intended separation of concerns:

1. **Syntax** — what the learner may write (grammar, recovery).
2. **Semantic / bound form** — what it means (symbols, types, profile-gated built-ins).
3. **IL** — the executable model (opcodes, stack/slots, calls).
4. **World API** — what side effects are possible on `RobotWorld`.
5. **Built-ins** — the bridge from IL dispatch to world / IO behavior.

End-to-end flow:

```text
Source → syntax tree → semantic analysis → bound tree
→ IL → interpreter → built-in handlers → RobotWorld
→ snapshots / render projection
```

## Parser neutrality

The parser treats `move()`, `turnLeft()`, `print(x)` as **ordinary call expressions**. It does not encode lesson profiles or world rules. **Availability** of built-ins is a **semantic** concern; **dispatch** is **runtime**. See [Pipeline boundaries](../architecture/pipeline-boundaries.md).

## Observability and determinism

Prefer **snapshots** and explicit state machines over hidden mutation for anything hosts or debuggers display. Keep the teaching story honest: what you see in the UI matches a defined intermediate representation.

## Dependency direction

Hosts and application layers depend inward; **Language** does not depend on World, Studio, or IO. See [Architecture overview](../architecture.md) and [dependency rules](../architecture/dependency-rules.md) when filled out.

## Related

- [Mission](mission.md)
- [Pipeline boundaries](../architecture/pipeline-boundaries.md)
- [Syntax-to-IL lowering](../compiler/syntax-to-il-lowering.md) (bridge examples)
