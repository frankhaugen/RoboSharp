namespace RoboSharp.IO;

public sealed class OverlayRoboFileSystem : IRoboFileSystem
{
    private readonly IRoboFileSystem _base;
    private readonly InMemoryRoboFileSystem _overlay;
    private readonly IRoboPathService _paths;
    private readonly object _gate = new();
    private readonly HashSet<string> _fileTombstones = new(StringComparer.Ordinal);
    private readonly HashSet<string> _directoryTombstones = new(StringComparer.Ordinal);

    public OverlayRoboFileSystem(IRoboFileSystem baseFileSystem, InMemoryRoboFileSystem overlayFileSystem)
    {
        ArgumentNullException.ThrowIfNull(baseFileSystem);
        ArgumentNullException.ThrowIfNull(overlayFileSystem);

        if (!RoboUriNormalizer.UriEqualsTrimmed(baseFileSystem.Root.Uri, overlayFileSystem.Root.Uri))
        {
            throw new ArgumentException("Base and overlay file systems must share the same root URI.", nameof(overlayFileSystem));
        }

        _base = baseFileSystem;
        _overlay = overlayFileSystem;
        _paths = new RoboPathService();
        Root = new OverlayRoboDirectory(this, baseFileSystem.Root.Uri, parent: null, relativePathFromRoot: string.Empty);
    }

    public IRoboDirectory Root { get; }

    internal IRoboFileSystem Base => _base;

    internal InMemoryRoboFileSystem Overlay => _overlay;

    internal IRoboPathService Paths => _paths;

    public IRoboFile GetFile(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ValidateUnderRoot(uri);
        var relative = _paths.GetRelativePath(Root.Uri, uri);
        var parent = TryGetParentDirectory(relative);
        return new OverlayRoboFile(this, uri, parent, relative);
    }

    public IRoboDirectory GetDirectory(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ValidateUnderRoot(uri);
        var relative = _paths.GetRelativePath(Root.Uri, uri);
        var parent = TryGetParentDirectory(relative);
        return new OverlayRoboDirectory(this, uri, parent, relative);
    }

    public bool FileExists(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        if (!_paths.IsUnderRoot(Root.Uri, uri))
        {
            return false;
        }

        if (_overlay.FileExists(uri))
        {
            return true;
        }

        var relative = _paths.GetRelativePath(Root.Uri, uri);
        return !IsMaskedFromBase(relative) && _base.FileExists(uri);
    }

    public bool DirectoryExists(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        if (!_paths.IsUnderRoot(Root.Uri, uri))
        {
            return false;
        }

        var relative = _paths.GetRelativePath(Root.Uri, uri);
        if (relative.Length == 0)
        {
            return true;
        }

        if (_overlay.DirectoryExists(uri))
        {
            return true;
        }

        if (IsMaskedFromBase(relative))
        {
            return false;
        }

        return _base.DirectoryExists(uri);
    }

    internal IRoboDirectory? TryGetParentDirectory(string relativePath)
    {
        if (relativePath.Length == 0)
        {
            return null;
        }

        var parentRelative = GetParentRelative(relativePath);
        var parentUri = parentRelative.Length == 0 ? Root.Uri : _paths.Combine(Root.Uri, parentRelative);
        var grandparent = TryGetParentDirectory(parentRelative);
        return new OverlayRoboDirectory(this, parentUri, grandparent, parentRelative);
    }

    internal OverlayRoboDirectory GetChildDirectory(OverlayRoboDirectory parent, string relativeName)
    {
        var childUri = _paths.Combine(RoboUriNormalizer.EnsureDirectoryUri(parent.Uri), relativeName);
        ValidateUnderRoot(childUri);
        var combinedRelative = _paths.GetRelativePath(Root.Uri, childUri);
        return new OverlayRoboDirectory(this, childUri, parent, combinedRelative);
    }

    internal OverlayRoboFile GetChildFile(OverlayRoboDirectory parent, string relativeName)
    {
        var childUri = _paths.Combine(RoboUriNormalizer.EnsureDirectoryUri(parent.Uri), relativeName);
        ValidateUnderRoot(childUri);
        var combinedRelative = _paths.GetRelativePath(Root.Uri, childUri);
        return new OverlayRoboFile(this, childUri, parent, combinedRelative);
    }

    internal bool IsMaskedFromBase(string relativePath)
    {
        lock (_gate)
        {
            if (_fileTombstones.Contains(relativePath))
            {
                return true;
            }

            foreach (var dir in _directoryTombstones)
            {
                if (relativePath.Equals(dir, StringComparison.Ordinal))
                {
                    return true;
                }

                if (relativePath.StartsWith(dir + "/", StringComparison.Ordinal))
                {
                    return true;
                }
            }
        }

        return false;
    }

    internal void ClearMasksForOverlayWrite(string relativePath)
    {
        lock (_gate)
        {
            _fileTombstones.Remove(relativePath);

            foreach (var d in _directoryTombstones.ToArray())
            {
                if (relativePath.Equals(d, StringComparison.Ordinal) || relativePath.StartsWith(d + "/", StringComparison.Ordinal))
                {
                    _directoryTombstones.Remove(d);
                }
            }
        }
    }

    internal void TombstoneFile(string relativePath)
    {
        lock (_gate)
        {
            _fileTombstones.Add(relativePath);
        }
    }

    internal void TombstoneDirectory(string relativePath)
    {
        lock (_gate)
        {
            _directoryTombstones.Add(relativePath);
        }
    }

    private void ValidateUnderRoot(Uri uri)
    {
        if (!_paths.IsUnderRoot(Root.Uri, uri))
        {
            throw new ArgumentOutOfRangeException(nameof(uri), "URI is not under the file system root.");
        }
    }

    private static string GetParentRelative(string relativePath)
    {
        var idx = relativePath.LastIndexOf('/');
        return idx < 0 ? string.Empty : relativePath[..idx];
    }
}
