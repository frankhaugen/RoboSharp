# Recommended concrete types and DI registrations

## Concrete types

```csharp
public sealed class RoboProjectWorkspace : IRoboWorkspace
{
    public required IRoboFileSystem FileSystem { get; init; }
    public required IRoboDirectory Root { get; init; }
    public required IRoboFile ProjectFile { get; init; }
    public required RoboSharpProject Project { get; init; }
    public required IBuildArtifactLayout ArtifactLayout { get; init; }
    public required string ActiveConfiguration { get; init; }
}
```

```csharp
public sealed class RoboTemporaryWorkspace : IRoboWorkspace
{
    // same core contract, but backed by synthetic project data
}
```

```csharp
public sealed class WorkspaceSession : IWorkspaceSession
{
    // open docs, active doc, save lifecycle, config switching
}
```

```csharp
public sealed class RoboDocumentSession : IRoboDocumentSession
{
    // file, dirty state, overlay-backed text
}
```

## DI registrations

```csharp
services.AddSingleton<IRoboPathService, RoboPathService>();

services.AddSingleton<IRoboFileSystemFactory, RoboFileSystemFactory>();
services.AddSingleton<IProjectLoader, ProjectLoader>();
services.AddSingleton<IWorkspaceLoader, WorkspaceLoader>();
services.AddSingleton<IBuildArtifactLayout, DefaultBuildArtifactLayout>();

services.AddTransient<IWorkspaceSession, WorkspaceSession>();
services.AddTransient<IRoboDocumentSessionFactory, RoboDocumentSessionFactory>();
```

For storage implementations:

```csharp
services.AddSingleton<PhysicalRoboFileSystem>();
services.AddSingleton<InMemoryRoboFileSystem>();
services.AddSingleton<OverlayRoboFileSystem>();
```

Do not register `IRoboFile` or `IRoboDirectory` directly in DI. Those are runtime objects, not app-singleton services.

Registration happens in host composition roots per [`AGENTS.md`](../../AGENTS.md).
