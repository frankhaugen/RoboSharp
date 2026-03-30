# Dependency policy

This page summarizes **allowed dependencies and dependency direction** for RoboSharp. Authoritative detail is in [AGENTS.md](../../AGENTS.md); if this page disagrees, follow **AGENTS.md**.

## Allowed packages

- **.NET BCL** — standard library types.
- **`Microsoft.Extensions.*`** — configuration, DI, hosting primitives where needed.
- **`TUnit`** — tests only.
- **Avalonia** (`Avalonia`, `Avalonia.Desktop`, `Avalonia.Themes.*`, `Avalonia.Fonts.*`) — **`RoboSharp.Studio` only**, as the approved code-first desktop UI.

No other third-party frameworks, ORMs, serializers, UI stacks, mocking libraries, or utility package creep unless **AGENTS.md** is updated deliberately.

## Layer direction

Hosts and application depend **inward**. Inner layers (`Language`, `Semantics`, `IL`, `Runtime`, `World`, `IO`, `Workspaces`) must not reference hosts or UI types.

See [AGENTS.md](../../AGENTS.md) **Dependency direction** and [../diagrams/layer-map.md](../diagrams/layer-map.md).

## Enforcement

Architecture tests in `RoboSharp.Architecture.Tests` guard project references and package use; keep them green when adding projects.

## Related

- [design-principles.md](design-principles.md)
- [mission.md](mission.md)
- [../nuget.md](../nuget.md)
