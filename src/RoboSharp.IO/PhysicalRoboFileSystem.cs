namespace RoboSharp.IO;

public sealed class PhysicalRoboFileSystem : IRoboFileSystem
{
    private readonly DirectoryInfo _rootDirectory;
    private readonly Uri _rootUri;
    private readonly ITextEncodingPolicy _encoding;
    private readonly IRoboPathService _paths;

    public PhysicalRoboFileSystem(DirectoryInfo rootDirectory, ITextEncodingPolicy? encodingPolicy = null, IRoboPathService? pathService = null)
    {
        ArgumentNullException.ThrowIfNull(rootDirectory);

        _rootDirectory = rootDirectory;
        _encoding = encodingPolicy ?? new Utf8TextEncodingPolicy();
        _paths = pathService ?? new RoboPathService();
        _rootUri = RoboUriNormalizer.EnsureDirectoryUri(new Uri(Path.GetFullPath(rootDirectory.FullName)));
        Root = new PhysicalRoboDirectory(this, _rootUri, parent: null, relativePathFromRoot: string.Empty);
    }

    public IRoboDirectory Root { get; }

    public IRoboFile GetFile(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ValidateUnderRoot(uri);
        var relative = _paths.GetRelativePath(_rootUri, uri);
        return new PhysicalRoboFile(this, uri, relative);
    }

    public IRoboDirectory GetDirectory(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ValidateUnderRoot(uri);
        var relative = _paths.GetRelativePath(_rootUri, uri);
        var parent = TryGetParentDirectory(relative);
        return new PhysicalRoboDirectory(this, uri, parent, relativePathFromRoot: relative);
    }

    public bool FileExists(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        if (!_paths.IsUnderRoot(_rootUri, uri))
        {
            return false;
        }

        var path = UriToLocalPath(uri);
        return File.Exists(path) && !Directory.Exists(path);
    }

    public bool DirectoryExists(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        if (!_paths.IsUnderRoot(_rootUri, uri))
        {
            return false;
        }

        var path = UriToLocalPath(uri);
        return Directory.Exists(path);
    }

    internal string UriToLocalPath(Uri uri)
    {
        if (!_paths.IsUnderRoot(_rootUri, uri))
        {
            throw new ArgumentOutOfRangeException(nameof(uri), "URI is not under the file system root.");
        }

        var relative = _paths.GetRelativePath(_rootUri, uri);
        if (relative.Length == 0)
        {
            return Path.GetFullPath(_rootDirectory.FullName);
        }

        var combined = Path.Combine(_rootDirectory.FullName, relative.Replace('/', Path.DirectorySeparatorChar));
        var full = Path.GetFullPath(combined);
        if (!RoboPathGuard.IsSubPathOrSame(Path.GetFullPath(_rootDirectory.FullName), full))
        {
            throw new IOException("Resolved path escaped the root directory.");
        }

        return full;
    }

    internal PhysicalRoboDirectory GetChildDirectory(PhysicalRoboDirectory parent, string relativeName)
    {
        var childUri = _paths.Combine(RoboUriNormalizer.EnsureDirectoryUri(parent.Uri), relativeName);
        ValidateUnderRoot(childUri);
        var combinedRelative = _paths.GetRelativePath(_rootUri, childUri);
        return new PhysicalRoboDirectory(this, childUri, parent, combinedRelative);
    }

    internal PhysicalRoboFile GetChildFile(PhysicalRoboDirectory parent, string relativeName)
    {
        var childUri = _paths.Combine(RoboUriNormalizer.EnsureDirectoryUri(parent.Uri), relativeName);
        ValidateUnderRoot(childUri);
        var combinedRelative = _paths.GetRelativePath(_rootUri, childUri);
        return new PhysicalRoboFile(this, childUri, combinedRelative);
    }

    internal IRoboDirectory? TryGetParentDirectory(string relativePath)
    {
        if (relativePath.Length == 0)
        {
            return null;
        }

        var parentRelative = GetParentRelative(relativePath);
        var parentUri = parentRelative.Length == 0 ? _rootUri : _paths.Combine(_rootUri, parentRelative);
        var grandparent = TryGetParentDirectory(parentRelative);
        return new PhysicalRoboDirectory(this, parentUri, grandparent, relativePathFromRoot: parentRelative);
    }

    internal ITextEncodingPolicy Encoding => _encoding;

    private void ValidateUnderRoot(Uri uri)
    {
        if (!_paths.IsUnderRoot(_rootUri, uri))
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
