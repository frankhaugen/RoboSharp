# Parser specification

Direction:

- recursive descent
- LL-friendly
- precedence climbing for expressions
- recovery for broken code

## Parser entry points

```csharp
ParseCompilationUnit
ParseMember
ParseFunctionDeclaration
ParseStatement
ParseExpression
ParseType
```

## Program grammar shape

A file is:

```text
Program = { Member }
```

A member is:

```text
FunctionDeclaration | Statement
```

`FunctionDeclaration` is either:

```text
Type Identifier "(" ParameterList ")" Block
```

or a **procedure** with omitted return type (defaults to void):

```text
Identifier "(" ParameterList ")" Block
```

The second form is allowed only when `(` is immediately followed by `)` or by a **parameter type** (`integer` / `number` / `string` / `bool` / `void` / array thereof), and the closing `)` is followed by `{`. That distinguishes `MoveMany(integer n) { }` from `MoveMany(5);` (a top-level call).

## Ambiguity rule

- A member that begins with a **type keyword** is either a typed function (`Type id "(" …`) or a top-level variable declaration (`Type id "=" …`).
- A member that begins with **`Identifier "("`** is parsed as a procedure declaration only if the header matches the procedure rule above; otherwise it is parsed as a **statement** (typically a call).

## Parser recovery

Important because the syntax tree is part of the teaching experience.

Recommended recovery:

- emit diagnostics
- skip until synchronization token
- continue parsing later statements

Suggested synchronization anchors:

```text
}
if
while
return
```

Example of recoverable tree:

```text
integer x =

move()
```

`move()` can survive as a statement after a broken declaration.

AST shapes: [syntax-tree.md](syntax-tree.md).
