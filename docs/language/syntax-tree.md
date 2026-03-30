# AST model

Direction:

- `CompilationUnitSyntax`
- `MemberSyntax`
- `StatementSyntax`
- `ExpressionSyntax`
- `TypeSyntax`

## Core nodes

```csharp
public abstract record SyntaxNode;
public abstract record MemberSyntax : SyntaxNode;
public abstract record StatementSyntax : SyntaxNode;
public abstract record ExpressionSyntax : SyntaxNode;
public abstract record TypeSyntax : SyntaxNode;
```

## Root

```csharp
public sealed record CompilationUnitSyntax(
    IReadOnlyList<MemberSyntax> Members,
    SyntaxToken EndOfFileToken) : SyntaxNode;
```

## Members

```csharp
public sealed record FunctionDeclarationSyntax(
    TypeSyntax ReturnType,
    SyntaxToken Identifier,
    ParameterListSyntax Parameters,
    BlockStatementSyntax Body) : MemberSyntax;

public sealed record GlobalStatementSyntax(
    StatementSyntax Statement) : MemberSyntax;
```

## Statements

- block
- variable declaration
- assignment
- expression statement
- if
- while
- return

## Expressions

- literal
- name
- unary
- binary
- parenthesized
- call
- array literal
- index

Kinds and facts: [syntax-kinds-and-facts.md](syntax-kinds-and-facts.md). Parser: [parser.md](parser.md).
