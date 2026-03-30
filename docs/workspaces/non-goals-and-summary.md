# Non-goals for v1 and final position

## Non-goals for v1

Do not add these yet:

- file watching
- distributed storage backends
- git-aware workspace model
- multi-project solution workspace
- document diff/merge engine
- file locking semantics
- arbitrary virtual mount providers

Single-project workspaces are enough for v1.

## Final position

The correct v1 split is:

### `RoboSharp.IO`

- file/directory abstractions
- physical FS
- in-memory FS
- overlay FS
- path resolution helpers

### `RoboSharp.Workspaces`

- `.robosharp` project loading
- source file enumeration
- active configuration
- obj/bin artifact layout
- document sessions
- temporary workspace support
- Studio-facing session state on top of a workspace

The hard rule remains:

**Workspace sits on top of filesystem abstractions; it does not choose or own the storage strategy itself.**

The next useful spec after this is the **project system / build orchestration layer**, where `ICompilerPipeline`, artifact generation, incremental analysis, and debugger launch all plug into the workspace model.
