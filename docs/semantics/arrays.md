# Arrays (semantics)

Keep the design:

- one collection syntax
- dynamic size
- zero-based indexing
- no separate stack/list types
- no member model

## Required semantic checks

- element type compatibility
- contextual typing for `[]`
- index expression must be `integer`
- indexed assignment must match element type
- target of indexing must be `T[]`

## Language rule

Array literals should rely on context in v1 to keep the type story simpler.

Syntax side: [../language/arrays.md](../language/arrays.md).
