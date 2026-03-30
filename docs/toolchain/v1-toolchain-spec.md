# RoboSharp v1 toolchain specification

Normative toolchain contract: Studio vs Player roles, build lifecycle, artifacts, CLI, and interfaces.

**Implementation (partial):** `RoboSharpPipeline`, `RoboSharpCompiler`, `RoboExecutableJsonSerializer` (v1 JSON for `.roboexe`-equivalent interchange). Project file / `obj`/`bin` emission and CLI flags are not implemented yet.

**Related:** [build-process](build-process.md), [artifact-layout](artifact-layout.md), [implementation-gaps](../implementation-gaps.md).

---

## 1. Purpose

The toolchain defines how projects are:

* loaded
* built
* debugged
* run
* validated
* emitted as artifacts

It covers both:

* `RoboSharpStudio.exe`
* `RoboSharpPlayer.exe`

This builds on the file/toolchain direction already established.  

---

## 2. Tool roles

## 2.1 RoboSharpStudio.exe

Studio is the authoring and inspection tool.

Responsibilities:

* open/save `.robosharp`
* edit `.robo`
* build project
* generate intermediate artifacts
* start debug session
* run project from source build
* show source / AST / IL / runtime / world / output

## 2.2 RoboSharpPlayer.exe

Player is the runtime host for compiled artifacts.

Responsibilities:

* load `.roboexe`
* validate compatibility
* run or debug executable
* optionally load adjacent or embedded debug symbols
* expose stdout/stderr and snapshots

This role split is already aligned with the current project direction. 

---

## 3. Build configurations

Supported configurations in v1:

* `Debug`
* `Release`

### Debug

May emit:

* `.roboexe`
* `.roboast.json`
* `.robobind.json`
* `.roboil.json`
* `.robo.pdb.json`

### Release

Must emit:

* `.roboexe`

Optional stripping of debug metadata is allowed.

This aligns with the existing file format spec. 

---

## 4. Build commands

The toolchain must support these logical operations:

* `LoadProject`
* `Build`
* `Rebuild`
* `Run`
* `Debug`
* `Clean`

### Build

Compile changed sources and emit configured outputs.

### Rebuild

Equivalent to clean + build.

### Clean

Delete `obj/` and configuration-specific `bin/` outputs for the project.

---

## 5. Project lifecycle

### Open project

When a `.robosharp` is opened:

1. validate project file
2. resolve source file paths
3. load active build/runtime settings
4. build workspace/project model

### Build project

When building:

1. read current in-memory document state if Studio has unsaved buffers
2. compile project
3. write configured artifacts
4. surface diagnostics

This is where the workspace/file-system separation matters conceptually, even though the detailed storage spec is separate. 

---

## 6. Incremental behavior

v1 may choose a simple policy:

* always rebuild the full project on build

That is acceptable for v1 due to small project size.

Incremental compilation may be added later, but should not complicate v1.

---

## 7. Artifact emission policy

Artifact emission must be phase-ordered:

1. syntax artifacts
2. bound artifact
3. IL artifact
4. debug symbols
5. executable

If compilation fails before a stage, later-stage artifacts must not be emitted as “successful” outputs.

Partial/debugging artifacts may still be emitted if useful, but only if clearly marked as invalid/incomplete.

Simpler v1 rule: emit only artifacts derived from successfully completed phases.

---

## 8. Run command behavior

## 8.1 Studio Run

`Run` in Studio means:

1. build current project
2. if build succeeds, run produced executable
3. show output/runtime state in Studio

If build fails, run does not start.

## 8.2 Player Run

`Run` in Player means:

1. load `.roboexe`
2. validate it
3. create runtime state
4. execute to completion or fault

---

## 9. Debug command behavior

## 9.1 Studio Debug

`Debug` in Studio means:

1. build project in Debug configuration
2. load generated `.roboexe`
3. load `.robo.pdb.json` or embedded symbols
4. create debug session
5. pause at entry by default

## 9.2 Player Debug

`Debug` in Player means:

1. load `.roboexe`
2. load adjacent or embedded debug symbols if present
3. create debug session
4. support source view only if source mapping exists
5. otherwise fall back to IL-only debug

This is consistent with the debugger direction. 

---

## 10. CLI contract

Recommended v1 commands:

### Studio

```text
RoboSharpStudio.exe <project.robosharp>
RoboSharpStudio.exe <project.robosharp> --run
RoboSharpStudio.exe <project.robosharp> --debug
RoboSharpStudio.exe <project.robosharp> --build
```

### Player

```text
RoboSharpPlayer.exe <program.roboexe>
RoboSharpPlayer.exe <program.roboexe> --debug
RoboSharpPlayer.exe <program.roboexe> --pause-at-entry
RoboSharpPlayer.exe <program.roboexe> --headless
RoboSharpPlayer.exe <program.roboexe> --max-steps 10000
```

Optional:

```text
--world <path>
--profile <name>
```

if runtime override is allowed.

---

## 11. Exit code policy

Recommended v1 exit codes:

* `0` = success
* `1` = build failure / diagnostics error
* `2` = invalid project or executable format
* `3` = runtime fault
* `4` = invalid CLI arguments

Studio may not expose exit codes much in interactive mode, but Player and CLI builds should.

---

## 12. Validation boundaries

### Studio must validate

* project file before build
* source before run/debug
* debug symbol availability before source-mapped debugging

### Player must validate

* executable before run/debug
* debug symbol compatibility before source-mapped debugging

---

## 13. Toolchain interfaces

```csharp
public interface IProjectLoader
{
    ValueTask<RoboSharpProject> LoadAsync(string path, CancellationToken cancellationToken = default);
}
```

```csharp
public interface IBuildRunner
{
    ValueTask<BuildResult> BuildAsync(
        RoboSharpProject project,
        CancellationToken cancellationToken = default);
}
```

```csharp
public interface IExecutableLoader
{
    ValueTask<RoboExecutable> LoadAsync(string path, CancellationToken cancellationToken = default);
}
```

```csharp
public interface IPlayerHost
{
    ValueTask<RunResult> RunAsync(
        RoboExecutable executable,
        PlayerRunOptions options,
        CancellationToken cancellationToken = default);
}
```

---

## 14. Toolchain data-flow contract

The full toolchain is:

```text
.robosharp
 + .robo source files
   ↓
Compiler
   ↓
obj/
  .roboast.json
  .robobind.json
  .roboil.json
  .robo.pdb.json
   ↓
bin/
  .roboexe
   ↓
RoboSharpPlayer.exe / Studio debugger
```

This matches the file-format spec already established, but now with a full operational contract. 

