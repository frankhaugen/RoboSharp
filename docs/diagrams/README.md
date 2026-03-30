# Diagrams

This folder holds **generated** Markdown with embedded **Mermaid** diagrams (plus this README).

| File | Contents |
| ---- | -------- |
| [project-references.md](project-references.md) | `ProjectReference` graph for all projects under `src/` and `tests/` |
| [nuget-references.md](nuget-references.md) | Direct NuGet packages per project (versions from `Directory.Packages.props`) |
| [layer-map.md](layer-map.md) | Conceptual layer map aligned with `AGENTS.md` |

With `core.hooksPath` set to `.githooks`, a **`git commit`** runs diagram generation, then `UpdateSlnx.cs`, and stages outputs—no separate `dotnet` step in normal use. See [Build and test](../build.md).

To run only the diagram step without committing (for example hooks off or debugging):

```powershell
dotnet run --file .githooks/GenerateDocDiagrams.cs -- $PWD.Path
```

The solution uses **one solution folder per directory** (`/docs/`, `/docs/diagrams/`, `/docs/diagrams/architecture/`, …) as siblings under the solution root so the IDE tree matches the repo layout.
