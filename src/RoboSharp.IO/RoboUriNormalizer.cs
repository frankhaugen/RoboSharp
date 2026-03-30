namespace RoboSharp.IO;

internal static class RoboUriNormalizer
{
    public static Uri EnsureDirectoryUri(Uri uri)
    {
        if (uri.Scheme == Uri.UriSchemeFile)
        {
            var path = uri.LocalPath;
            if (!path.EndsWith(Path.DirectorySeparatorChar) && !path.EndsWith(Path.AltDirectorySeparatorChar))
            {
                path += Path.DirectorySeparatorChar;
                return new Uri(path);
            }

            return uri;
        }

        if (uri.Scheme == RoboUriSchemes.Memory)
        {
            var path = uri.AbsolutePath;
            if (path.Length == 0)
            {
                return new UriBuilder(RoboUriSchemes.Memory, string.Empty, -1, "/").Uri;
            }

            if (path[^1] != '/')
            {
                return new UriBuilder(uri) { Path = path + "/" }.Uri;
            }

            return uri;
        }

        var absoluteUri = uri.AbsoluteUri;
        return absoluteUri.EndsWith('/') ? uri : new Uri(absoluteUri + '/', UriKind.Absolute);
    }

    public static bool IsStrictlyUnderOrEqualDirectory(Uri baseDirectory, Uri candidate)
    {
        var dir = EnsureDirectoryUri(baseDirectory);
        if (UriEqualsTrimmed(dir, candidate))
        {
            return true;
        }

        return dir.IsBaseOf(candidate);
    }

    public static bool UriEqualsTrimmed(Uri a, Uri b) =>
        string.Equals(a.AbsoluteUri.TrimEnd('/'), b.AbsoluteUri.TrimEnd('/'), StringComparison.Ordinal);
}
