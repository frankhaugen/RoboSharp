# RoboSharp.Semantics documentation

Meaning and analysis: symbols, scopes, binding, bound tree, conversions, semantic diagnostics, built-in **signatures** and profile **availability**.

Depends on [`RoboSharp.Language`](../language/README.md) for syntax. Does **not** own lexer/parser.

Authoritative policy: [`AGENTS.md`](../../AGENTS.md).

| Topic | Document |
| ----- | -------- |
| Pipeline and bound tree role | [overview.md](overview.md) |
| Type symbols and rules | [type-system.md](type-system.md) |
| Symbols and lexical scopes | [symbols-and-scopes.md](symbols-and-scopes.md) |
| Built-ins vs profiles | [builtins-and-profiles.md](builtins-and-profiles.md) |
| Binding architecture | [binding-and-bound-tree.md](binding-and-bound-tree.md) |
| Assignability / conversions | [conversions.md](conversions.md) |
| `if` / `while` conditions | [control-flow-and-conditions.md](control-flow-and-conditions.md) |
| Array typing checks | [arrays.md](arrays.md) |
| Operator typing | [operators.md](operators.md) |
| Diagnostic categories | [diagnostics.md](diagnostics.md) |
| `SemanticModel` output | [semantic-model-output.md](semantic-model-output.md) |
| Public API (bind side) | [public-api.md](public-api.md) |
| Suggested layout inside `RoboSharp.Semantics` | [project-layout.md](project-layout.md) |
| Non-goals and summary | [summary.md](summary.md) |
