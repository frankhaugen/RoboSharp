using RoboSharp.IO;

namespace RoboSharp.Workspaces;

internal static class WorkspacePathUtilities
{
    public static void ThrowIfInvalidConfiguration(string configuration, string paramName)
    {
        if (string.IsNullOrWhiteSpace(configuration))
        {
            throw new ArgumentException("Configuration is required.", paramName);
        }

        if (configuration.Contains('/') || configuration.Contains('\\'))
        {
            throw new ArgumentException("Configuration must be a single path segment.", paramName);
        }
    }

    /// <summary>
    /// Normalizes a project-relative path: trims, uses forward slashes, rejects absolute paths and empty segments.
    /// </summary>
    public static string NormalizeRelativePath(string path, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ProjectLoadException($"{fieldName} cannot be empty.");
        }

        var trimmed = path.Trim();
        if (trimmed.StartsWith('/') || trimmed.StartsWith('\\'))
        {
            throw new ProjectLoadException($"{fieldName} must be relative to the project directory.");
        }

        var segments = trimmed.Split(['/', '\\'], StringSplitOptions.None);
        foreach (var segment in segments)
        {
            if (segment.Length == 0)
            {
                throw new ProjectLoadException($"{fieldName} contains an empty path segment.");
            }

            if (segment is "." or "..")
            {
                throw new ProjectLoadException($"{fieldName} cannot contain '.' or '..' segments.");
            }
        }

        return string.Join('/', segments);
    }

    public static void SplitRelativePath(string relativePath, out string directoryPart, out string fileName)
    {
        var idx = relativePath.LastIndexOf('/');
        if (idx < 0)
        {
            directoryPart = string.Empty;
            fileName = relativePath;
            return;
        }

        directoryPart = relativePath[..idx];
        fileName = relativePath[(idx + 1)..];
    }

    public static string GetSyntaxArtifactRelativePath(string sourceFileRelative)
    {
        SplitRelativePath(sourceFileRelative, out var dir, out var file);
        var stem = file.EndsWith(".robo", StringComparison.Ordinal)
            ? file[..^5]
            : Path.GetFileNameWithoutExtension(file);
        var artifactFile = stem + ".roboast.json";
        return dir.Length == 0 ? artifactFile : dir + "/" + artifactFile;
    }

    public static string GetProjectStem(IRoboFile projectFile) =>
        Path.GetFileNameWithoutExtension(projectFile.Name);
}
