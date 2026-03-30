# Dependency rules

RoboSharp enforces **one-way dependencies** so teaching layers stay replaceable and hosts stay thin.

## Allowed direction

```text
Hosts (Studio, Player, Web)
  → Application / Toolchain / Workspaces
  → Runtime / World / IL / Semantics / Language / IO
```

## Hard rules (summary)

- `Language` does not depend on Semantics, IL, Runtime, World, or hosts.
- `Semantics` depends on `Language` only where syntax input is required.
- `IL` depends on semantic/bound models, not UI.
- `Runtime` depends on IL and World contracts, not UI.
- `Workspaces` depends on IO; IO does not depend on Workspaces.
- No host or Avalonia references outside `RoboSharp.Studio` (and test harnesses as needed).

Full policy: [AGENTS.md](../../AGENTS.md) **Dependency direction**.

## Verification

`RoboSharp.Architecture.Tests` asserts reference graphs and allowed packages. Add new projects with an explicit home layer and update tests if seams change.

## Related

- [dependency-policy.md](../governance/dependency-policy.md)
- [pipeline-boundaries.md](pipeline-boundaries.md)
- [../diagrams/layer-map.md](../diagrams/layer-map.md)
