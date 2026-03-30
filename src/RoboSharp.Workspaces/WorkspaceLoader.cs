using RoboSharp.IO;

namespace RoboSharp.Workspaces;

public sealed class WorkspaceLoader : IWorkspaceLoader
{
    private readonly IProjectLoader _projectLoader;
    private readonly IBuildArtifactLayout _artifactLayout;
    private readonly IRoboPathService _paths;

    public WorkspaceLoader(
        IProjectLoader? projectLoader = null,
        IBuildArtifactLayout? artifactLayout = null,
        IRoboPathService? pathService = null)
    {
        _projectLoader = projectLoader ?? new ProjectLoader();
        _artifactLayout = artifactLayout ?? new DefaultBuildArtifactLayout();
        _paths = pathService ?? new RoboPathService();
    }

    public async ValueTask<IRoboWorkspace> LoadAsync(
        IRoboFileSystem fileSystem,
        IRoboFile projectFile,
        string activeConfiguration = "Debug",
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);
        ArgumentNullException.ThrowIfNull(projectFile);
        WorkspacePathUtilities.ThrowIfInvalidConfiguration(activeConfiguration, nameof(activeConfiguration));

        var (root, project) = await LoadCoreAsync(fileSystem, projectFile, cancellationToken).ConfigureAwait(false);
        return new RoboProjectWorkspace(
            fileSystem,
            root,
            projectFile,
            project,
            _artifactLayout,
            activeConfiguration,
            _paths);
    }

    internal async ValueTask<RoboTemporaryWorkspace> LoadTemporaryAsync(
        IRoboFileSystem fileSystem,
        IRoboFile projectFile,
        string activeConfiguration,
        CancellationToken cancellationToken)
    {
        var (root, project) = await LoadCoreAsync(fileSystem, projectFile, cancellationToken).ConfigureAwait(false);
        return new RoboTemporaryWorkspace(
            fileSystem,
            root,
            projectFile,
            project,
            _artifactLayout,
            activeConfiguration,
            _paths);
    }

    private async Task<(IRoboDirectory Root, RoboSharpProject Project)> LoadCoreAsync(
        IRoboFileSystem fileSystem,
        IRoboFile projectFile,
        CancellationToken cancellationToken)
    {
        if (!projectFile.Exists())
        {
            throw new ProjectLoadException("Project file does not exist.");
        }

        var project = await _projectLoader.LoadAsync(projectFile, cancellationToken).ConfigureAwait(false);
        var root = projectFile.Parent;
        if (root is null)
        {
            throw new ProjectLoadException("Project file has no parent directory.");
        }

        if (!_paths.IsUnderRoot(fileSystem.Root.Uri, root.Uri))
        {
            throw new ProjectLoadException("Project root directory must be under the file system root.");
        }

        foreach (var relative in project.SourceFilesRelative)
        {
            var uri = _paths.Combine(root.Uri, relative);
            if (!_paths.IsUnderRoot(root.Uri, uri))
            {
                throw new ProjectLoadException($"Source file '{relative}' resolves outside the project directory.");
            }

            if (!fileSystem.FileExists(uri))
            {
                throw new ProjectLoadException($"Source file '{relative}' does not exist.");
            }
        }

        return (root, project);
    }
}
