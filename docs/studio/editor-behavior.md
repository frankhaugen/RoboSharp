# Documents and source editor

## Document kinds

```text
SourceDocument
ProjectDocument
WorldDocument
SyntaxArtifactDocument
BoundArtifactDocument
IlArtifactDocument
ReadOnlyTextDocument
```

## Document state

Every open document should track:

- URI/id
- display name
- dirty flag
- read-only flag
- current text or structured content
- diagnostics subset
- last parse/analysis revision

## Save rules

- source documents can save independently
- project file can save independently
- build artifacts are never hand-edited by default
- artifact tabs are regenerated outputs

## Source editor goals

The source editor must support:

- syntax highlighting
- diagnostics squiggles
- breakpoint gutter
- current execution line/instruction mapping
- completion based on active built-in profile
- basic hover info
- go to definition for user-defined functions
- find references later

## Completion behavior

Completions should be profile-aware.

If a lesson only enables `move()` and `turnLeft()`, completion should not suggest unavailable built-ins. That follows the built-in profile model in semantic analysis.

## Diagnostics behavior

Source diagnostics should merge:

- parse diagnostics
- semantic diagnostics
- profile restriction diagnostics
- optional workspace/build diagnostics

## Statement mapping

The editor should allow mapping from source to:

- syntax node
- bound node
- IL instruction range
- current debug span

That is one of the key educational payoffs.

See [syntax-highlighting.md](syntax-highlighting.md) and [../debugger/debugger-architecture.md](../debugger/debugger-architecture.md).
