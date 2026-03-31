# Samples

## `hello.roboexe`

Minimal v1 JSON fake executable: entry function `TopLevel` (lowered top-level body) prints `42` via `print` (teaching IL: `PushInt`, `CallBuiltin` for `BuiltinId.Print`, `Return`).

Run with the Player from the repository root:

```powershell
dotnet run --project src/RoboSharp.Player/RoboSharp.Player.csproj -- samples/hello.roboexe
```

You should see `42` on standard output.

To produce your own `.roboexe`, compile RoboSharp source with the toolchain and serialize with `RoboExecutableJsonSerializer` (see `WorkspaceBuildService` and [toolchain/roboexe-format.md](../docs/toolchain/roboexe-format.md)).
