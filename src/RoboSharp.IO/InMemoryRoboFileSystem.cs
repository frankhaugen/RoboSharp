using System.Collections.Concurrent;
using System.Text;

namespace RoboSharp.IO;

public sealed class InMemoryRoboFileSystem : IRoboFileSystem
{
    private readonly Uri _rootUri;
    private readonly ITextEncodingPolicy _encoding;
    private readonly IRoboPathService _paths;
    private readonly ConcurrentDictionary<string, Node> _nodes = new(StringComparer.Ordinal);

    public InMemoryRoboFileSystem(Uri rootUri, ITextEncodingPolicy? encodingPolicy = null, IRoboPathService? pathService = null)
    {
        ArgumentNullException.ThrowIfNull(rootUri);

        if (!rootUri.IsAbsoluteUri)
        {
            throw new ArgumentException("Root URI must be absolute.", nameof(rootUri));
        }

        _rootUri = RoboUriNormalizer.EnsureDirectoryUri(rootUri);
        _encoding = encodingPolicy ?? new Utf8TextEncodingPolicy();
        _paths = pathService ?? new RoboPathService();
        Root = new InMemoryRoboDirectory(this, _rootUri, parent: null, relativePathFromRoot: string.Empty);
    }

    public IRoboDirectory Root { get; }

    public IRoboFile GetFile(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ValidateUnderRoot(uri);
        var relative = _paths.GetRelativePath(_rootUri, uri);
        return new InMemoryRoboFile(this, uri, relative);
    }

    public IRoboDirectory GetDirectory(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ValidateUnderRoot(uri);
        var relative = _paths.GetRelativePath(_rootUri, uri);
        var parent = TryGetParentDirectory(relative);
        return new InMemoryRoboDirectory(this, uri, parent, relativePathFromRoot: relative);
    }

    public bool FileExists(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        if (!_paths.IsUnderRoot(_rootUri, uri))
        {
            return false;
        }

        var relative = _paths.GetRelativePath(_rootUri, uri);
        return _nodes.TryGetValue(relative, out var node) && !node.IsDirectory;
    }

    public bool DirectoryExists(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        if (!_paths.IsUnderRoot(_rootUri, uri))
        {
            return false;
        }

        var relative = _paths.GetRelativePath(_rootUri, uri);
        if (relative.Length == 0)
        {
            return true;
        }

        if (_nodes.TryGetValue(relative, out var node) && node.IsDirectory)
        {
            return true;
        }

        var prefix = relative + "/";
        return _nodes.Keys.Any(k => k.StartsWith(prefix, StringComparison.Ordinal));
    }

    internal bool NodeExists(string relativePath, bool asDirectory)
    {
        if (relativePath.Length == 0)
        {
            return true;
        }

        if (_nodes.TryGetValue(relativePath, out var node))
        {
            return node.IsDirectory == asDirectory;
        }

        if (asDirectory)
        {
            var prefix = relativePath + "/";
            return _nodes.Keys.Any(k => k.StartsWith(prefix, StringComparison.Ordinal));
        }

        return false;
    }

    internal async ValueTask<string> ReadAllTextAsync(string relativePath, CancellationToken cancellationToken)
    {
        if (!_nodes.TryGetValue(relativePath, out var node) || node.IsDirectory)
        {
            throw new FileNotFoundException("File not found.", relativePath);
        }

        cancellationToken.ThrowIfCancellationRequested();
        await Task.CompletedTask.ConfigureAwait(false);
        return _encoding.DefaultEncoding.GetString(node.Content ?? []);
    }

    internal async ValueTask WriteAllTextAsync(string relativePath, string content, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.CompletedTask.ConfigureAwait(false);
        var bytes = _encoding.DefaultEncoding.GetBytes(content);
        EnsureParentDirectories(relativePath);
        var now = DateTimeOffset.UtcNow;
        _nodes[relativePath] = new Node { IsDirectory = false, Content = bytes, LastWriteUtc = now };
    }

    internal async ValueTask<Stream> OpenReadAsync(string relativePath, CancellationToken cancellationToken)
    {
        if (!_nodes.TryGetValue(relativePath, out var node) || node.IsDirectory)
        {
            throw new FileNotFoundException("File not found.", relativePath);
        }

        cancellationToken.ThrowIfCancellationRequested();
        await Task.CompletedTask.ConfigureAwait(false);
        return new MemoryStream(node.Content ?? [], writable: false);
    }

    internal async ValueTask<Stream> OpenWriteAsync(string relativePath, bool overwrite, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.CompletedTask.ConfigureAwait(false);

        if (_nodes.TryGetValue(relativePath, out var existing))
        {
            if (existing.IsDirectory)
            {
                throw new IOException("Path refers to a directory.");
            }

            if (!overwrite)
            {
                throw new IOException("File already exists.");
            }
        }

        EnsureParentDirectories(relativePath);
        return new CommitOnDisposeMemoryStream(bytes =>
        {
            var committedAt = DateTimeOffset.UtcNow;
            _nodes[relativePath] = new Node { IsDirectory = false, Content = bytes, LastWriteUtc = committedAt };
        });
    }

    internal async ValueTask DeleteFileAsync(string relativePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.CompletedTask.ConfigureAwait(false);
        _nodes.TryRemove(relativePath, out _);
    }

    internal async ValueTask<DateTimeOffset?> GetLastWriteTimeUtcAsync(string relativePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.CompletedTask.ConfigureAwait(false);

        if (!_nodes.TryGetValue(relativePath, out var node) || node.IsDirectory)
        {
            return null;
        }

        return node.LastWriteUtc;
    }

    internal async ValueTask EnsureDirectoryExistsAsync(string relativePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.CompletedTask.ConfigureAwait(false);

        if (relativePath.Length == 0)
        {
            return;
        }

        if (_nodes.TryGetValue(relativePath, out var existing) && !existing.IsDirectory)
        {
            throw new IOException("Path conflicts with an existing file.");
        }

        CreateDirectoryChain(relativePath);
    }

    internal async ValueTask DeleteDirectoryAsync(string relativePath, bool recursive, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.CompletedTask.ConfigureAwait(false);

        if (relativePath.Length == 0)
        {
            if (!recursive)
            {
                throw new IOException("Cannot delete the root directory without recursive deletion.");
            }

            _nodes.Clear();
            return;
        }

        var prefix = relativePath + "/";
        if (!recursive)
        {
            if (_nodes.Keys.Any(k => k.StartsWith(prefix, StringComparison.Ordinal)))
            {
                throw new IOException("Directory is not empty.");
            }
        }

        _nodes.TryRemove(relativePath, out _);
        foreach (var key in _nodes.Keys.ToArray())
        {
            if (key.StartsWith(prefix, StringComparison.Ordinal))
            {
                _nodes.TryRemove(key, out _);
            }
        }
    }

    internal IEnumerable<IRoboDirectory> EnumerateDirectories(InMemoryRoboDirectory parent)
    {
        foreach (var name in GetImmediateChildNames(parent.RelativePathFromRoot))
        {
            if (!IsDirectoryChild(parent.RelativePathFromRoot, name))
            {
                continue;
            }

            var childRelative = parent.RelativePathFromRoot.Length == 0 ? name : parent.RelativePathFromRoot + "/" + name;
            var childUri = _paths.Combine(_rootUri, childRelative);
            yield return new InMemoryRoboDirectory(this, childUri, parent, relativePathFromRoot: childRelative);
        }
    }

    internal IEnumerable<IRoboFile> EnumerateFiles(InMemoryRoboDirectory parent)
    {
        foreach (var name in GetImmediateChildNames(parent.RelativePathFromRoot))
        {
            if (!IsFileChild(parent.RelativePathFromRoot, name))
            {
                continue;
            }

            var childRelative = parent.RelativePathFromRoot.Length == 0 ? name : parent.RelativePathFromRoot + "/" + name;
            var childUri = _paths.Combine(_rootUri, childRelative);
            yield return new InMemoryRoboFile(this, childUri, childRelative);
        }
    }

    internal InMemoryRoboDirectory GetChildDirectory(InMemoryRoboDirectory parent, string relativeName)
    {
        var childUri = _paths.Combine(RoboUriNormalizer.EnsureDirectoryUri(parent.Uri), relativeName);
        ValidateUnderRoot(childUri);
        var combinedRelative = _paths.GetRelativePath(_rootUri, childUri);
        return new InMemoryRoboDirectory(this, childUri, parent, combinedRelative);
    }

    internal InMemoryRoboFile GetChildFile(InMemoryRoboDirectory parent, string relativeName)
    {
        var childUri = _paths.Combine(RoboUriNormalizer.EnsureDirectoryUri(parent.Uri), relativeName);
        ValidateUnderRoot(childUri);
        var combinedRelative = _paths.GetRelativePath(_rootUri, childUri);
        return new InMemoryRoboFile(this, childUri, combinedRelative);
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
        return new InMemoryRoboDirectory(this, parentUri, grandparent, relativePathFromRoot: parentRelative);
    }

    private void ValidateUnderRoot(Uri uri)
    {
        if (!_paths.IsUnderRoot(_rootUri, uri))
        {
            throw new ArgumentOutOfRangeException(nameof(uri), "URI is not under the file system root.");
        }
    }

    private void EnsureParentDirectories(string fileRelativePath)
    {
        var parent = GetParentRelative(fileRelativePath);
        if (parent.Length == 0)
        {
            return;
        }

        CreateDirectoryChain(parent);
    }

    private void CreateDirectoryChain(string directoryRelativePath)
    {
        var segments = directoryRelativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var current = new StringBuilder();
        var now = DateTimeOffset.UtcNow;
        for (var i = 0; i < segments.Length; i++)
        {
            if (current.Length > 0)
            {
                current.Append('/');
            }

            current.Append(segments[i]);
            var path = current.ToString();
            if (_nodes.TryGetValue(path, out var existing))
            {
                if (!existing.IsDirectory)
                {
                    throw new IOException("Path conflicts with an existing file.");
                }

                continue;
            }

            _nodes[path] = new Node { IsDirectory = true, Content = null, LastWriteUtc = now };
        }
    }

    internal IEnumerable<string> GetImmediateChildNamesForOverlay(string dirRel) => GetImmediateChildNames(dirRel);

    internal bool IsNamedDirectoryUnder(string parentRel, string name) => IsDirectoryChild(parentRel, name);

    internal bool IsNamedFileUnder(string parentRel, string name) => IsFileChild(parentRel, name);

    private IEnumerable<string> GetImmediateChildNames(string dirRel)
    {
        var prefix = dirRel.Length == 0 ? "" : dirRel + "/";
        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (var key in _nodes.Keys)
        {
            if (prefix.Length > 0 && !key.StartsWith(prefix, StringComparison.Ordinal))
            {
                continue;
            }

            var remainder = prefix.Length == 0 ? key : key[prefix.Length..];
            var slash = remainder.IndexOf('/');
            var name = slash < 0 ? remainder : remainder[..slash];
            if (name.Length == 0)
            {
                continue;
            }

            names.Add(name);
        }

        return names.Order(StringComparer.Ordinal);
    }

    private bool IsFileChild(string dirRel, string name)
    {
        var path = dirRel.Length == 0 ? name : dirRel + "/" + name;
        return _nodes.TryGetValue(path, out var n) && !n.IsDirectory;
    }

    private bool IsDirectoryChild(string dirRel, string name)
    {
        var path = dirRel.Length == 0 ? name : dirRel + "/" + name;
        if (_nodes.TryGetValue(path, out var n) && n.IsDirectory)
        {
            return true;
        }

        var prefix = path + "/";
        return _nodes.Keys.Any(k => k.StartsWith(prefix, StringComparison.Ordinal));
    }

    private static string GetParentRelative(string relativePath)
    {
        var idx = relativePath.LastIndexOf('/');
        return idx < 0 ? string.Empty : relativePath[..idx];
    }

    private sealed class Node
    {
        public required bool IsDirectory { get; init; }
        public byte[]? Content { get; set; }
        public DateTimeOffset LastWriteUtc { get; set; }
    }
}
