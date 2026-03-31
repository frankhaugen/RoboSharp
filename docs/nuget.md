# NuGet and central package management

## Central package management (CPM)

Package versions are defined once in [`Directory.Packages.props`](../Directory.Packages.props). Projects reference packages **without** a `Version` attribute:

```xml
<PackageReference Include="TUnit" />
```

Adding a new package:

1. Add `<PackageVersion Include="PackageId" Version="x.y.z" />` to `Directory.Packages.props`.
2. Add `<PackageReference Include="PackageId" />` to the project(s) that need it.

`CentralPackageTransitivePinningEnabled` is set to `true` so transitive versions are pinned consistently; see NuGet documentation if you need to adjust that behavior.

## Allowed dependencies

Per [`AGENTS.md`](../AGENTS.md), only these NuGet families are in scope unless policy changes:

- .NET BCL (framework references)
- `Microsoft.Extensions.*`
- `TUnit` (tests)
- `Microsoft.Testing.Extensions.TrxReport` (tests only — TRX output for CI from Microsoft.Testing.Platform; see `Directory.Build.props`)
- **Avalonia** (`Avalonia`, `Avalonia.Desktop`, `Avalonia.AvaloniaEdit`, `Avalonia.Themes.*`, `Avalonia.Fonts.*`, **`Avalonia.Diagnostics` for Debug-configuration `RoboSharp.Studio` builds only**, and **`AvaloniaMcp.Diagnostics` for Debug-configuration `RoboSharp.Studio` builds only** for optional [AvaloniaMcp](https://github.com/adirh3/AvaloniaMcp) tooling) as the approved cross-platform desktop host UI (see [`docs/studio/technology-stack.md`](studio/technology-stack.md) and [`docs/studio/avalonia-mcp.md`](studio/avalonia-mcp.md))
- **Spectre.Console** — **`RoboSharp.Player` only**, for the default terminal TUI after running a `.roboexe` (see [`docs/player/README.md`](player/README.md))

`Directory.Packages.props` includes a label reminding maintainers of that surface.

## Feed policy

[`nuget.config`](../nuget.config) uses `<clear />` under `packageSources` and then adds **nuget.org** only. That avoids NU1507 when using CPM alongside extra feeds that might exist in a developer’s global NuGet configuration.

If you must consume packages from another feed (for example in a fork), add the feed **and** either:

- use [package source mapping](https://learn.microsoft.com/nuget/consume-packages/package-source-mapping) so every package ID maps to exactly one source, or  
- keep a single feed if that matches your policy.

Do not expand package usage beyond what `AGENTS.md` allows without updating that document.
