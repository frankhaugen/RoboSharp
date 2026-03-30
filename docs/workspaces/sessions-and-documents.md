# Workspace session vs workspace

Important split:

- `IRoboWorkspace` = stable project/storage/build layout abstraction
- `IWorkspaceSession` = mutable Studio/session state

Recommended:

```csharp
public interface IWorkspaceSession
{
    IRoboWorkspace Workspace { get; }

    string ActiveConfiguration { get; }
    IReadOnlyList<IRoboDocumentSession> OpenDocuments { get; }
    IRoboDocumentSession? ActiveDocument { get; }

    ValueTask<IRoboDocumentSession> OpenDocumentAsync(IRoboFile file, CancellationToken cancellationToken = default);
    ValueTask SaveAllAsync(CancellationToken cancellationToken = default);
}
```

That keeps the base workspace usable in CLI/headless builds without dragging in editor state.

## Document sessions

Recommended shape:

```csharp
public interface IRoboDocumentSession
{
    IRoboFile File { get; }
    string DisplayName { get; }

    bool IsDirty { get; }
    bool IsReadOnly { get; }

    ValueTask<string> GetTextAsync(CancellationToken cancellationToken = default);
    ValueTask SetTextAsync(string text, CancellationToken cancellationToken = default);
    ValueTask SaveAsync(CancellationToken cancellationToken = default);
    ValueTask RevertAsync(CancellationToken cancellationToken = default);
}
```

### Rules

- source docs are editable
- generated artifact docs are read-only by default
- changes should hit overlay FS first, not physical storage immediately
- save flushes overlay content into physical storage for persistent docs

See [studio-overlay-and-save.md](studio-overlay-and-save.md) for Studio flow.
