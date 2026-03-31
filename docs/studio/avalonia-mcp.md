# Avalonia MCP (agent / CLI inspection)

Optional workflow for inspecting and driving **RoboSharp Studio** while it is running: **[AvaloniaMcp](https://github.com/adirh3/AvaloniaMcp)** exposes a named-pipe diagnostic server inside the app and an MCP server (or CLI) that can read the visual tree, properties, screenshots, and more. MIT-licensed packages: **`AvaloniaMcp.Diagnostics`** (in-process) and **`AvaloniaMcp`** (dotnet tool).

## What is wired in this repo

- **Debug builds only:** `RoboSharp.Studio` references **`AvaloniaMcp.Diagnostics`** together with **`Avalonia.Diagnostics`** (see the `.csproj`). There is no UI; the pipe server starts when the process starts.
- **`Program.BuildAvaloniaApp()`** calls **`UseMcpDiagnostics()`** under **`#if DEBUG`** (see [`Program.cs`](../../src/RoboSharp.Studio/Program.cs)).

Release-configuration Studio binaries do **not** reference `AvaloniaMcp.Diagnostics` and do not open the MCP pipe.

## Install the MCP tool (once per clone)

From the repository root:

```bash
dotnet tool restore
```

This reads [`.config/dotnet-tools.json`](../../.config/dotnet-tools.json) and installs the **`avalonia-mcp`** command (package **`AvaloniaMcp`**, version pinned there). To upgrade later, bump the version in that manifest and run `dotnet tool restore` again.

Verify:

```bash
dotnet tool run avalonia-mcp -- --help
```

## Run Studio and attach

1. Build and run **RoboSharp.Studio** in **Debug** (debugger or `dotnet run --project src/RoboSharp.Studio/RoboSharp.Studio.csproj`).
2. Start the MCP server from the repo root (stdio transport for editors):

   ```bash
   dotnet tool run avalonia-mcp
   ```

   Or use the **`cli`** subcommand for one-off inspection without an MCP host; see [upstream README](https://github.com/adirh3/AvaloniaMcp).

If several Avalonia apps with MCP diagnostics are running, pass **`--pid`** (see upstream docs).

## Cursor / editor MCP configuration

Configure your editor to start the tool with **stdio**. Example for **Cursor** (workspace or user `mcp.json`): run the tool via **`dotnet tool run`** so the repo-local manifest is used when the shell’s working directory is the workspace:

```json
{
  "mcpServers": {
    "avalonia-mcp": {
      "command": "dotnet",
      "args": ["tool", "run", "avalonia-mcp"],
      "cwd": "${workspaceFolder}"
    }
  }
}
```

Adjust the key structure if your client uses a different schema (`mcpServers` vs `servers`, etc.). After changing config, restart the MCP connection or the editor.

## F12 DevTools vs MCP

- **F12** (**`Avalonia.Diagnostics`**) is the built-in visual inspector for humans.
- **AvaloniaMcp** is aimed at **automation and AI agents** (tree queries, screenshots, scripted input). You can use either or both during Debug sessions.

## Security note

The diagnostic server is intended for **local development**. Only run Debug Studio with MCP enabled in environments where you trust other processes on the machine.
