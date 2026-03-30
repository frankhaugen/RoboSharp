# Canonical IO abstractions

Recommended contract surface:

```csharp
public interface IRoboFileSystem
{
    IRoboDirectory Root { get; }

    IRoboFile GetFile(Uri uri);
    IRoboDirectory GetDirectory(Uri uri);

    bool FileExists(Uri uri);
    bool DirectoryExists(Uri uri);
}
```

```csharp
public interface IRoboNode
{
    Uri Uri { get; }
    string Name { get; }
    IRoboDirectory? Parent { get; }
    bool Exists();
}
```

```csharp
public interface IRoboFile : IRoboNode
{
    ValueTask<string> ReadAllTextAsync(CancellationToken cancellationToken = default);
    ValueTask WriteAllTextAsync(string content, CancellationToken cancellationToken = default);

    ValueTask<Stream> OpenReadAsync(CancellationToken cancellationToken = default);
    ValueTask<Stream> OpenWriteAsync(bool overwrite = true, CancellationToken cancellationToken = default);

    ValueTask DeleteAsync(CancellationToken cancellationToken = default);
    ValueTask<DateTimeOffset?> GetLastWriteTimeUtcAsync(CancellationToken cancellationToken = default);
}
```

```csharp
public interface IRoboDirectory : IRoboNode
{
    IEnumerable<IRoboDirectory> EnumerateDirectories();
    IEnumerable<IRoboFile> EnumerateFiles();

    IRoboDirectory GetDirectory(string relativeName);
    IRoboFile GetFile(string relativeName);

    ValueTask EnsureExistsAsync(CancellationToken cancellationToken = default);
    ValueTask DeleteAsync(bool recursive, CancellationToken cancellationToken = default);
}
```

That is enough for v1.

## Why `Uri` over plain strings

Use `Uri` for identity. Not because this is “webby,” but because it forces a clearer separation between:

- node identity
- display name
- relative resolution

It also keeps overlay/composite implementations cleaner.

For convenience, higher layers can still offer helpers that accept relative project paths.
