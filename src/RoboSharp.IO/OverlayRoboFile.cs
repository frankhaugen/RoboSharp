namespace RoboSharp.IO;

internal sealed class OverlayRoboFile : IRoboFile
{
    private readonly OverlayRoboFileSystem _fileSystem;

    public OverlayRoboFile(
        OverlayRoboFileSystem fileSystem,
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
        if (_fileSystem.Overlay.FileExists(Uri))
        {
            return true;
        }

        return !_fileSystem.IsMaskedFromBase(RelativePathFromRoot) && _fileSystem.Base.FileExists(Uri);
    }

    public async ValueTask<string> ReadAllTextAsync(CancellationToken cancellationToken = default)
    {
        if (_fileSystem.Overlay.FileExists(Uri))
        {
            return await _fileSystem.Overlay.GetFile(Uri).ReadAllTextAsync(cancellationToken).ConfigureAwait(false);
        }

        if (_fileSystem.IsMaskedFromBase(RelativePathFromRoot))
        {
            throw new FileNotFoundException("File not found.", RelativePathFromRoot);
        }

        return await _fileSystem.Base.GetFile(Uri).ReadAllTextAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask WriteAllTextAsync(string content, CancellationToken cancellationToken = default)
    {
        _fileSystem.ClearMasksForOverlayWrite(RelativePathFromRoot);
        await _fileSystem.Overlay.GetFile(Uri).WriteAllTextAsync(content, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Stream> OpenReadAsync(CancellationToken cancellationToken = default)
    {
        if (_fileSystem.Overlay.FileExists(Uri))
        {
            return await _fileSystem.Overlay.GetFile(Uri).OpenReadAsync(cancellationToken).ConfigureAwait(false);
        }

        if (_fileSystem.IsMaskedFromBase(RelativePathFromRoot))
        {
            throw new FileNotFoundException("File not found.", RelativePathFromRoot);
        }

        return await _fileSystem.Base.GetFile(Uri).OpenReadAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Stream> OpenWriteAsync(bool overwrite = true, CancellationToken cancellationToken = default)
    {
        _fileSystem.ClearMasksForOverlayWrite(RelativePathFromRoot);
        return await _fileSystem.Overlay.GetFile(Uri).OpenWriteAsync(overwrite, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask DeleteAsync(CancellationToken cancellationToken = default)
    {
        await _fileSystem.Overlay.GetFile(Uri).DeleteAsync(cancellationToken).ConfigureAwait(false);
        _fileSystem.TombstoneFile(RelativePathFromRoot);
    }

    public async ValueTask<DateTimeOffset?> GetLastWriteTimeUtcAsync(CancellationToken cancellationToken = default)
    {
        if (_fileSystem.Overlay.FileExists(Uri))
        {
            return await _fileSystem.Overlay.GetFile(Uri).GetLastWriteTimeUtcAsync(cancellationToken).ConfigureAwait(false);
        }

        if (_fileSystem.IsMaskedFromBase(RelativePathFromRoot))
        {
            return null;
        }

        return await _fileSystem.Base.GetFile(Uri).GetLastWriteTimeUtcAsync(cancellationToken).ConfigureAwait(false);
    }
}
