# IO helper services and error policy

## Helper services

Useful supporting services:

```csharp
public interface IRoboPathService
{
    Uri Combine(Uri baseUri, string relativePath);
    string GetRelativePath(Uri fromDirectory, Uri toNode);
    bool IsUnderRoot(Uri root, Uri candidate);
}
```

```csharp
public interface ITextEncodingPolicy
{
    Encoding DefaultEncoding { get; }
}
```

`IRoboPathService` is worth it: it prevents every layer from inventing path logic.

## IO error policy

IO failures are host/infrastructure failures, not RoboSharp language diagnostics.

Examples:

- access denied
- file missing during load
- malformed project path reference
- directory cannot be created

These should surface as structured workspace/build/load failures, not language parse errors.

Do not let `RoboSharp.IO` throw user-facing teaching diagnostics.
