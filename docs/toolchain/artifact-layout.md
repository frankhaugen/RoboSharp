# Toolchain artifact layout (`obj/` / `bin/`)

This page is the **toolchain-facing** view of where build outputs live. Workspace types resolve concrete paths via `IBuildArtifactLayout` ([../workspaces/artifact-layout.md](../workspaces/artifact-layout.md)).

## Directories

| Location | Role |
| -------- | ---- |
| `obj/<configuration>/` | Intermediate teaching artifacts (JSON dumps of pipeline stages) |
| `bin/<configuration>/` | Shippable output, primarily `.roboexe` |

Configuration names follow MSBuild-style `Debug` / `Release` unless the repo standardizes otherwise.

## Typical files

| File | Stage | Notes |
| ---- | ----- | ----- |
| `.roboast.json` | Syntax | Parse tree / syntax snapshot |
| `.robobind.json` | Semantics | Bound program or binding snapshot |
| `.roboil.json` | IL | Lowered instruction listing |
| `.robo.pdb.json` | Debug | Source ↔ IL mapping, local names |
| `.roboexe` | Package | Compiled executable ([roboexe-format.md](roboexe-format.md)) |

Exact naming is centralized in layout services; avoid scattering string literals across hosts.

## Emission order

Emit in pipeline order; see [v1-toolchain-spec.md](v1-toolchain-spec.md) §7 and [build-process.md](build-process.md).

## Related

- [roboexe-format.md](roboexe-format.md)
- [project-format.md](project-format.md)
- [../workspaces/artifact-layout.md](../workspaces/artifact-layout.md)
