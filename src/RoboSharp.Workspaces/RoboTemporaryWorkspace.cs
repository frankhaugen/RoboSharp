using RoboSharp.IO;

namespace RoboSharp.Workspaces;

/// <summary>
/// Workspace backed by an in-memory tree, typically used for scratch scenarios while still exposing the normal <see cref="IRoboWorkspace"/> contract.
/// </summary>
public sealed class RoboTemporaryWorkspace : RoboWorkspaceBase
{
    internal RoboTemporaryWorkspace(
        IRoboFileSystem fileSystem,
        IRoboDirectory root,
        IRoboFile projectFile,
        RoboSharpProject project,
        IBuildArtifactLayout artifactLayout,
        string activeConfiguration,
        IRoboPathService? pathService = null)
        : base(fileSystem, root, projectFile, project, artifactLayout, activeConfiguration, pathService)
    {
    }

    /// <summary>
    /// Creates a memory-backed workspace rooted at <c>memory:///temp-workspace/</c>, materializes <paramref name="project"/> as <c>workspace.robosharp</c>, and loads it with the same validation as a disk project.
    /// </summary>
    public static async ValueTask<RoboTemporaryWorkspace> CreateInMemoryAsync(
        RoboSharpProject project,
        string activeConfiguration = "Debug",
        IBuildArtifactLayout? artifactLayout = null,
        IRoboPathService? pathService = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);

        var paths = pathService ?? new RoboPathService();
        var layout = artifactLayout ?? new DefaultBuildArtifactLayout();
        var rootUri = new UriBuilder(RoboUriSchemes.Memory, string.Empty, -1, "/temp-workspace/").Uri;
        var fs = new InMemoryRoboFileSystem(rootUri, pathService: paths);
        foreach (var relative in project.SourceFilesRelative)
        {
            var sourceFile = fs.Root.GetFile(relative);
            await sourceFile.WriteAllTextAsync(string.Empty, cancellationToken).ConfigureAwait(false);
        }

        var projectFile = fs.Root.GetFile("workspace.robosharp");
        var projectLoader = new ProjectLoader();
        await projectLoader.SaveAsync(project, projectFile, cancellationToken).ConfigureAwait(false);

        var workspaceLoader = new WorkspaceLoader(projectLoader, layout, paths);
        return await workspaceLoader.LoadTemporaryAsync(fs, projectFile, activeConfiguration, cancellationToken)
            .ConfigureAwait(false);
    }
}
