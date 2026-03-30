# Dependency injection

RoboSharp uses **`Microsoft.Extensions.DependencyInjection`** at composition roots to wire filesystems, workspaces, compilers, and host services.

## Principles (from AGENTS.md)

- Register services in **composition roots** (e.g. hosting extensions, Studio bootstrap), not static service locators.
- Prefer **constructor injection** and **explicit registrations** over assembly scanning.
- Use **interfaces at real seams** (filesystem, workspace, pipeline, runtime host); avoid an interface per class.

## Typical registration areas

- **IO:** physical vs in-memory `IFileSystem` / directory abstractions.
- **Workspaces:** workspace factory, `IBuildArtifactLayout`, session/document services.
- **Toolchain:** compiler, pipeline, executable serializer.
- **Application:** run/debug facades consumed by Studio or Player.
- **Studio-only:** Avalonia views, view models, panel factories.

## Multiple hosts

`RoboSharp.Hosting` (when used) holds **shared** registration extensions so Studio, Player, and tests reuse the same inner graph with different outer UI.

## Related

- [io-workspace-overview.md](io-workspace-overview.md)
- [runtime-hosts.md](runtime-hosts.md)
- [../workspaces/concrete-types-and-di.md](../workspaces/concrete-types-and-di.md)
