# Suggested implementation order

RoboSharp is built as a **visible pipeline**. When growing the codebase, prefer vertical slices that keep each stage inspectable before adding large surface area elsewhere.

This is **guidance**, not a strict roadmap; [implementation-gaps.md](../implementation-gaps.md) tracks concrete code status.

## 1. Language surface

Tokens, lexer, parser, syntax tree, parse diagnostics — enough grammar to write small programs. No execution yet.

## 2. Semantics

Symbols, scopes, binding, bound tree, semantic diagnostics. Profile-aware builtins at least for a single “full” profile.

## 3. IL and lowering

`RoboProgram`, opcodes, `IlLowerer`, deterministic lowering from bound trees. Emit JSON IL artifacts for teaching.

## 4. Runtime

Interpreter, `ExecutionResult` / faults, stdout/stderr, stepping hooks. World integration for movement builtins.

## 5. Toolchain and workspace

Project load, compile orchestration, `obj`/`bin` layout, `.roboexe` emit. In-memory and physical IO paths share behavior where practical.

## 6. Application and hosts

Thin `RoboSharp.Application` facades, then Studio / Player / Web hosts over the same services. Debugger UI last among host features that depend on stable snapshots.

## 7. Lessons engine (later)

JSON lesson packs, goal evaluation, and session metrics once the run/debug loop is solid ([../lessons/README.md](../lessons/README.md)).

## Related

- [mission.md](mission.md)
- [design-principles.md](design-principles.md)
- [../documentation-todo.md](../documentation-todo.md) — doc authoring order
