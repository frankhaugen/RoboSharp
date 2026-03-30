namespace RoboSharp.IO;

internal sealed class PhysicalRoboFile : IRoboFile
{
    private readonly PhysicalRoboFileSystem _fileSystem;

    public PhysicalRoboFile(PhysicalRoboFileSystem fileSystem, Uri uri, string relativePathFromRoot)
    {
        _fileSystem = fileSystem;
        Uri = uri;
        RelativePathFromRoot = relativePathFromRoot;
    }

    public Uri Uri { get; }

    public string Name => RoboNodeNames.GetName(Uri);

    public IRoboDirectory? Parent => _fileSystem.TryGetParentDirectory(RelativePathFromRoot);

    internal string RelativePathFromRoot { get; }

    public bool Exists()
    {
        var path = _fileSystem.UriToLocalPath(Uri);
        return File.Exists(path) && !Directory.Exists(path);
    }

    public async ValueTask<string> ReadAllTextAsync(CancellationToken cancellationToken = default)
    {
        var path = _fileSystem.UriToLocalPath(Uri);
        return await File.ReadAllTextAsync(path, _fileSystem.Encoding.DefaultEncoding, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask WriteAllTextAsync(string content, CancellationToken cancellationToken = default)
    {
        var path = _fileSystem.UriToLocalPath(Uri);
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllTextAsync(path, content, _fileSystem.Encoding.DefaultEncoding, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Stream> OpenReadAsync(CancellationToken cancellationToken = default)
    {
        var path = _fileSystem.UriToLocalPath(Uri);
        return await Task.Run(
                () => new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, FileOptions.Asynchronous),
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<Stream> OpenWriteAsync(bool overwrite = true, CancellationToken cancellationToken = default)
    {
        var path = _fileSystem.UriToLocalPath(Uri);
        var mode = overwrite ? FileMode.Create : FileMode.CreateNew;
        return await Task.Run(() =>
            {
                var directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                return new FileStream(path, mode, FileAccess.Write, FileShare.None, bufferSize: 4096, FileOptions.Asynchronous);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask DeleteAsync(CancellationToken cancellationToken = default)
    {
        var path = _fileSystem.UriToLocalPath(Uri);
        await Task.Run(() =>
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<DateTimeOffset?> GetLastWriteTimeUtcAsync(CancellationToken cancellationToken = default)
    {
        var path = _fileSystem.UriToLocalPath(Uri);
        return await Task.Run(() =>
            {
                if (!File.Exists(path))
                {
                    return (DateTimeOffset?)null;
                }

                var info = new FileInfo(path);
                return (DateTimeOffset?)info.LastWriteTimeUtc;
            }, cancellationToken)
            .ConfigureAwait(false);
    }
}
