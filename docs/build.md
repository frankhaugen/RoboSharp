# Build and test

## Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download) matching [`global.json`](../global.json) (or compatible via `rollForward`).

## Line endings

[`.gitattributes`](../.gitattributes) sets `* text=auto eol=lf` so Git keeps text files as **LF** in the object database and on checkout. [`.editorconfig`](../.editorconfig) sets `end_of_line = lf` for editors that honor it.

On Windows, prefer `git config core.autocrlf false` (or `input`) in this repo so Git does not fight `.gitattributes`. After changing attributes, you can re-scan the index with `git add --renormalize .`.

## Commands

From the repository root:

```powershell
dotnet restore RoboSharp.slnx
dotnet build RoboSharp.slnx
dotnet test RoboSharp.slnx
```

Use `--configuration Release` for release builds.

## Publishing a single-file executable

For self-contained **Player** or **Studio** binaries (one executable per RID), see [publishing.md](publishing.md) and `tools/publish-singlefile.ps1`.

## Run RoboSharp Studio (desktop host)

From the repository root:

```powershell
dotnet run --project src/RoboSharp.Studio/RoboSharp.Studio.csproj
```

The Studio app is a code-first **Avalonia** shell with pipeline inspection tabs; see [`docs/studio/README.md`](studio/README.md).

## Run RoboSharp Player (compiled `.roboexe` host)

The **Player** is a thin console host that loads a v1 **JSON** `.roboexe`, runs it on a default Karel world, and prints **stdout/stderr** with a process exit code (see [`docs/player/README.md`](player/README.md) and [`toolchain/v1-toolchain-spec.md`](toolchain/v1-toolchain-spec.md)).

From the repository root, using the checked-in sample:

```powershell
dotnet run --project src/RoboSharp.Player/RoboSharp.Player.csproj -- samples/hello.roboexe
```

General form:

```powershell
dotnet run --project src/RoboSharp.Player/RoboSharp.Player.csproj -- path\to\your.roboexe
```

Optional cap on IL steps (matches toolchain spec intent for bounded runs):

```powershell
dotnet run --project src/RoboSharp.Player/RoboSharp.Player.csproj -- --max-steps 5000 path\to\your.roboexe
```

Use `RoboSharp.Player --help` for the full option list.

After `dotnet build`, you can also run the built assembly under `artifacts/` (layout depends on `Directory.Build.props`) and pass the `.roboexe` path as the only argument.

Visual Studio / Rider / VS Code with C# Dev Kit: set **RoboSharp.Player** as the startup project and use the launch profile **RoboSharp.Player (samples/hello.roboexe)** (see `src/RoboSharp.Player/Properties/launchSettings.json`).

## Solution file and pre-commit hook

[`RoboSharp.slnx`](../RoboSharp.slnx) is **generated** after diagram docs. The [.NET 10 file-based app](../.githooks/GenerateDocDiagrams.cs) **`.githooks/GenerateDocDiagrams.cs`** writes Markdown with Mermaid under [`docs/diagrams/`](diagrams/README.md) (project references, NuGet references, layer map). Then [`.githooks/UpdateSlnx.cs`](../.githooks/UpdateSlnx.cs) rewrites the solution so:

- every directory under `docs/` becomes its own **sibling** solution folder next to `/docs/` (for example `/docs/diagrams/`, `/docs/diagrams/architecture/`), each holding only `<File/>` entries for that directory—no `<Folder>` nested inside `/docs/`, so the IDE shows a tree of sections;
- agreed **infrastructure** files (MSBuild, NuGet, `global.json`, `AGENTS.md`, license, hooks, etc.) appear under `/infrastructure/`;
- all `src/**/*.csproj` and `tests/**/*.csproj` are listed under `/src/` and `/tests/`.

**Normal workflow:** configure Git once, then use **`git commit`**; the pre-commit hook runs **GenerateDocDiagrams**, then **UpdateSlnx**, then `git add` on `docs/` and `RoboSharp.slnx`. You do not need to run those `dotnet` commands yourself before committing.

```sh
git config core.hooksPath .githooks
```

Details: [`.githooks/README.md`](../.githooks/README.md).

**If hooks are disabled or you are debugging the scripts**, regenerate from the repo root:

```powershell
dotnet run --file .githooks/GenerateDocDiagrams.cs -- $PWD.Path
dotnet run --file .githooks/UpdateSlnx.cs -- $PWD.Path
# or pass (git rev-parse --show-toplevel) for the root argument
```

Then stage `docs/` and `RoboSharp.slnx` if they changed.

The file-based app uses `#:property` directives at the top of `.githooks/UpdateSlnx.cs` to override repo-wide MSBuild settings (for example `UseArtifactsOutput`, `TreatWarningsAsErrors`, `PublishAot`) so hook runs stay lightweight.

In **CI**, you can enforce an up-to-date solution file with:

```sh
dotnet run --file .githooks/GenerateDocDiagrams.cs -- "$(pwd)"
dotnet run --file .githooks/UpdateSlnx.cs -- "$(pwd)"
git diff --exit-code RoboSharp.slnx
git diff --exit-code docs/
```

## Build output layout

The repo enables the .NET SDK **artifacts output** layout (`UseArtifactsOutput` in [`Directory.Build.props`](../Directory.Build.props)). Compiled binaries and test outputs go under:

`artifacts/bin/<ProjectName>/<configuration>/`

Intermediate files still use each project’s `obj` folder under the project directory (SDK default). The root [`artifacts/`](../artifacts/) directory is listed in [`.gitignore`](../.gitignore).

## Local CI parity and releases

**CI** finishing (including on `main`) does **not** start a deployment or GitHub Release. **CD** (archives + GitHub Release) runs only when you push a calendar **version tag** — see [`.github/workflows/release.yml`](../.github/workflows/release.yml) and [publishing.md](publishing.md).

The GitHub Actions **CI** job (`.github/workflows/ci.yml`) delegates build, test, and generated-file checks to [`tools/ci-verify.sh`](../tools/ci-verify.sh) via [`.github/actions/robo-build-verify`](../.github/actions/robo-build-verify/action.yml). Run the same sequence locally from the repo root:

- **Windows (PowerShell):** [`tools/ci-local.ps1`](../tools/ci-local.ps1) (uses Git Bash’s `bash` when available so behavior matches CI; otherwise runs equivalent `dotnet`/`git` commands).
- **Bash (Git Bash, WSL, Linux, macOS):** `bash tools/ci-verify.sh`  
  Optional release-style versioned build: `PACKAGE_VERSION=2026.03.31.1 bash tools/ci-verify.sh`

On GitHub Actions, the same script leaves **TRX** files under the runner temp `test-results` folder (artifact `ci-test-results-*`) and the workflow also publishes **RoboSharp.Studio** for **linux-x64** and **win-x64** (artifact `studio-ci-*`, self-contained single-file style).

The **Release** workflow (see [`.github/workflows/release.yml`](../.github/workflows/release.yml)) runs on tags **`vyyyy.MM.dd.#`** (four parts, calendar-style; **example** `v2026.03.31.1` for the first release of that day). It adds versioned publish, tarballs, `SHA256SUMS.txt`, and a GitHub Release whose **description** is a human-written intro (table of assets, verify command, doc links) **prepended** to GitHub’s auto-generated notes. To reproduce packaging locally (without creating a GitHub Release): [`tools/release-pack-local.ps1`](../tools/release-pack-local.ps1) `-Version` `2026.03.31.1` (no leading `v`).

## Testing (TUnit)

Tests use **[TUnit](https://tunit.dev/)** on **Microsoft.Testing.Platform** (not VSTest). Keep these in mind:

- **Await every** `Assert.That(...)` — without `await`, assertions do not run ([awaiting assertions](https://tunit.dev/docs/assertions/awaiting)).
- Prefer **`await Assert.That(() => ...).Throws<T>()`** for expected exceptions ([exception assertions](https://tunit.dev/docs/assertions/exceptions)).
- **CI reporting:** `tools/ci-verify.sh` uses **`--report-trx`**, **`--timeout 15m`**, and **`--disable-logo`**. The **`robo-build-verify`** composite action sets **`TUNIT_GITHUB_REPORTER_STYLE=collapsible`** and **`TESTINGPLATFORM_TELEMETRY_OPTOUT=1`** for GitHub-hosted runs ([CI/CD reporting](https://tunit.dev/docs/execution/ci-cd-reporting)).
- More tips: [best practices](https://tunit.dev/docs/guides/best-practices), [parallelism](https://tunit.dev/docs/execution/parallelism).

## Continuous integration

When `CI`, `GITHUB_ACTIONS`, or `TF_BUILD` is set, [`Directory.Build.props`](../Directory.Build.props) sets `ContinuousIntegrationBuild` for deterministic, CI-friendly behavior. Individual pipelines can set additional properties as needed.

## Code style

[`.editorconfig`](../.editorconfig) applies to C#, MSBuild, JSON, and Markdown. `EnforceCodeStyleInBuild` and `TreatWarningsAsErrors` are enabled globally in `Directory.Build.props`, so new warnings fail the build.
