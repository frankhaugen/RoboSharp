# Program model and functions

A `.robo` file is the program body. There is no `script` wrapper in v1.

A compilation unit can contain:

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

This balances C# familiarity and simplicity.

## Syntax shape

Members are either a function declaration or a global (top-level) statement. See [parser.md](parser.md) for grammar.

AST shape (Semantics binds names):

```csharp
public sealed record FunctionDeclarationSyntax(
    TypeSyntax ReturnType,
    SyntaxToken Identifier,
    ParameterListSyntax Parameters,
    BlockStatementSyntax Body) : MemberSyntax;
```

See [syntax-tree.md](syntax-tree.md).
