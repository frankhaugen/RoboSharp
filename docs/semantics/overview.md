# Semantic analysis — role

The semantic phase answers:

- does this name exist?
- what type is this expression?
- is this assignment valid?
- is this condition a `bool`?
- is this call legal?
- is this builtin available in the current profile?

## Pipeline

```text
Source
→ Lexer
→ Parser
→ Syntax Tree
→ Binding
→ Semantic Analysis
→ Bound Tree
```

Use the syntax tree for source structure and the bound tree for executable meaning ([binding-and-bound-tree.md](binding-and-bound-tree.md)).

If errors exist, IL generation does not proceed.
