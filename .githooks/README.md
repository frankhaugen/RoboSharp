# Git hooks

This repository uses a **pre-commit** hook to regenerate `RoboSharp.slnx` so documentation and infrastructure files stay listed in the solution (see `.githooks/UpdateSlnx.cs`).

## One-time setup

From the repository root, point Git at this folder:

```sh
git config core.hooksPath .githooks
```

On Windows (PowerShell), the same command works in Git for Windows.

To use the default `.git/hooks` directory instead, copy `pre-commit` there and ensure the file is executable on Unix (`chmod +x .git/hooks/pre-commit`).

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download) on `PATH` so `dotnet run --file .githooks/UpdateSlnx.cs` works.

## Manual run

```sh
dotnet run --file .githooks/UpdateSlnx.cs -- "$(git rev-parse --show-toplevel)"
```

Then stage `RoboSharp.slnx` if it changed.
