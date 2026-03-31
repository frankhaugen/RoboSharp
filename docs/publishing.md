# Publishing (single-file executables)

RoboSharp can ship **one self-contained executable** per host using .NET’s **single-file publish** (compressed bundle with native runtime extracted on first run).

## When to use which host

| Host | Typical size | Use case |
| ---- | ------------- | -------- |
| **RoboSharp.Player** | Smaller | Run a compiled `.roboexe` (JSON) from the command line — best “single small exe” for sharing runs. |
| **RoboSharp.Studio** | Larger (Avalonia UI) | Full teaching IDE — still one file, but not “small”. |
| **RoboSharp.Web** | N/A | ASP.NET Core — publish as a folder/site, not a single-file console binary. |

## Quick publish (script)

From the repository root:

```powershell
./tools/publish-singlefile.ps1 -Project Player -Runtime win-x64
```

Output defaults to `artifacts/publish/win-x64/RoboSharp.Player/` with `RoboSharp.Player.exe` on Windows.

Other RIDs examples: `linux-x64`, `osx-arm64`, `win-arm64`.

## Manual `dotnet publish`

Opt-in property **`RoboSharpSingleFilePublish=true`** loads [`eng/RoboSharp.SingleFile.props`](../eng/RoboSharp.SingleFile.props) (imported by Player and Studio projects).

```powershell
dotnet publish src/RoboSharp.Player/RoboSharp.Player.csproj `
  -c Release -r win-x64 `
  -p:RoboSharpSingleFilePublish=true `
  -o ./dist/player
```

```powershell
dotnet publish src/RoboSharp.Studio/RoboSharp.Studio.csproj `
  -c Release -r win-x64 `
  -p:RoboSharpSingleFilePublish=true `
  -o ./dist/studio
```

You can cross-publish from any OS that supports the target RID (e.g. publish `win-x64` from Linux with the .NET SDK).

Self-contained **Player** is on the order of tens of MB (~38 MB for `win-x64` at time of writing); **Studio** (Avalonia) is larger (~48 MB). Sizes change with the .NET runtime version.

The publish folder is intentionally **just the main executable** (reference PDBs are stripped after `ComputeFilesToPublish` when `RoboSharpSingleFilePublish=true`).

## Smaller vs simpler trade-offs

- **Self-contained** (what we default to in `RoboSharp.SingleFile.props`): one download, includes the .NET runtime; larger on disk.
- **Framework-dependent** single file: add `-p:SelfContained=false` on the command line (overrides the props file for that invocation) — smaller exe, but recipients must install a matching .NET runtime.

Trimming (`PublishTrimmed`) is **not** enabled by default; turning it on may break DI/reflection paths — profile carefully before enabling for releases.

## GitHub Releases

### CI prereleases (every push to `main`)

When the **CI** workflow completes successfully on a **push** to `main`, a follow-up job creates a **GitHub prerelease** tagged `ci-<workflow_run_id>` (for example `ci-12345678901`). It attaches the same archives as a tagged release:

- `RoboSharp.Studio-ci-<id>-linux-x64.tar.gz`, `…-win-x64.tar.gz`
- `RoboSharp.Player-ci-<id>-linux-x64.tar.gz`, `…-win-x64.tar.gz`
- `RoboSharp.Web-ci-<id>-linux-x64.tar.gz`
- `SHA256SUMS.txt`

Embedded assembly version uses `0.0.0-ci.<run_number>`. Pull-request CI does **not** publish a release (only pushes to `main` do). For a formal, calendar-versioned release, use tags and `release.yml` below.

### Tagged releases (`release.yml`)

Tag-driven releases (`.github/workflows/release.yml`) publish **Studio**, **Player**, and **Web** and attach archives. Player and Studio use the same single-file switch when publishing.

**Version tags** use **`vyyyy.MM.dd.#`** (four parts, dot-separated). Example: `v2026.03.31.1` — year, month, day, and a per-day **build index** (`.1`, `.2`, …). Push the tag to `main` (or the branch you release from):

```bash
git tag v2026.03.31.1
git push origin v2026.03.31.1
```

The release’s **title** is `RoboSharp yyyy.MM.dd.#` (without the leading `v`). The **body** starts with a short intro (what each archive contains, how to verify `SHA256SUMS.txt`, links to docs at that tag), then GitHub appends **auto-generated** notes from commits and merged pull requests.

## See also

- [build.md](build.md) — day-to-day build and test
- [player/README.md](player/README.md) — Player CLI
