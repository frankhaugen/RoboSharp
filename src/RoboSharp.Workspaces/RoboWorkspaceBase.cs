using RoboSharp.IO;

namespace RoboSharp.Workspaces;

public abstract class RoboWorkspaceBase : IRoboWorkspace
{
    private readonly IRoboPathService _paths;
    private readonly IBuildArtifactLayout _artifactLayout;
    private IReadOnlyList<IRoboFile>? _sourceFiles;

    protected RoboWorkspaceBase(
        IRoboFileSystem fileSystem,
        IRoboDirectory root,
        IRoboFile projectFile,
        RoboSharpProject project,
        IBuildArtifactLayout artifactLayout,
        string activeConfiguration,
        IRoboPathService? pathService = null)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(projectFile);
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(artifactLayout);
        WorkspacePathUtilities.ThrowIfInvalidConfiguration(activeConfiguration, nameof(activeConfiguration));

        FileSystem = fileSystem;
        Root = root;
        ProjectFile = projectFile;
        Project = project;
        _artifactLayout = artifactLayout;
        ActiveConfiguration = activeConfiguration;
        _paths = pathService ?? new RoboPathService();
    }

    public IRoboFileSystem FileSystem { get; }

    public IRoboDirectory Root { get; }

    public IRoboFile ProjectFile { get; }

    public RoboSharpProject Project { get; }

    public string ActiveConfiguration { get; }

    public IReadOnlyList<IRoboFile> GetSourceFiles() =>
        _sourceFiles ??= Project.SourceFilesRelative
            .Select(rel => FileSystem.GetFile(_paths.Combine(Root.Uri, rel)))
            .ToList();

    public IRoboFile GetStartupSourceFile() =>
        FileSystem.GetFile(_paths.Combine(Root.Uri, Project.StartupFileRelative));

    public IRoboDirectory GetIntermediateDirectory(string configuration) =>
        _artifactLayout.GetIntermediateDirectory(this, configuration);

    public IRoboDirectory GetOutputDirectory(string configuration) =>
        _artifactLayout.GetOutputDirectory(this, configuration);

    public IRoboFile GetArtifactFile(ArtifactKind artifactKind, string configuration, string? sourceFileRelative = null)
    {
        WorkspacePathUtilities.ThrowIfInvalidConfiguration(configuration, nameof(configuration));

        return artifactKind switch
        {
            ArtifactKind.SyntaxTree when sourceFileRelative is null =>
                throw new ArgumentException("Syntax artifacts require a source file path.", nameof(sourceFileRelative)),
            ArtifactKind.SyntaxTree => _artifactLayout.GetSyntaxArtifact(this, configuration, sourceFileRelative),
            ArtifactKind.BoundProgram => _artifactLayout.GetBoundArtifact(this, configuration),
            ArtifactKind.Il => _artifactLayout.GetIlArtifact(this, configuration),
            ArtifactKind.DebugSymbols => _artifactLayout.GetDebugSymbolsArtifact(this, configuration),
            ArtifactKind.Executable => _artifactLayout.GetExecutableArtifact(this, configuration),
            _ => throw new ArgumentOutOfRangeException(nameof(artifactKind)),
        };
    }
}
