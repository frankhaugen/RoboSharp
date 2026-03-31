# Types (syntax)

In source, types are written as:

- primitive type keywords: `integer`, `number`, `string`, `bool`, and optional `void` for procedure return types (procedures may omit `void` in source; see [parser.md](parser.md)) ([syntax.md](syntax.md))
- arrays: `type[]`

`TypeSyntax` nodes represent these in the tree ([syntax-tree.md](syntax-tree.md)).

**Semantic** type symbols, equality, and assignability: [../semantics/type-system.md](../semantics/type-system.md).
