# Project loading

A workspace should be built from a parsed `.robosharp` project file, not from arbitrary folder scanning. That aligns with the toolchain spec where `.robosharp` is the project container and `.robo` files are separate source files.

Recommended services:

```csharp
public interface IProjectLoader
{
    ValueTask<RoboSharpProject> LoadAsync(IRoboFile projectFile, CancellationToken cancellationToken = default);
    ValueTask SaveAsync(RoboSharpProject project, IRoboFile projectFile, CancellationToken cancellationToken = default);
}
```

```csharp
public interface IWorkspaceLoader
{
    ValueTask<IRoboWorkspace> LoadAsync(IRoboFileSystem fileSystem, IRoboFile projectFile, CancellationToken cancellationToken = default);
}
```

## Rules

- `startupFile` must exist in `sourceFiles`
- every referenced source file must resolve under workspace root
- invalid project structure must fail load cleanly
- project load should not run full compilation automatically
