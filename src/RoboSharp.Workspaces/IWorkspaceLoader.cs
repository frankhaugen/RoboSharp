using RoboSharp.IO;

namespace RoboSharp.Workspaces;

public interface IWorkspaceLoader
{
    ValueTask<IRoboWorkspace> LoadAsync(
        IRoboFileSystem fileSystem,
        IRoboFile projectFile,
        string activeConfiguration = "Debug",
        CancellationToken cancellationToken = default);
}
