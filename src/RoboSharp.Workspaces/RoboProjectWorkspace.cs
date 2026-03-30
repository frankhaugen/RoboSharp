using RoboSharp.IO;

namespace RoboSharp.Workspaces;

public sealed class RoboProjectWorkspace : RoboWorkspaceBase
{
    internal RoboProjectWorkspace(
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
}
