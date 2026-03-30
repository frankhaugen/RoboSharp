namespace RoboSharp.IO;

public sealed class RoboPathService : IRoboPathService
{
    public Uri Combine(Uri baseUri, string relativePath)
    {
        ArgumentNullException.ThrowIfNull(baseUri);
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);

        if (!baseUri.IsAbsoluteUri)
        {
            throw new ArgumentException("Base URI must be absolute.", nameof(baseUri));
        }

        var baseDirectory = RoboUriNormalizer.EnsureDirectoryUri(baseUri);
        var normalizedRelative = NormalizeRelativePathSegments(relativePath);
        if (normalizedRelative.Count == 0)
        {
            return baseDirectory;
        }

        var joined = string.Join('/', normalizedRelative);
        var resolved = new Uri(baseDirectory, joined);
        if (!RoboUriNormalizer.IsStrictlyUnderOrEqualDirectory(baseDirectory, resolved))
        {
            throw new ArgumentException("Relative path escapes its base directory.", nameof(relativePath));
        }

        return resolved;
    }

    public string GetRelativePath(Uri fromDirectory, Uri toNode)
    {
        ArgumentNullException.ThrowIfNull(fromDirectory);
        ArgumentNullException.ThrowIfNull(toNode);

        if (!fromDirectory.IsAbsoluteUri || !toNode.IsAbsoluteUri)
        {
            throw new ArgumentException("URIs must be absolute.");
        }

        var fromDir = RoboUriNormalizer.EnsureDirectoryUri(fromDirectory);
        var relative = fromDir.MakeRelativeUri(toNode);
        return Uri.UnescapeDataString(relative.ToString());
    }

    public bool IsUnderRoot(Uri root, Uri candidate)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(candidate);

        if (!root.IsAbsoluteUri || !candidate.IsAbsoluteUri)
        {
            return false;
        }

        var rootDirectory = RoboUriNormalizer.EnsureDirectoryUri(root);
        return rootDirectory.IsBaseOf(candidate) || UriEqualsNormalized(rootDirectory, candidate);
    }

    private static bool UriEqualsNormalized(Uri a, Uri b) =>
        string.Equals(a.AbsoluteUri.TrimEnd('/'), b.AbsoluteUri.TrimEnd('/'), StringComparison.Ordinal);

    private static List<string> NormalizeRelativePathSegments(string relativePath)
    {
        var stack = new List<string>();
        foreach (var segment in SplitPathSegments(relativePath))
        {
            if (segment is "." or "")
            {
                continue;
            }

            if (segment == "..")
            {
                if (stack.Count == 0)
                {
                    throw new ArgumentException("Relative path escapes its base directory.", nameof(relativePath));
                }

                stack.RemoveAt(stack.Count - 1);
                continue;
            }

            stack.Add(segment);
        }

        return stack;
    }

    private static IEnumerable<string> SplitPathSegments(string relativePath) =>
        relativePath.Split(['/', '\\'], StringSplitOptions.None);
}
