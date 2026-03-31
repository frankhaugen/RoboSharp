# RoboSharp v1 compiler specification

Normative compiler contract: phases, lowering model, outputs, and recommended host interfaces.

**Implementation (partial):** `RoboSharpCompiler.Compile` (parse → bind → lower), `IlLowerer`, `RoboExecutable`. Opcode names in code are `RoboSharp.IL.RoboOpcode` (e.g. `PushInt` rather than a separate `PushConstant` id pool in v1).

**Related:** [compilation-pipeline](compilation-pipeline.md), [syntax-to-il-lowering](syntax-to-il-lowering.md), [il-generation](il-generation.md), [pipeline boundaries](../architecture/pipeline-boundaries.md).

---

## 1. Purpose

The compiler turns one RoboSharp project into:

* diagnostics
* inspectable intermediate artifacts
* a compiled `.roboexe`

The compiler must be:

* deterministic
* phase-based
* inspectable
* explicit about failure boundaries

The compiler does **not** execute code.
It only transforms source into validated executable form.

---

## 2. Compiler pipeline

The compiler pipeline is:

```text
Project Load
→ Source Load
→ Lexing
→ Parsing
→ Syntax Tree
→ Symbol Collection
→ Semantic Analysis / Binding
→ Bound Program
→ Lowering to IL
→ Executable Packaging
→ Artifact Emission
```

This refines the already-established source → syntax → semantic → IL pipeline into concrete compiler stages.  

---

## 3. Compiler phase rules

## 3.1 Project load

Input:

* `.robosharp` project file
* referenced `.robo` source files
* active builtin profile name
* optional world metadata path

Output:

* resolved project model
* absolute/normalized source file list
* build settings

Failure conditions:

* invalid project format
* missing startup file
* missing source file
* unsupported project version

Compilation stops here on failure.

---

## 3.2 Lexing

Each `.robo` source file is lexed independently.

Output:

* token stream
* trivia if preserved
* lexical diagnostics

Lexing must continue after bad tokens where possible.

Lexing never resolves meaning.

---

## 3.3 Parsing

Each source file is parsed into syntax.

Output:

* `CompilationUnitSyntax`
* parse diagnostics

The parser must recover and continue so downstream tools can still inspect syntax trees. That matches the earlier parser direction. 

---

## 3.4 Symbol collection

Before full semantic binding, the compiler collects global declarations:

* user-defined function names
* function signatures
* source locations of declarations

This phase also imports built-ins from the active builtin profile into the global function space. Profile-based builtin availability was already established earlier. 

Rules:

* no overloads in v1
* no duplicate function names
* no shadowing built-in names
* function declarations are global only

If symbol collection has errors, semantic analysis may continue, but IL generation must not proceed.

---

## 3.5 Semantic analysis / binding

Binding transforms syntax into a typed, symbol-resolved bound form.

Output:

* `SemanticModel`
* `BoundProgram`
* semantic diagnostics

Binding rules follow the already-defined symbol, scope, and type model:

* lexical scopes
* bound expressions/statements
* builtin resolution
* array typing
* return validation
* assignment legality 

Compilation may continue through this phase to gather diagnostics, but IL generation only proceeds if there are no errors.

---

## 3.6 Lowering to IL

This is the first missing major spec.

Input:

* fully valid `BoundProgram`

Output:

* constant pool
* compiled function table
* instruction list
* debug mapping metadata

Lowering is deterministic and must not perform additional type reasoning beyond what semantic analysis already established.

### Lowering responsibilities

* assign function ids
* assign local slots
* assign a single compiled function that holds top-level statements (implementation name: `TopLevel`)
* emit instructions in execution order
* generate branch targets / labels
* produce instruction-to-source mapping
* produce local-slot-to-name debug metadata

---

## 4. Lowering model

## 4.1 Entry point

Top-level statements lower into **one** compiled function so the runtime has a normal call frame to start from. The stable implementation name for that function is **`TopLevel`**; hosts and learner-facing text should describe it as *top-level statements* or *program entry*, not as a user-defined procedure.

A valid program must contain at least one top-level statement. The name **`main`** is not a user entry point; declaring `main` is a semantic error.

---

## 4.2 Local slot allocation

Each compiled function gets contiguous local slots.

Rule:

* parameters occupy the first slots in declaration order
* local variables occupy subsequent slots in declaration order of first appearance in bound form

Example:

```text
integer add(integer a, integer b)
{
    integer c = a + b
    return c
}
```

Lowered slot layout:

```text
slot 0 = a
slot 1 = b
slot 2 = c
```

No slot reuse in v1.

That keeps debug behavior simple.

---

## 4.3 Constant pool

Literal values used in IL are interned into a project-level constant pool.

Allowed constant kinds:

* integer
* number
* string
* bool

Duplicate constants may be deduplicated, but this is optional in v1.

---

## 4.4 Expression lowering rules

### Literal

```text
5
```

→

```text
PushConstant { constantId = ... }
```

### Variable read

```text
x
```

→

```text
LoadLocal { slot = x }
```

### Variable assignment

```text
x = expr
```

→

```text
...expr...
StoreLocal { slot = x }
```

### Binary arithmetic

```text
a + b
```

→

```text
...a...
...b...
Add
```

### Builtin call

```text
move()
```

→

```text
CallBuiltin { builtinId = Move }
```

### User function call

```text
add(1, 2)
```

→

```text
PushConstant 1
PushConstant 2
Call { functionId = add }
```

### Array literal

```text
[1, 2, 3]
```

→

```text
NewArray { elementType = integer }
PushConstant 1
ArrayAdd
PushConstant 2
ArrayAdd
PushConstant 3
ArrayAdd
```

### Index read

```text
values[0]
```

→

```text
LoadLocal values
PushConstant 0
ArrayGet
```

### Index assignment

```text
values[1] = 42
```

→

```text
LoadLocal values
PushConstant 1
PushConstant 42
ArraySet
```

---

## 4.5 Statement lowering rules

### Variable declaration

```text
integer x = 5
```

→

```text
PushConstant 5
StoreLocal x
```

### Expression statement

```text
print(x)
```

→

```text
LoadLocal x
CallBuiltin Print
```

### If

```text
if (cond) thenStmt else elseStmt
```

→

```text
...cond...
JumpIfFalse elseLabel
...then...
Jump endLabel
elseLabel:
...else...
endLabel:
```

### While

```text
while (cond) body
```

→

```text
loopStart:
...cond...
JumpIfFalse loopEnd
...body...
Jump loopStart
loopEnd:
```

### Return

```text
return expr
```

→

```text
...expr...
Return
```

For no-value return in the top-level entry function (`TopLevel`), emit `Return` with no required stack value.

---

## 5. Compiler outputs

Successful compilation produces:

* `.roboexe`
* optionally `.roboast.json`
* optionally `.robobind.json`
* optionally `.roboil.json`
* optionally `.robo.pdb.json`

That output model is already aligned with the file-format direction. 

---

## 6. Compiler failure boundary

IL generation and packaging must not run if any diagnostic with severity `Error` exists after semantic analysis.

Warnings may still allow build success.

---

## 7. Compiler interfaces

Recommended shape:

```csharp
public interface IProjectCompiler
{
    ValueTask<CompilationResult> CompileAsync(
        RoboSharpProject project,
        CancellationToken cancellationToken = default);
}
```

```csharp
public sealed record CompilationResult(
    bool Success,
    IReadOnlyList<Diagnostic> Diagnostics,
    CompilationArtifacts? Artifacts);
```

```csharp
public sealed record CompilationArtifacts(
    RoboExecutable Executable,
    SyntaxArtifactCollection SyntaxArtifacts,
    BoundArtifact? BoundArtifact,
    IlArtifact? IlArtifact,
    RoboDebugSymbols? DebugSymbols);
```

