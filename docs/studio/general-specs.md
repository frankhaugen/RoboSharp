# RoboSharpStudio Specification

Below is a concrete architecture/spec for **RoboSharpStudio** built around **Avalonia**, **Microsoft.Extensions.DependencyInjection**, and a **code-first-first** mindset. Avalonia supports using `Microsoft.Extensions.DependencyInjection` and can be built entirely in C# without requiring XAML, even though XAML remains available. ([Avalonia Docs][1])

## Avalonia + Built-in .NET DI + Code-First UI

## 1. Purpose

`RoboSharpStudio` is the desktop IDE for the RoboSharp ecosystem.

It exists to let a learner or teacher:

* open a RoboSharp workspace/project
* edit `.robo` source files
* inspect tokens, syntax tree, semantic model, IL, runtime state, stdout, stderr, and world state
* compile to `.roboexe`
* debug via a deterministic step debugger
* author or preview lesson/world content
* do all of that without coupling the application to one rendering technology or one storage mode

The Studio is not “the runtime with panels.”
It is a host shell over:

* workspace services
* compiler services
* runtime/debugger services
* rendering projection services
* lesson/profile services
* IDE state services

That separation is non-negotiable.

---

## 2. Technology Position

## 2.1 UI framework

Use **Avalonia** as the desktop UI shell.

Reason:

* better fit for code-first UI than WPF
* cross-platform option preserved
* works naturally with `Microsoft.Extensions.DependencyInjection` patterns in official guidance ([Avalonia Docs][1])

## 2.2 DI container

Use only:

* `Microsoft.Extensions.DependencyInjection`
* `Microsoft.Extensions.Hosting`
* `Microsoft.Extensions.Logging`
* optionally `Microsoft.Extensions.Options`

No Autofac, DryIoc, Prism, ReactiveUI container, or framework-owned service locator.

## 2.3 UI declaration rule

The official direction for this project should be:

* **code-first views by default**
* **no required XAML**
* XAML may be tolerated in isolated cases only if later proven materially better

For your preference set, I would spec the first version as entirely C#-built UI.

---

## 3. Solution Layout

Recommended solution shape:

```text
RoboSharp.slnx

src/
  RoboSharp.Core/
  RoboSharp.Language/
  RoboSharp.Semantics/
  RoboSharp.IL/
  RoboSharp.Runtime/
  RoboSharp.Debugging/
  RoboSharp.World/
  RoboSharp.Rendering/
  RoboSharp.IO/
  RoboSharp.Workspaces/
  RoboSharp.Lessons/
  RoboSharp.ProjectSystem/
  RoboSharp.Studio/
  RoboSharp.Studio.Shell/
  RoboSharp.Studio.Documents/
  RoboSharp.Studio.Panels/
  RoboSharp.Studio.Theming/
  RoboSharp.Studio.Commands/

tests/
  RoboSharp.Core.Tests/
  RoboSharp.Language.Tests/
  RoboSharp.Semantics.Tests/
  RoboSharp.Runtime.Tests/
  RoboSharp.Debugging.Tests/
  RoboSharp.World.Tests/
  RoboSharp.Workspaces.Tests/
  RoboSharp.Studio.Tests/
```

### Intent

`RoboSharp.Studio` should not own compiler/runtime logic.

It should mostly own:

* composition
* screen/view model orchestration
* IDE commands
* IDE state persistence
* panel composition
* document lifecycle
* application shell behavior

That keeps the Studio replaceable.

---

## 4. High-Level Studio Architecture

```text
RoboSharpStudio
 ├─ Host / Composition Root
 ├─ Shell
 │   ├─ Main Window
 │   ├─ Dock/Panel Layout
 │   ├─ Menus / Toolbars / Status
 │   └─ Dialog orchestration
 ├─ Workspace Layer
 │   ├─ Project open/save
 │   ├─ Source document lifecycle
 │   ├─ Build artifact layout
 │   └─ File system integration
 ├─ Editing Layer
 │   ├─ Source editor
 │   ├─ Diagnostics view
 │   ├─ Symbol navigation
 │   └─ Outline
 ├─ Inspection Layer
 │   ├─ Tokens
 │   ├─ Syntax tree
 │   ├─ Bound tree
 │   ├─ IL
 │   └─ Build outputs
 ├─ Debug Layer
 │   ├─ Run
 │   ├─ Step debugger
 │   ├─ Breakpoints
 │   ├─ Call stack
 │   ├─ Locals
 │   ├─ Heap/arrays
 │   ├─ stdout/stderr
 │   └─ Runtime fault display
 ├─ World Layer
 │   ├─ World view
 │   ├─ ASCII preview
 │   ├─ Sprite view later
 │   └─ Route / metrics panel
 └─ Lesson Layer
     ├─ Active builtin profile
     ├─ Goal/lesson metadata
     └─ Lesson-aware help
```

---

## 5. Composition Root

## 5.1 Startup model

Use `Host.CreateApplicationBuilder(args)` and build one single root `IHost`.

The Studio app should resolve:

* `MainWindow`
* shell view models
* panel factories
* services

through the built-in container.

## 5.2 Root startup flow

```text
Program.cs
  → Build host
  → Register services
  → Start host
  → Resolve App
  → Resolve MainWindow
  → Run Avalonia lifetime
```

## 5.3 Lifetime rules

* application-wide singletons for cross-session services
* scoped-like behavior simulated through explicit document/session objects
* transient UI components where appropriate
* no ambient static service locator

---

## 6. DI Registration Strategy

## 6.1 Registration philosophy

Use DI heavily, but avoid interface spam.

Interfaces should exist where they express a true seam:

* storage backends
* workspace abstraction
* compiler pipeline orchestration
* debugger contract
* world renderer/projector
* dialog service
* user settings store

Concrete classes should be used directly when there is no real polymorphic value.

## 6.2 Recommended service groups

```csharp
builder.Services
    .AddLogging()
    .AddSingleton<IClock, SystemClock>()
    .AddSingleton<IUserSettingsStore, UserSettingsStore>()
    .AddSingleton<IRecentProjectsStore, RecentProjectsStore>()
    .AddSingleton<IStudioThemeService, StudioThemeService>()
    .AddSingleton<ICommandRegistry, CommandRegistry>()
    .AddSingleton<IKeyboardShortcutService, KeyboardShortcutService>();

builder.Services
    .AddSingleton<IRoboFileSystemFactory, RoboFileSystemFactory>()
    .AddSingleton<IWorkspaceLoader, WorkspaceLoader>()
    .AddSingleton<IWorkspaceSessionManager, WorkspaceSessionManager>();

builder.Services
    .AddSingleton<ICompilerPipeline, CompilerPipeline>()
    .AddSingleton<IBuiltinProfileProvider, BuiltinProfileProvider>()
    .AddSingleton<ILessonProvider, LessonProvider>();

builder.Services
    .AddSingleton<IDebugger, Debugger>()
    .AddSingleton<IRuntimeSessionFactory, RuntimeSessionFactory>();

builder.Services
    .AddSingleton<IWorldRenderProjector, WorldRenderProjector>()
    .AddSingleton<IAsciiWorldRenderer, AsciiWorldRenderer>();

builder.Services
    .AddSingleton<IMainWindowViewModel, MainWindowViewModel>()
    .AddTransient<MainWindow>();
```

## 6.3 Avoid

Do not register every panel behind an interface unless there is a real extension seam.
Panels can often be concrete types.

---

## 7. Core Studio Domain Concepts

The Studio should explicitly distinguish these concepts.

## 7.1 Application state

Global desktop app state:

* theme
* recent projects
* last opened layout
* user preferences
* global commands
* global tool windows

## 7.2 Workspace session

A currently opened project/workspace:

* project file
* source files
* artifact directories
* active lesson/world/profile
* build state
* open documents
* selected document
* debug/run state

## 7.3 Document session

An opened editor/inspector tab:

* source file tab
* IL artifact tab
* AST artifact tab
* world file tab
* settings tab

## 7.4 Run/debug session

A live execution session:

* executable/program
* debug metadata
* breakpoints
* snapshots
* current state
* output streams

Keeping these separate prevents the shell from becoming a god object.

---

## 8. Main Window Specification

## 8.1 Top-level layout

Recommended shell layout:

```text
+----------------------------------------------------------------------------------+
| Menu Bar                                                                         |
+----------------------------------------------------------------------------------+
| Toolbar / Command Bar                                                            |
+----------------------+--------------------------------------+--------------------+
| Solution/Workspace   | Document Tabs                        | Inspector Switcher |
| Explorer             |                                      |                    |
|                      | Source / IL / AST / World / etc.     |                    |
|                      |                                      |                    |
+----------------------+----------------------+---------------+--------------------+
| Bottom Panel: Diagnostics / stdout / stderr / Build / Search / Debug Output     |
+----------------------+----------------------+------------------------------------+
| Left Bottom Optional | Right Bottom Optional| Status Bar                         |
| Call Stack / Outline | Locals / Heap        |                                    |
+----------------------------------------------------------------------------------+
```

## 8.2 Minimum panes

The Studio should ship with these panes:

* Workspace Explorer
* Editor/Document Tabs
* Diagnostics
* Tokens
* Syntax Tree
* Semantic/Bound Tree
* IL View
* World View
* Debug Controls
* Call Stack
* Locals
* Arrays/Heap
* stdout
* stderr
* Route/Metrics
* Status Bar

Not all need to be visible by default.

## 8.3 Default layout

Default learner-friendly layout:

* center: source editor
* right: world view + IL view tabs
* bottom: stdout/stderr + diagnostics
* left: workspace explorer
* hidden but dockable: tokens, syntax tree, bound tree, heap, route metrics

Default advanced layout can expose more.

---

## 9. Shell Navigation Model

Use a document/panel hybrid model.

### Document tabs

For editable or focused artifacts:

* `.robo`
* `.robosharp`
* `.world.json`
* `.roboil.json`
* `.roboast.json`
* `.robobind.json`

### Tool windows

For contextual views:

* diagnostics
* call stack
* locals
* heap
* output
* runtime metrics
* lesson help

This maps well to IDE expectations.

---

## 10. Workspace Model in Studio

The Studio should consume your already-corrected layering:

* storage/file system below
* workspace above storage

That means the Studio opens a workspace over an `IRoboFileSystem`, not over raw paths. This keeps unsaved/overlay scenarios viable. 

## 10.1 File system strategy

The Studio should support:

* physical file system
* in-memory file system
* overlay file system

Best practical design:

```text
Physical file system
       +
In-memory modified documents
       ↓
Overlay file system
       ↓
Workspace session
```

This makes “dirty but not saved” documents natural.

## 10.2 Workspace session responsibilities

`WorkspaceSession` should expose:

* current project
* source documents
* artifact directories
* active file
* build configuration
* current build results
* current lesson/profile/world settings

It should not contain rendering or compiler logic itself.

---

## 11. Document Management

## 11.1 Document kinds

```text
SourceDocument
ProjectDocument
WorldDocument
SyntaxArtifactDocument
BoundArtifactDocument
IlArtifactDocument
ReadOnlyTextDocument
```

## 11.2 Document state

Every open document should track:

* URI/id
* display name
* dirty flag
* read-only flag
* current text or structured content
* diagnostics subset
* last parse/analysis revision

## 11.3 Save rules

* source documents can save independently
* project file can save independently
* build artifacts are never hand-edited by default
* artifact tabs are regenerated outputs

---

## 12. Source Editor Specification

## 12.1 Editor goals

The source editor must support:

* syntax highlighting
* diagnostics squiggles
* breakpoint gutter
* current execution line/instruction mapping
* completion based on active built-in profile
* basic hover info
* go to definition for user-defined functions
* find references later

## 12.2 Completion behavior

Completions should be profile-aware.

If a lesson only enables:

* `move()`
* `turnLeft()`

then completion should not suggest unavailable built-ins.

That follows your built-in profile model directly. 

## 12.3 Diagnostics behavior

Source diagnostics should merge:

* parse diagnostics
* semantic diagnostics
* profile restriction diagnostics
* optional workspace/build diagnostics

## 12.4 Statement mapping

The editor should allow mapping from source to:

* syntax node
* bound node
* IL instruction range
* current debug span

That is one of the key educational payoffs.

---

## 13. Build / Analysis Pipeline Inside Studio

## 13.1 Phases

The Studio should expose these phases explicitly:

```text
Text
→ Tokens
→ Syntax Tree
→ Semantic Model
→ Bound Tree
→ IL
→ Executable Packaging
```

## 13.2 Build modes

Three useful build modes:

### Live analysis

Triggered after edits, debounced.
Produces:

* tokens
* syntax tree
* semantic diagnostics
* optionally bound tree in memory

### Full debug build

Produces:

* `.roboast.json`
* `.robobind.json`
* `.roboil.json`
* `.robo.pdb.json`
* `.roboexe`

### Release build

Produces:

* `.roboexe`

## 13.3 Backgrounding rule

Do not mutate UI state directly from compilation services.
Compilation emits immutable result objects.

---

## 14. Inspection Panels

## 14.1 Tokens panel

Shows token stream:

* kind
* text
* span
* trivia optionally

Good for teaching lexing.

## 14.2 Syntax Tree panel

Shows raw syntax nodes.

Should preserve invalid/recovered structure where parsing recovered.

## 14.3 Semantic/Bound panel

Shows:

* resolved names
* types
* bound nodes
* call targets
* assignment targets

## 14.4 IL panel

Should be one of the flagship panes.

Needs:

* instruction list
* opcode
* typed operand display
* source mapping
* breakpoint support
* current instruction highlight

This pane is central to the project’s identity.

---

## 15. Runtime / Debugger Integration

## 15.1 Debug model

The Studio should integrate the snapshot-based debugger model you already shaped:

* Step Into
* Step Over
* Step Out
* Continue
* Pause
* Stop
* Reset
* breakpoints
* source + IL synchronization
* call stack / locals / arrays / world / stdout / stderr

## 15.2 Debug command bar

Must expose, at minimum:

* Run
* Debug
* Pause
* Stop
* Reset
* Step Into
* Step Over
* Step Out

## 15.3 Debug visual synchronization

When paused:

* source span highlighted
* IL instruction highlighted
* call stack updated
* locals pane updated
* heap pane updated
* world pane updated
* stdout/stderr panes updated
* route/metrics pane updated

## 15.4 Breakpoints

Source and IL breakpoints must both be supported.
Source breakpoints resolve to instruction indices via debug metadata. 

---

## 16. World View Specification

## 16.1 Rendering model

Studio should render from `RobotWorldSnapshot`, not from live runtime internals.
Projection to render tiles should be separate. That keeps runtime UI-independent.

## 16.2 Layer model

World visualization must respect:

* `TerrainGrid`
* `ItemGrid`
* `ActorGrid`

That layered design is already the correct world abstraction for this project. 

## 16.3 Required world views

### Sprite/grid view

Primary learner view.

### ASCII view

First-class, not debug-trash.
Useful for headless tests and side-by-side comparison.

### Tile inspector

When selecting a tile, show:

* terrain
* item
* actor id/state
* coordinates
* metadata if relevant

## 16.4 Debug overlays

Useful overlays:

* coordinates
* visited tiles
* route history
* goal tiles
* blocked attempts
* current actor facing

---

## 17. Output System in Studio

The Studio must visually separate:

* **Program Output** (`stdout`)
* **Runtime Messages** (`stderr`)

That is already the right runtime model for RoboSharp. 

## 17.1 stdout pane

For `print(...)` and intentional program output.

## 17.2 stderr pane

For runtime warnings/fault messages, such as:

* divide by zero fallback
* blocked movement
* out-of-bounds array access
* empty-array operation

## 17.3 Output timeline behavior

Each output line should retain instruction-pointer metadata.
The Studio can then optionally show output “since last step.”

---

## 18. Metrics / Runtime State Panels

The “metrics view” is worth having, but it should stay didactic rather than profiler-like. 

## 18.1 Required state panels

### Call Stack

Current frames, current frame highlighted.

### Locals

Current frame locals and values.

### Arrays / Heap

Array id, type, contents, references.

### Runtime statistics

* instructions executed
* arrays allocated
* current stack depth
* max stack depth

### Robot state

* position
* direction
* inventory
* world interaction values

## 18.2 Route / lesson metrics

Nice value for the Studio:

* visited tiles
* repeated tiles
* failed move attempts
* turn count
* shortest path comparison later

---

## 19. Lesson-Aware Studio Behavior

## 19.1 Lesson metadata

The Studio should understand:

* lesson title
* description
* active built-in profile
* world file
* goal definition

## 19.2 Lesson-aware help

Show only the current lesson’s available built-ins and concepts.
Do not flood early learners with the full language surface.

## 19.3 Goal panel

Should display:

* current objective
* completion state
* optional hints
* optional ideal constraints later

---

## 20. Menus and Commands

## 20.1 Top-level menus

Recommended:

* File
* Edit
* View
* Build
* Run
* Debug
* World
* Lesson
* Tools
* Help

## 20.2 Must-have commands

### File

* New Project
* Open Project
* Save
* Save All
* Close Project
* Recent Projects

### Build

* Analyze
* Build Debug
* Build Release
* Rebuild
* Open Output Folder

### Run/Debug

* Run
* Debug
* Pause
* Stop
* Reset
* Step Into
* Step Over
* Step Out

### View

* Toggle Tokens
* Toggle Syntax Tree
* Toggle Semantic Tree
* Toggle IL
* Toggle Diagnostics
* Toggle World
* Toggle stdout/stderr
* Toggle Call Stack
* Toggle Locals
* Toggle Heap
* Toggle Metrics

---

## 21. Settings / Preferences

## 21.1 Application settings

* theme
* font size
* default layout
* auto-save behavior
* live analysis on/off
* ASCII world preview visible
* debug pause-at-entry
* max snapshots retained
* max steps default

## 21.2 Project settings

Mostly live in `.robosharp`:

* source files
* output paths
* active builtin profile
* world file
* build flags

## 21.3 Persistence

User-global settings separate from project settings.

---

## 22. Theming and UI Style

Even if you stay code-first, define a clean theme layer.

## 22.1 Theming goals

* dark mode first
* high readability
* didactic emphasis colors for:

  * source
  * IL
  * syntax
  * runtime
  * world
  * diagnostics

## 22.2 Highlight colors should differentiate:

* current source execution span
* current IL instruction
* errors
* warnings
* stdout
* stderr
* lesson goal tiles
* route overlay

---

## 23. Extensibility Model

Do not over-engineer plugins in v1.
But leave clean seams.

## 23.1 Likely extension seams

* world renderer
* lesson source/provider
* file system backend
* command registration
* panel registration
* artifact viewers

## 23.2 Avoid

Do not build a general MEF-style extension system yet.

---

## 24. Recommended Project Breakdown for Studio

### `RoboSharp.Studio`

Application composition root and startup.

### `RoboSharp.Studio.Shell`

Main window, shell state, docking, command surfaces.

### `RoboSharp.Studio.Documents`

Document session types, editors, viewers, tabs.

### `RoboSharp.Studio.Panels`

Diagnostics, IL, syntax tree, output, call stack, locals, heap, world.

### `RoboSharp.Studio.Commands`

Command definitions and handlers.

### `RoboSharp.Studio.Theming`

Theme services, brushes/resources, editor coloring.

This keeps Studio modular without becoming fragmented.

---

## 25. Concrete Main Types

Recommended core types:

```csharp
public sealed class StudioApp : Application;
public sealed class MainWindow : Window;

public sealed class MainWindowViewModel;
public sealed class WorkspaceSession;
public sealed class DocumentSession;
public sealed class DebugSessionHost;
public sealed class StudioLayoutState;
public sealed class StudioCommand;
```

Recommended service seams:

```csharp
public interface IWorkspaceSessionManager;
public interface IWorkspaceLoader;
public interface IStudioDialogService;
public interface IStudioThemeService;
public interface IRecentProjectsStore;
public interface IUserSettingsStore;
public interface ICommandRegistry;
public interface IWorldRenderProjector;
public interface IAsciiWorldRenderer;
```

---

## 26. Testing Strategy

The Studio should be testable without booting real windows for most logic.

## 26.1 Unit-test heavily

* command handlers
* layout state reducers
* document session logic
* workspace session logic
* debug orchestration
* panel view models

## 26.2 UI tests

Only a thinner layer:

* window opens
* basic document switching
* breakpoint interactions
* debug stepping visible
* panes update correctly

## 26.3 Determinism

Debugger integration tests should use known `.roboexe` fixtures and snapshot assertions.

---

## 27. Performance Rules

The Studio is not a game engine, but responsiveness matters.

## 27.1 Rules

* debounce live compilation
* avoid reparsing every panel separately
* cache compilation products per document revision
* never let panel rendering trigger recompilation
* snapshots immutable and cheap to diff
* large artifact viewers virtualized if needed

## 27.2 Build cancellation

Typing should cancel outdated background analysis.

---

## 28. Explicit Non-Goals for v1 Studio

Do not include these in the first serious cut:

* plugin marketplace
* collaborative editing
* arbitrary watch expressions
* REPL
* drag-drop visual programming mode
* designer-generated UI builder
* Roslyn-like full language server complexity
* hot reload of runtime internals
* complex docking framework if simple layout works

---

## 29. MVP Cut vs Full v1

## MVP Studio

* open/save `.robosharp`
* edit `.robo`
* show diagnostics
* build debug
* show IL
* run/debug
* show world
* show stdout/stderr
* show call stack + locals
* built-in profile-aware completion/help

## Full v1 Studio

Adds:

* tokens
* syntax tree
* bound tree
* heap/array view
* route metrics
* lesson goal panel
* ASCII view
* layout persistence
* source + IL breakpoint parity
* debug metadata-backed source mapping

---

## 30. Final Position

The right Studio architecture is:

* **Avalonia shell**
* **built-in .NET DI via Generic Host / ServiceCollection**
* **code-first UI**
* **workspace over filesystem abstractions**
* **compiler/runtime/debugger as separate libraries**
* **snapshot-based debug visualization**
* **layered world rendering**
* **stdout/stderr separation**
* **lesson/profile-aware editing experience**

That gives you an IDE that matches the philosophy of the language itself:

* explicit
* inspectable
* deterministic
* modular
* teachable

[1]: https://docs.avaloniaui.net/docs/app-development/dependency-injection "Implementing dependency injection | Avalonia Docs"
