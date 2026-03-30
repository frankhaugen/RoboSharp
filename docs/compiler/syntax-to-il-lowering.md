# Syntax-to-IL lowering (v1 direction)

This document is the **bridge** between surface syntax and fake IL: it keeps syntax “honest” relative to the execution model. It complements [IL instruction set](../runtime/il-instruction-set.md) and [pipeline boundaries](../architecture/pipeline-boundaries.md).

Lowering is defined on the **bound** program (after types and symbols are known), not on raw syntax.

## Variable declaration

Source:

```text
integer x = 5
```

Illustrative IL sequence:

```text
PushConstant 5
StoreLocal x
```

## Assignment

Source:

```text
x = x + 1
```

Illustrative IL:

```text
LoadLocal x
PushConstant 1
Add
StoreLocal x
```

## If / else (built-in condition)

Source:

```text
if (frontIsClear())
{
    move()
}
else
{
    turnLeft()
}
```

Illustrative pattern (labels symbolic):

```text
CallBuiltin FrontIsClear
JumpIfFalse elseLabel
CallBuiltin Move
Jump endLabel
elseLabel:
CallBuiltin TurnLeft
endLabel:
```

User-defined predicates would use `Call` and the same branching opcodes.

## While loop

Source:

```text
while (condition)
{
    body
}
```

Illustrative pattern:

```text
loopHead:
  … evaluate condition …
  JumpIfFalse loopEnd
  … body …
  Jump loopHead
loopEnd:
```

Exact encodings depend on the frozen opcode set and whether conditions leave a boolean on the stack or use compare + branch.

## Calls: user vs built-in

- **User function** → `Call` target function metadata.
- **Built-in** (from active profile) → `CallBuiltin` with built-in id / operand form per IL spec.

See [Built-ins and profiles](../semantics/builtins-and-profiles.md).

## Related

- [Compilation pipeline](compilation-pipeline.md)
- [IL instruction set](../runtime/il-instruction-set.md)
- [Parser](parsing.md), [Semantic analysis](semantic-analysis.md)
