using RoboSharp.IO;

namespace RoboSharp.Workspaces;

public interface IRoboWorkspace
{
    IRoboFileSystem FileSystem { get; }

    IRoboDirectory Root { get; }

    IRoboFile ProjectFile { get; }

    RoboSharpProject Project { get; }

    string ActiveConfiguration { get; }

    IReadOnlyList<IRoboFile> GetSourceFiles();

    IRoboFile GetStartupSourceFile();

    IRoboDirectory GetIntermediateDirectory(string configuration);

    IRoboDirectory GetOutputDirectory(string configuration);

    IRoboFile GetArtifactFile(ArtifactKind artifactKind, string configuration, string? sourceFileRelative = null);
}
