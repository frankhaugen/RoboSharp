# Git hooks

This repository uses a **pre-commit** hook that:

1. Runs **`.githooks/GenerateDocDiagrams.cs`** — writes Mermaid-based Markdown under `docs/diagrams/` (project graph, NuGet graph, layer map).
2. Runs **`.githooks/UpdateSlnx.cs`** — regenerates `RoboSharp.slnx` (nested `docs/**` folders, infrastructure files, projects).

Then it stages **`docs/`** and **`RoboSharp.slnx`**.

## One-time setup

From the repository root, point Git at this folder:

```sh
git config core.hooksPath .githooks
```

On Windows (PowerShell), the same command works in Git for Windows.

To use the default `.git/hooks` directory instead, copy `pre-commit` there and ensure the file is executable on Unix (`chmod +x .git/hooks/pre-commit`).

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download) on `PATH` for `dotnet run --file`.

## Manual run

```sh
ROOT="$(git rev-parse --show-toplevel)"
dotnet run --file .githooks/GenerateDocDiagrams.cs -- "$ROOT"
dotnet run --file .githooks/UpdateSlnx.cs -- "$ROOT"
```

Then stage `docs/` and `RoboSharp.slnx` if they changed.
