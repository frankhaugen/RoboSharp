# Build and analysis pipeline inside Studio

## Phases

The Studio should expose these phases explicitly:

```text
Text
→ Tokens
→ Syntax Tree
→ Semantic Model
→ Bound Tree
→ IL
→ Executable Packaging
```

## Build modes

### Live analysis

Triggered after edits, debounced. Produces:

- tokens
- syntax tree
- semantic diagnostics
- optionally bound tree in memory

### Full debug build

Produces:

- `.roboast.json`
- `.robobind.json`
- `.roboil.json`
- `.robo.pdb.json`
- `.roboexe`

### Release build

Produces:

- `.roboexe`

## Backgrounding rule

Do not mutate UI state directly from compilation services. Compilation emits immutable result objects.

See [../toolchain/build-process.md](../toolchain/build-process.md) for toolchain-wide build semantics.
