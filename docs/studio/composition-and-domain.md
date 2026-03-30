# Composition root and Studio domain

## Startup model

Use `Host.CreateApplicationBuilder(args)` and build one single root `IHost`.

The Studio app should resolve `MainWindow`, shell view models, panel factories, and services through the built-in container.

## Root startup flow

```text
Program.cs
  → Build host
  → Register services
  → Start host
  → Resolve App
  → Resolve MainWindow
  → Run Avalonia lifetime
```

## Lifetime rules

- application-wide singletons for cross-session services
- scoped-like behavior simulated through explicit document/session objects
- transient UI components where appropriate
- no ambient static service locator

## DI registration strategy

Use DI heavily, but avoid interface spam.

Interfaces should exist where they express a true seam:

- storage backends
- workspace abstraction
- compiler pipeline orchestration
- debugger contract
- world renderer/projector
- dialog service
- user settings store

Concrete classes should be used directly when there is no real polymorphic value.

### Recommended service groups (illustrative)

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

### Avoid

Do not register every panel behind an interface unless there is a real extension seam. Panels can often be concrete types.

## Core domain concepts

The Studio should explicitly distinguish these concepts.

### Application state

Global desktop app state:

- theme
- recent projects
- last opened layout
- user preferences
- global commands
- global tool windows

### Workspace session

A currently opened project/workspace:

- project file
- source files
- artifact directories
- active lesson/world/profile
- build state
- open documents
- selected document
- debug/run state

### Document session

An opened editor/inspector tab:

- source file tab
- IL artifact tab
- AST artifact tab
- world file tab
- settings tab

### Run/debug session

A live execution session:

- executable/program
- debug metadata
- breakpoints
- snapshots
- current state
- output streams

Keeping these separate prevents the shell from becoming a god object.

## Concrete main types (illustrative)

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

### Service seams (illustrative)

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
