namespace RoboSharp.IO;

internal sealed class OverlayRoboDirectory : IRoboDirectory
{
    private readonly OverlayRoboFileSystem _fileSystem;

    public OverlayRoboDirectory(
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
        if (RelativePathFromRoot.Length == 0)
        {
            return true;
        }

        if (_fileSystem.Overlay.DirectoryExists(Uri))
        {
            return true;
        }

        if (_fileSystem.IsMaskedFromBase(RelativePathFromRoot))
        {
            return false;
        }

        return _fileSystem.Base.GetDirectory(Uri).Exists();
    }

    public IEnumerable<IRoboDirectory> EnumerateDirectories()
    {
        var names = new HashSet<string>(StringComparer.Ordinal);

        foreach (var name in _fileSystem.Overlay.GetImmediateChildNamesForOverlay(RelativePathFromRoot))
        {
            if (_fileSystem.Overlay.IsNamedDirectoryUnder(RelativePathFromRoot, name))
            {
                names.Add(name);
            }
        }

        foreach (var dir in _fileSystem.Base.GetDirectory(Uri).EnumerateDirectories())
        {
            var childRelative = CombineRelative(RelativePathFromRoot, dir.Name);
            if (_fileSystem.IsMaskedFromBase(childRelative))
            {
                continue;
            }

            names.Add(dir.Name);
        }

        foreach (var name in names.Order(StringComparer.Ordinal))
        {
            yield return _fileSystem.GetChildDirectory(this, name);
        }
    }

    public IEnumerable<IRoboFile> EnumerateFiles()
    {
        var names = new HashSet<string>(StringComparer.Ordinal);

        foreach (var name in _fileSystem.Overlay.GetImmediateChildNamesForOverlay(RelativePathFromRoot))
        {
            if (_fileSystem.Overlay.IsNamedFileUnder(RelativePathFromRoot, name))
            {
                names.Add(name);
            }
        }

        foreach (var file in _fileSystem.Base.GetDirectory(Uri).EnumerateFiles())
        {
            var childRelative = CombineRelative(RelativePathFromRoot, file.Name);
            if (_fileSystem.IsMaskedFromBase(childRelative))
            {
                continue;
            }

            names.Add(file.Name);
        }

        foreach (var name in names.Order(StringComparer.Ordinal))
        {
            yield return _fileSystem.GetChildFile(this, name);
        }
    }

    public IRoboDirectory GetDirectory(string relativeName) => _fileSystem.GetChildDirectory(this, relativeName);

    public IRoboFile GetFile(string relativeName) => _fileSystem.GetChildFile(this, relativeName);

    public async ValueTask EnsureExistsAsync(CancellationToken cancellationToken = default)
    {
        _fileSystem.ClearMasksForOverlayWrite(RelativePathFromRoot);
        await _fileSystem.Overlay.GetDirectory(Uri).EnsureExistsAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask DeleteAsync(bool recursive, CancellationToken cancellationToken = default)
    {
        if (RelativePathFromRoot.Length == 0)
        {
            throw new IOException("Cannot delete the root directory.");
        }

        await _fileSystem.Overlay.GetDirectory(Uri).DeleteAsync(recursive, cancellationToken).ConfigureAwait(false);
        _fileSystem.TombstoneDirectory(RelativePathFromRoot);
    }

    private static string CombineRelative(string parentRel, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return parentRel.Length == 0 ? name : parentRel + "/" + name;
    }
}
