# Core workspace contracts

```csharp
public interface IRoboWorkspace
{
    IRoboFileSystem FileSystem { get; }
    IRoboDirectory Root { get; }
    IRoboFile ProjectFile { get; }

    RoboSharpProject Project { get; }

    IEnumerable<IRoboFile> GetSourceFiles();
    IRoboDirectory GetIntermediateDirectory(string configuration);
    IRoboDirectory GetOutputDirectory(string configuration);
}
```

Minimal shape, extended slightly:

```csharp
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

    IRoboFile GetArtifactFile(ArtifactKind artifactKind, string configuration, string? sourceFileName = null);
}
```

Types such as `IRoboFileSystem`, `IRoboFile`, and `IRoboDirectory` are defined in the IO layer; see [`docs/io/canonical-abstractions.md`](../io/canonical-abstractions.md).
