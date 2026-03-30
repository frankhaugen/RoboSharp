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

## Ambiguity rule

A declaration is recognized because it begins with a **type keyword**. Otherwise it is parsed as assignment/expression statement. Simple and sufficient for v1.

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
