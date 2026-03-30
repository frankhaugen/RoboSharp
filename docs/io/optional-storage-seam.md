# Optional lower-level storage seam

An optional `IRoboStorage` can sit below `IRoboFileSystem` in v1 if needed.

If included, it should represent byte persistence primitives, not project semantics.

Example:

```csharp
public interface IRoboStorage
{
    ValueTask<bool> ExistsAsync(Uri uri, CancellationToken cancellationToken = default);
    ValueTask<Stream> OpenReadAsync(Uri uri, CancellationToken cancellationToken = default);
    ValueTask<Stream> OpenWriteAsync(Uri uri, bool overwrite, CancellationToken cancellationToken = default);
    ValueTask DeleteAsync(Uri uri, CancellationToken cancellationToken = default);
}
```

Unless you already know you need that extra seam, `IRoboFileSystem` may be enough for v1.
