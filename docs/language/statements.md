# Statements (syntax)

Statement kinds in v1:

- block
- variable declaration
- assignment
- expression statement
- if (with optional else)
- while
- return

These appear as `StatementSyntax` nodes in the syntax tree. Parsing rules: [parser.md](parser.md). Surface summary: [syntax.md](syntax.md).

Semantic rules (`bool` conditions, reachability, etc.): [../semantics/control-flow-and-conditions.md](../semantics/control-flow-and-conditions.md).
