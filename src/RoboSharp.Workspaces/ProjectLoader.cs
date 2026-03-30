using System.Text.Json;
using RoboSharp.IO;

namespace RoboSharp.Workspaces;

public sealed class ProjectLoader : IProjectLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public async ValueTask<RoboSharpProject> LoadAsync(IRoboFile projectFile, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(projectFile);

        if (!projectFile.Exists())
        {
            throw new ProjectLoadException("Project file does not exist.");
        }

        string text;
        try
        {
            text = await projectFile.ReadAllTextAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new ProjectLoadException("Failed to read project file.", ex);
        }

        RoboSharpProjectDocument? document;
        try
        {
            document = JsonSerializer.Deserialize<RoboSharpProjectDocument>(text, JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new ProjectLoadException("Project file is not valid JSON.", ex);
        }

        if (document is null)
        {
            throw new ProjectLoadException("Project file is empty or invalid.");
        }

        return ValidateAndBuild(document);
    }

    public async ValueTask SaveAsync(RoboSharpProject project, IRoboFile projectFile, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(projectFile);

        var document = new RoboSharpProjectDocument
        {
            Name = project.Name,
            StartupFile = project.StartupFileRelative,
            SourceFiles = project.SourceFilesRelative.ToList(),
        };

        var text = JsonSerializer.Serialize(document, JsonOptions);
        await projectFile.WriteAllTextAsync(text, cancellationToken).ConfigureAwait(false);
    }

    private static RoboSharpProject ValidateAndBuild(RoboSharpProjectDocument document)
    {
        if (document.SourceFiles is null || document.SourceFiles.Count == 0)
        {
            throw new ProjectLoadException("Project must list at least one source file in 'sourceFiles'.");
        }

        var normalizedSources = new List<string>(document.SourceFiles.Count);
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var raw in document.SourceFiles)
        {
            var normalized = WorkspacePathUtilities.NormalizeRelativePath(raw, "sourceFiles entry");
            if (!seen.Add(normalized))
            {
                throw new ProjectLoadException($"Duplicate source file entry '{normalized}'.");
            }

            normalizedSources.Add(normalized);
        }

        if (string.IsNullOrWhiteSpace(document.StartupFile))
        {
            throw new ProjectLoadException("Project must specify 'startupFile'.");
        }

        var startup = WorkspacePathUtilities.NormalizeRelativePath(document.StartupFile, "startupFile");
        if (!normalizedSources.Contains(startup, StringComparer.Ordinal))
        {
            throw new ProjectLoadException($"Startup file '{startup}' must appear in 'sourceFiles'.");
        }

        var name = string.IsNullOrWhiteSpace(document.Name) ? null : document.Name.Trim();
        return new RoboSharpProject(name, normalizedSources, startup);
    }
}
