# RoboSharp.Player

`RoboSharp.Player` is the **compiled-artifact runtime host**: a thin entry point over the same application/runtime concepts as other hosts. Policy and layering: [`AGENTS.md`](../../AGENTS.md).

## Run the Player (CLI)

Implementation: [`src/RoboSharp.Player/Program.cs`](../../src/RoboSharp.Player/Program.cs).

- **Input:** path to a v1 **JSON** `.roboexe` (same interchange as the toolchain writes; see [`../toolchain/roboexe-format.md`](../toolchain/roboexe-format.md)), plus optional flags (see `--help`).
- **Behavior:** deserialize, create a bordered empty world (16×16), run via `RoboSharpExecutionService.RunExecutableJsonAsync` (optional instruction cap), exit with [`RoboSharpExitCode`](../../src/RoboSharp.Application/RoboSharpExitCode.cs).
- **Flags:** `--max-steps <n>` — stop after *n* IL instructions (structured fault if the program has not finished). `--headless` is accepted as a no-op placeholder for future hosts.

From the repo root, using the sample artifact:

```powershell
dotnet run --project src/RoboSharp.Player/RoboSharp.Player.csproj -- samples/hello.roboexe
```

Details and IDE launch profile: [`../build.md`](../build.md#run-robosharp-player-compiled-roboexe-host). Sample file notes: [`../../samples/README.md`](../../samples/README.md).

## Lesson mode (direction)

In addition to running a bare `.roboexe`, the Player should be able to run in a **lesson context**:

| Mode | Behavior |
| ---- | -------- |
| **Free run** | Execute a `.roboexe` (or equivalent) without lesson metadata |
| **Lesson run** | Load lesson definition (profile, world, goals), run the program, evaluate goals, report pass/fail |

That supports classroom demos, headless checks, and parity with Studio’s teaching loop.

Details of goals, packs, and JSON shape: [../lessons/README.md](../lessons/README.md).
