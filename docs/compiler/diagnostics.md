# Compiler diagnostics (cross-phase)

RoboSharp surfaces problems as **structured diagnostics** with severity and source location, not as exceptions in normal compiler flow. This page ties together **where** diagnostics are produced and **how** hosts should treat them.

## Layers

| Layer | Typical diagnostics | Stops IL / emit? |
| ----- | --------------------- | ---------------- |
| **Lexer** | Bad characters, unterminated literals | No; lexing continues when possible |
| **Parser** | Unexpected token, malformed construct | No; parser recovers |
| **Binder / semantics** | Unknown name, type mismatch, bad call arity | **Yes** if any error severity after binding |
| **Toolchain** | Missing file, invalid project | Yes before compile |

Parse-time messages are owned by `RoboSharp.Language` (alongside the parser). Semantic messages are owned by `RoboSharp.Semantics`. See [../semantics/diagnostics.md](../semantics/diagnostics.md) for message style and categories.

## Severity

- **Error:** Compilation must not emit a “successful” executable. IL generation and packaging are skipped when errors remain after semantic analysis ([v1-compiler-spec.md](v1-compiler-spec.md)).
- **Warning:** May still produce `.roboexe`; host shows warnings in the diagnostics surface.

## Shape (conceptual)

Diagnostics should carry at least:

- stable id or code (for tooling)
- severity
- message text
- primary source span (file + range)

Exact types live in the Language / Semantics public APIs.

## Runtime vs compiler

**Compiler diagnostics** are distinct from **runtime stderr**: the latter is produced while executing IL (builtins, soft failures). Do not conflate parse/bind errors with `StdErr` lines from the interpreter ([../runtime/standard-output.md](../runtime/standard-output.md), [../runtime/error-handling.md](../runtime/error-handling.md)).

## Related

- [semantic-analysis.md](semantic-analysis.md)
- [compilation-pipeline.md](compilation-pipeline.md)
- [v1-compiler-spec.md](v1-compiler-spec.md) — failure boundary
