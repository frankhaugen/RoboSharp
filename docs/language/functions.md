# Program model and functions

A `.robo` file is the program body. There is no `script` wrapper in v1.

A compilation unit can contain:

- top-level statements
- function declarations

## Entry point (v1)

The program **must** include at least one **top-level statement** at file scope. That sequence is the program body. The compiler lowers it into one compiled function (see [v1 compiler spec](../compiler/v1-compiler-spec.md) §4.1); teaching tools describe that as **top-level statements**, not as a user-declared procedure.

You may also declare ordinary functions and call them from top-level code.

The identifiers **`TopLevel`** and **`main`** are **reserved**: do not declare user functions with those names. (`main` is not a RoboSharp entry point.)

Example (preferred, typical small robot programs):

```text
move();
print(1);
```

Example (functions + top-level):

```text
integer add(integer a, integer b)
{
    return a + b;
}

integer x = add(1, 2);
print(x);
```

This keeps the teaching story about *your* code and the robot, not about mimicking another language’s entry-point ceremony.

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

`ReturnType` is always present in the tree; for `Name(params) { }` in source it is a synthetic `void`.

See [syntax-tree.md](syntax-tree.md).
