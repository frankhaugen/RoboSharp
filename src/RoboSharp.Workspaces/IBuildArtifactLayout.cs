using RoboSharp.IO;

namespace RoboSharp.Workspaces;

public interface IBuildArtifactLayout
{
    IRoboDirectory GetIntermediateDirectory(IRoboWorkspace workspace, string configuration);

    IRoboDirectory GetOutputDirectory(IRoboWorkspace workspace, string configuration);

    IRoboFile GetSyntaxArtifact(IRoboWorkspace workspace, string configuration, string sourceFileRelative);

    IRoboFile GetBoundArtifact(IRoboWorkspace workspace, string configuration);

    IRoboFile GetIlArtifact(IRoboWorkspace workspace, string configuration);

    IRoboFile GetDebugSymbolsArtifact(IRoboWorkspace workspace, string configuration);

    IRoboFile GetExecutableArtifact(IRoboWorkspace workspace, string configuration);
}
