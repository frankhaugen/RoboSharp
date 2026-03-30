# Repository layout

## Top-level files

| Path | Role |
| ---- | ---- |
| `README.md` | Short entry point; links to `AGENTS.md` and `docs/` |
| `RoboSharp.slnx` | XML solution (SDK 10+), folders for `src/` and `tests/` |
| `Directory.Build.props` | Shared MSBuild properties: TFM, nullable, analyzers, artifacts output, test project defaults |
| `Directory.Build.targets` | Shared MSBuild targets (placeholder for repo-wide custom targets) |
| `Directory.Packages.props` | Central package versions (CPM) |
| `global.json` | SDK version band and `rollForward` |
| `nuget.config` | Package sources (nuget.org only after `<clear />`) |
| `.editorconfig` | Editor and .NET code style |
| `.gitignore` | Ignores `artifacts/`, `bin/`, `obj/`, IDE cruft, test results |
| `AGENTS.md` | Architecture, dependency rules, teaching mission |
| `.gitattributes` | Repo-wide `text=auto eol=lf` plus explicit/binary overrides |
| `.githooks/UpdateSlnx.cs` | .NET 10 **file-based** app (`#:property` overrides) that regenerates `RoboSharp.slnx` |
| `.githooks/` | Git hooks (`pre-commit`, `README.md`); set `core.hooksPath` to `.githooks` |

`RoboSharp.slnx` lists **docs**, **infrastructure** files, and all projects. Do not hand-edit project lists for long; run `dotnet run --file .githooks/UpdateSlnx.cs` or rely on the pre-commit hook (see [Build and test](build.md)).

## Documentation tree

- Hand-written pages live under `docs/` (nested directories on disk are fine).
- `docs/diagrams/` holds **generated** Mermaid Markdown from `.githooks/GenerateDocDiagrams.cs`; `docs/diagrams/architecture/` adds another level on disk. In `RoboSharp.slnx`, each distinct directory becomes a **top-level** `<Folder Name="/docs/.../"/>` (siblings under `<Solution>`), so the IDE shows nested sections without invalid nested-folder XML. Regenerate diagrams before `UpdateSlnx` (pre-commit does both).

## Source and test projects

`src/` and `tests/` mirror the layout described in [`AGENTS.md`](../AGENTS.md): language, semantics, IL, runtime, world, IO, workspaces, toolchain, application, hosting, and hosts (Player, Studio, Web), plus matching test projects and `RoboSharp.Architecture.Tests`.

## Dependency direction (summary)

Hosts and application layers depend inward. Inner layers do not reference hosts or UI. The full rule set and rationale are in `AGENTS.md` under **Dependency direction**.
