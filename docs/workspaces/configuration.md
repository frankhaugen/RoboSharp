# Active configuration model

Keep configuration simple:

```text
Debug
Release
```

Recommended workspace property:

```csharp
public interface IConfigurationContext
{
    string ActiveConfiguration { get; }
    ValueTask SetActiveConfigurationAsync(string configuration, CancellationToken cancellationToken = default);
}
```

The workspace must resolve different obj/bin views cleanly.
