namespace RoboSharp.IO;

internal static class RoboNodeNames
{
    public static string GetName(Uri uri)
    {
        if (uri.Scheme == Uri.UriSchemeFile)
        {
            var path = uri.LocalPath.TrimEnd('/', '\\');
            return Path.GetFileName(path);
        }

        var absolutePath = uri.AbsolutePath.TrimEnd('/');
        var idx = absolutePath.LastIndexOf('/');
        return idx < 0 ? absolutePath : absolutePath[(idx + 1)..];
    }
}
