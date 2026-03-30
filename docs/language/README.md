# RoboSharp.Language documentation

Syntax-only layer: tokens, lexer, parser, syntax tree, spans, parse diagnostics. **No** runtime, world, Studio UI, or workspace behavior.

**Companion:** [`docs/semantics/`](../semantics/README.md) (`RoboSharp.Semantics`) — binding, types, symbols, bound tree, semantic diagnostics.

Authoritative policy: [`AGENTS.md`](../../AGENTS.md).

| Topic | Document |
| ----- | -------- |
| Purpose, goals, two-project split, separation rules | [language-overview.md](language-overview.md) |
| v1 surface (keywords, types, statements, expressions) | [syntax.md](syntax.md) |
| Top-level program and functions | [functions.md](functions.md) |
| Statement kinds (reference) | [statements.md](statements.md) |
| Expression kinds (reference) | [expressions.md](expressions.md) |
| Array type and literals (syntax) | [arrays.md](arrays.md) |
| Built-in names (catalog; availability is semantic/profile) | [built-in-functions.md](built-in-functions.md) |
| `SourceText`, spans, lines | [source-model.md](source-model.md) |
| `SyntaxKind`, `SyntaxFacts` | [syntax-kinds-and-facts.md](syntax-kinds-and-facts.md) |
| Tokens and trivia | [tokens.md](tokens.md) |
| Lexer | [lexer.md](lexer.md) |
| Parser and recovery | [parser.md](parser.md) |
| AST shape | [syntax-tree.md](syntax-tree.md) |
| Public API (lex/parse side) | [public-api.md](public-api.md) |
| Suggested layout inside `RoboSharp.Language` | [project-layout.md](project-layout.md) |

The former monolithic spec lived in this README; it is now split. For the **semantics** half, start at [../semantics/README.md](../semantics/README.md).
