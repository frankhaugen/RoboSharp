# Language layer — purpose and boundaries

The language side should stay small and separate from:

- lessons/profiles (availability is semantic/profile)
- runtime/world
- Studio UI
- file/project system

The core language model: fixed keywords, four primitive types, `type[]`, normal functions, top-level statements, and no object/member model.

## Purpose

This layer defines **language-only** concerns:

- what source code looks like
- how source becomes tokens
- how tokens become syntax

Meaning and binding live in [`RoboSharp.Semantics`](../semantics/README.md). Those layers answer how syntax becomes IL-ready structure together; this document focuses on the **Language** project boundary.

It does **not** define:

- lesson progression
- world simulation
- debugger UI
- storage/workspace behavior
- Avalonia Studio composition

Those consume the language layer; they do not define it.

## Design goals

The language layer must be:

- small
- explicit
- deterministic
- easy to inspect
- easy to teach
- cheap to implement
- cheap to evolve carefully

RoboSharp is statically typed, explicit, small, predictable, syntax-oriented, and runtime-visible, while avoiding objects, classes, generics, member syntax, and other complexity multipliers.

## Two-project split (v1)

Recommended compact layout (matches [`AGENTS.md`](../../AGENTS.md)):

```text
src/
  RoboSharp.Language/
  RoboSharp.Semantics/
```

Conceptually you can use internal namespaces (`.Syntax`, `.Symbols`, etc.) inside those assemblies rather than many small projects.

### Responsibility split

`RoboSharp.Language`

- token model
- lexer
- parser
- syntax nodes
- syntax facts
- source text/span model

`RoboSharp.Semantics`

- binder
- type rules
- conversions
- bound tree
- symbol resolution
- semantic diagnostics

See [../semantics/README.md](../semantics/README.md).

## Hard separation rules

### Parser does not know lesson profiles

Parser parses call syntax, not availability. Built-in profiles matter during semantic analysis, not parsing.

### Semantics does not know world state

The semantic layer can know built-in signatures. It must not know whether a robot can move right now.

### Syntax tree is source-oriented

It preserves what was written, including recoverable broken code.

### Bound tree is execution-oriented

It holds resolved symbols, validated types, and legal operations. Owned by Semantics.

## Program model (reminder)

A `.robo` file is the program body; there is no `script` wrapper in v1.

A compilation unit contains:

- top-level statements
- function declarations

Example:

```text
integer add(integer a, integer b)
{
    return a + b
}

integer x = add(1, 2)
print(x)
```

Details: [functions.md](functions.md), [syntax.md](syntax.md).

## Seams (language side)

From source to syntax tree:

1. Source model  
2. Token model  
3. Lexer  
4. Syntax facts  
5. Parser  
6. Syntax tree  

See [README.md](README.md) for topic links. Semantics continues with symbols, types, binder, bound tree, and diagnostics.
