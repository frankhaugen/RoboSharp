using RoboSharp.IO;

namespace RoboSharp.Workspaces;

public interface IProjectLoader
{
    ValueTask<RoboSharpProject> LoadAsync(IRoboFile projectFile, CancellationToken cancellationToken = default);

    ValueTask SaveAsync(RoboSharpProject project, IRoboFile projectFile, CancellationToken cancellationToken = default);
}
