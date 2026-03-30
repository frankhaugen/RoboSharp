# Binding and bound tree

## Bound tree

Direction:

- bound statements
- bound expressions
- explicit types on expressions
- explicit call targets
- explicit assignment targets

Clean lowering boundary before IL.

### Key point

The bound tree is where user-friendly source concepts stop and IL-ready concepts begin.

## Architecture tie-in

Binding walks the syntax tree, resolves symbols, applies types, and emits diagnostics. See [overview.md](overview.md) for pipeline placement.
