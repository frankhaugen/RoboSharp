# Build pipeline integration

The compiler pipeline should consume a workspace, not raw random files.

```csharp
public interface ICompilerPipeline
{
    ValueTask<CompilationArtifacts> BuildAsync(
        IRoboWorkspace workspace,
        CancellationToken cancellationToken = default);
}
```

## Why

The workspace already knows:

- project identity
- source file set
- startup file
- build output paths
- active configuration
- runtime metadata

That is cleaner than passing many separate arguments.
