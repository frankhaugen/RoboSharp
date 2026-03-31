# v1 language surface

Freeze this shape as the v1 baseline.

## Keywords

v1 uses a **small fixed set** of reserved words (lexer keywords). Built-in calls such as `print` and `move` are **identifiers**, not keywords; availability is decided in semantic analysis via the active profile.

```text
if
else
while
return
integer
number
string
bool
void
```

`void` is a **type** keyword you can write for procedure return types (e.g. `void helper()`). For procedures you may **omit** `void` in source and write `Name(parameters) { … }` instead; the parser still builds a `void` return type in the syntax tree. See [functions.md](functions.md) and [parser.md](parser.md).

## Primitive types

```text
integer
number
string
bool
```

## Collection type

```text
type[]
```

## Statements

- variable declaration
- assignment
- expression statement
- if
- while
- return
- block

## Expressions

- literals
- variable names
- unary operators
- binary operators
- call expressions
- array literals
- index expressions
- parenthesized expressions

## Deliberately excluded

- classes
- interfaces
- member access
- methods on values
- generics
- null
- exceptions as language feature
- tuples
- dictionaries
- advanced collections
- operator overloading

That keeps a teaching language, not a disguised small general-purpose language.

More detail: [statements.md](statements.md), [expressions.md](expressions.md), [arrays.md](arrays.md).
