# Inspection panels (tokens, trees, IL)

## Tokens panel

Shows token stream:

- kind
- text
- span
- trivia optionally

Good for teaching lexing.

## Syntax Tree panel

Shows raw syntax nodes. Preserve invalid/recovered structure where parsing recovered.

## Semantic/Bound panel

Shows:

- resolved names
- types
- bound nodes
- call targets
- assignment targets

## IL panel

Flagship pane. Needs:

- instruction list
- opcode
- typed operand display
- source mapping
- breakpoint support
- current instruction highlight

This pane is central to the project’s identity.

See [../compiler/compilation-pipeline.md](../compiler/compilation-pipeline.md) for the pipeline these panels reflect.
