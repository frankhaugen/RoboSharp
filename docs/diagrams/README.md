# Diagrams

This folder holds **generated** Markdown with embedded **Mermaid** diagrams (plus this README).

| File | Contents |
| ---- | -------- |
| [project-references.md](project-references.md) | `ProjectReference` graph for all projects under `src/` and `tests/` |
| [nuget-references.md](nuget-references.md) | Direct NuGet packages per project (versions from `Directory.Packages.props`) |
| [layer-map.md](layer-map.md) | Conceptual layer map aligned with `AGENTS.md` |

Regenerate before the solution file update:

```powershell
dotnet run --file .githooks/GenerateDocDiagrams.cs -- $PWD.Path
```

The **pre-commit** hook runs this automatically, then `UpdateSlnx.cs`. The solution uses **one solution folder per directory** (`/docs/`, `/docs/diagrams/`, `/docs/diagrams/architecture/`, …) as siblings under the solution root so the IDE tree matches the repo layout.
