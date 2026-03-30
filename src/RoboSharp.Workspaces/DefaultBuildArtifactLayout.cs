using RoboSharp.IO;

namespace RoboSharp.Workspaces;

public sealed class DefaultBuildArtifactLayout : IBuildArtifactLayout
{
    public IRoboDirectory GetIntermediateDirectory(IRoboWorkspace workspace, string configuration)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        WorkspacePathUtilities.ThrowIfInvalidConfiguration(configuration, nameof(configuration));

        return workspace.Root.GetDirectory("obj").GetDirectory(configuration);
    }

    public IRoboDirectory GetOutputDirectory(IRoboWorkspace workspace, string configuration)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        WorkspacePathUtilities.ThrowIfInvalidConfiguration(configuration, nameof(configuration));

        return workspace.Root.GetDirectory("bin").GetDirectory(configuration);
    }

    public IRoboFile GetSyntaxArtifact(IRoboWorkspace workspace, string configuration, string sourceFileRelative)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        WorkspacePathUtilities.ThrowIfInvalidConfiguration(configuration, nameof(configuration));
        if (string.IsNullOrWhiteSpace(sourceFileRelative))
        {
            throw new ArgumentException("Source file path is required.", nameof(sourceFileRelative));
        }

        string normalized;
        try
        {
            normalized = WorkspacePathUtilities.NormalizeRelativePath(sourceFileRelative, nameof(sourceFileRelative));
        }
        catch (ProjectLoadException ex)
        {
            throw new ArgumentException(ex.Message, nameof(sourceFileRelative), ex);
        }
        var artifactRelative = WorkspacePathUtilities.GetSyntaxArtifactRelativePath(normalized);
        var intermediate = GetIntermediateDirectory(workspace, configuration);
        var directoryPart = GetDirectoryPart(artifactRelative);
        var fileName = GetFileNamePart(artifactRelative);
        var dir = GetDescendantDirectory(intermediate, directoryPart);
        return dir.GetFile(fileName);
    }

    public IRoboFile GetBoundArtifact(IRoboWorkspace workspace, string configuration)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        WorkspacePathUtilities.ThrowIfInvalidConfiguration(configuration, nameof(configuration));

        var dir = GetIntermediateDirectory(workspace, configuration);
        var stem = WorkspacePathUtilities.GetProjectStem(workspace.ProjectFile);
        return dir.GetFile(stem + ".robobind.json");
    }

    public IRoboFile GetIlArtifact(IRoboWorkspace workspace, string configuration)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        WorkspacePathUtilities.ThrowIfInvalidConfiguration(configuration, nameof(configuration));

        var dir = GetIntermediateDirectory(workspace, configuration);
        var stem = WorkspacePathUtilities.GetProjectStem(workspace.ProjectFile);
        return dir.GetFile(stem + ".roboil.json");
    }

    public IRoboFile GetDebugSymbolsArtifact(IRoboWorkspace workspace, string configuration)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        WorkspacePathUtilities.ThrowIfInvalidConfiguration(configuration, nameof(configuration));

        var dir = GetIntermediateDirectory(workspace, configuration);
        var stem = WorkspacePathUtilities.GetProjectStem(workspace.ProjectFile);
        return dir.GetFile(stem + ".robo.pdb.json");
    }

    public IRoboFile GetExecutableArtifact(IRoboWorkspace workspace, string configuration)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        WorkspacePathUtilities.ThrowIfInvalidConfiguration(configuration, nameof(configuration));

        var dir = GetOutputDirectory(workspace, configuration);
        var stem = WorkspacePathUtilities.GetProjectStem(workspace.ProjectFile);
        return dir.GetFile(stem + ".roboexe");
    }

    private static string GetDirectoryPart(string relativePath)
    {
        WorkspacePathUtilities.SplitRelativePath(relativePath, out var dir, out _);
        return dir;
    }

    private static string GetFileNamePart(string relativePath)
    {
        WorkspacePathUtilities.SplitRelativePath(relativePath, out _, out var file);
        return file;
    }

    private IRoboDirectory GetDescendantDirectory(IRoboDirectory start, string relativeDirectory)
    {
        if (relativeDirectory.Length == 0)
        {
            return start;
        }

        var current = start;
        foreach (var segment in relativeDirectory.Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            current = current.GetDirectory(segment);
        }

        return current;
    }
}
