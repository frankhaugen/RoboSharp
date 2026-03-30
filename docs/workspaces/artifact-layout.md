# Build artifact layout and kinds

## `IBuildArtifactLayout`

This deserves its own seam.

```csharp
public interface IBuildArtifactLayout
{
    IRoboDirectory GetIntermediateDirectory(IRoboWorkspace workspace, string configuration);
    IRoboDirectory GetOutputDirectory(IRoboWorkspace workspace, string configuration);

    IRoboFile GetSyntaxArtifact(IRoboWorkspace workspace, string configuration, string sourceFileName);
    IRoboFile GetBoundArtifact(IRoboWorkspace workspace, string configuration);
    IRoboFile GetIlArtifact(IRoboWorkspace workspace, string configuration);
    IRoboFile GetDebugSymbolsArtifact(IRoboWorkspace workspace, string configuration);
    IRoboFile GetExecutableArtifact(IRoboWorkspace workspace, string configuration);
}
```

This keeps obj/bin naming policy in one place.

RoboSharp intentionally has a visible artifact pipeline:

- `.roboast.json`
- `.robobind.json`
- `.roboil.json`
- `.robo.pdb.json`
- `.roboexe`

## Artifact kinds

Recommended enum:

```csharp
public enum ArtifactKind
{
    SyntaxTree,
    BoundProgram,
    Il,
    DebugSymbols,
    Executable
}
```

The workspace should not hardcode file names all over the place. Resolve them centrally via `IBuildArtifactLayout`.

See [`docs/toolchain/artifact-layout.md`](../toolchain/artifact-layout.md) for toolchain-wide conventions.
