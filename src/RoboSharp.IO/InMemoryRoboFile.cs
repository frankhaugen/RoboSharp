namespace RoboSharp.IO;

internal sealed class InMemoryRoboFile : IRoboFile
{
    private readonly InMemoryRoboFileSystem _fileSystem;

    public InMemoryRoboFile(InMemoryRoboFileSystem fileSystem, Uri uri, string relativePathFromRoot)
    {
        _fileSystem = fileSystem;
        Uri = uri;
        RelativePathFromRoot = relativePathFromRoot;
    }

    public Uri Uri { get; }

    public string Name => RoboNodeNames.GetName(Uri);

    public IRoboDirectory? Parent => _fileSystem.TryGetParentDirectory(RelativePathFromRoot);

    internal string RelativePathFromRoot { get; }

    public bool Exists() => _fileSystem.NodeExists(RelativePathFromRoot, asDirectory: false);

    public ValueTask<string> ReadAllTextAsync(CancellationToken cancellationToken = default) =>
        _fileSystem.ReadAllTextAsync(RelativePathFromRoot, cancellationToken);

    public ValueTask WriteAllTextAsync(string content, CancellationToken cancellationToken = default) =>
        _fileSystem.WriteAllTextAsync(RelativePathFromRoot, content, cancellationToken);

    public ValueTask<Stream> OpenReadAsync(CancellationToken cancellationToken = default) =>
        _fileSystem.OpenReadAsync(RelativePathFromRoot, cancellationToken);

    public ValueTask<Stream> OpenWriteAsync(bool overwrite = true, CancellationToken cancellationToken = default) =>
        _fileSystem.OpenWriteAsync(RelativePathFromRoot, overwrite, cancellationToken);

    public ValueTask DeleteAsync(CancellationToken cancellationToken = default) =>
        _fileSystem.DeleteFileAsync(RelativePathFromRoot, cancellationToken);

    public ValueTask<DateTimeOffset?> GetLastWriteTimeUtcAsync(CancellationToken cancellationToken = default) =>
        _fileSystem.GetLastWriteTimeUtcAsync(RelativePathFromRoot, cancellationToken);
}
