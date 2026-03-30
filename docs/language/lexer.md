# Lexer specification

Lexical surface:

- identifiers
- integer literals
- number literals
- string literals
- booleans
- keywords
- operators
- punctuation

## Responsibilities

- scan source into tokens
- preserve trivia
- classify keywords
- parse literal token values
- emit `BadToken` for illegal input
- continue scanning after bad tokens
- normalize line endings at source-model level or treat them consistently

## Supported operators

```text
+  -  *  /
== !=
< <= > >=
&& ||
!
=
```

## Supported punctuation

```text
( )
{ }
[ ]
,
;
```

## Comments (first pass)

Include:

- `// single line comment`

Cheap, familiar, useful in a teaching IDE.

Input: [source-model.md](source-model.md). Output tokens: [tokens.md](tokens.md).
