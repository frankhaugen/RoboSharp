namespace RoboSharp.IO;

internal sealed class InMemoryRoboDirectory : IRoboDirectory
{
    private readonly InMemoryRoboFileSystem _fileSystem;

    public InMemoryRoboDirectory(
        InMemoryRoboFileSystem fileSystem,
        Uri uri,
        IRoboDirectory? parent,
        string relativePathFromRoot)
    {
        _fileSystem = fileSystem;
        Uri = uri;
        Parent = parent;
        RelativePathFromRoot = relativePathFromRoot;
    }

    public Uri Uri { get; }

    public string Name => RoboNodeNames.GetName(Uri);

    public IRoboDirectory? Parent { get; }

    internal string RelativePathFromRoot { get; }

    public bool Exists() => _fileSystem.NodeExists(RelativePathFromRoot, asDirectory: true);

    public IEnumerable<IRoboDirectory> EnumerateDirectories() => _fileSystem.EnumerateDirectories(this);

    public IEnumerable<IRoboFile> EnumerateFiles() => _fileSystem.EnumerateFiles(this);

    public IRoboDirectory GetDirectory(string relativeName) => _fileSystem.GetChildDirectory(this, relativeName);

    public IRoboFile GetFile(string relativeName) => _fileSystem.GetChildFile(this, relativeName);

    public ValueTask EnsureExistsAsync(CancellationToken cancellationToken = default) =>
        _fileSystem.EnsureDirectoryExistsAsync(RelativePathFromRoot, cancellationToken);

    public ValueTask DeleteAsync(bool recursive, CancellationToken cancellationToken = default) =>
        _fileSystem.DeleteDirectoryAsync(RelativePathFromRoot, recursive, cancellationToken);
}
