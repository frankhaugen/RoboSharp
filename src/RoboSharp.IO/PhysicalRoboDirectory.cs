namespace RoboSharp.IO;

internal sealed class PhysicalRoboDirectory : IRoboDirectory
{
    private readonly PhysicalRoboFileSystem _fileSystem;

    public PhysicalRoboDirectory(
        PhysicalRoboFileSystem fileSystem,
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

    public bool Exists()
    {
        var path = _fileSystem.UriToLocalPath(Uri);
        return Directory.Exists(path);
    }

    public IEnumerable<IRoboDirectory> EnumerateDirectories()
    {
        if (!Exists())
        {
            return [];
        }

        var path = _fileSystem.UriToLocalPath(Uri);
        var info = new DirectoryInfo(path);
        return info
            .EnumerateDirectories()
            .OrderBy(d => d.Name, StringComparer.Ordinal)
            .Select(d => _fileSystem.GetChildDirectory(this, d.Name));
    }

    public IEnumerable<IRoboFile> EnumerateFiles()
    {
        if (!Exists())
        {
            return [];
        }

        var path = _fileSystem.UriToLocalPath(Uri);
        var info = new DirectoryInfo(path);
        return info
            .EnumerateFiles()
            .OrderBy(f => f.Name, StringComparer.Ordinal)
            .Select(f => _fileSystem.GetChildFile(this, f.Name));
    }

    public IRoboDirectory GetDirectory(string relativeName) => _fileSystem.GetChildDirectory(this, relativeName);

    public IRoboFile GetFile(string relativeName) => _fileSystem.GetChildFile(this, relativeName);

    public async ValueTask EnsureExistsAsync(CancellationToken cancellationToken = default)
    {
        var path = _fileSystem.UriToLocalPath(Uri);
        await Task.Run(() => Directory.CreateDirectory(path), cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask DeleteAsync(bool recursive, CancellationToken cancellationToken = default)
    {
        var path = _fileSystem.UriToLocalPath(Uri);
        await Task.Run(() =>
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            Directory.Delete(path, recursive);
        }, cancellationToken).ConfigureAwait(false);
    }
}
