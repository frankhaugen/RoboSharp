# Standard output and standard error

The runtime exposes two **text streams** for teaching visibility:

- **Stdout** — intentional program output (e.g. `print` builtin).
- **Stderr** — runtime warnings, soft failures, and diagnostic lines from builtins.

## Writers

Hosts pass `TextWriter` instances (or adapters) when running the interpreter. Console and Studio panels attach different sinks to the same contract.

## Line metadata (normative direction)

[v1-runtime-spec.md](v1-runtime-spec.md) §12 calls for associating output lines with **instruction pointer metadata** so the UI can scroll to the emitting site. Whether each line stores `(functionIndex, ip)` as structured fields or as a prefix string is an implementation detail; the teaching goal is **correlation**, not POSIX compliance.

## Separation from compiler diagnostics

Messages emitted **during compilation** are `Diagnostic` objects in the Language/Semantics layers. **Stderr** is **runtime-only** and must not be overloaded for parse errors.

## Related

- [error-handling.md](error-handling.md)
- [v1-runtime-spec.md](v1-runtime-spec.md) §12
- [../studio/output-and-state-panels.md](../studio/output-and-state-panels.md)
