# Project format (`.robosharp`)

The RoboSharp **project file** names the program, entry source, included `.robo` files, and build/runtime settings. It is the workspace anchor for Studio and CLI builds.

## File role

- **Extension:** `.robosharp` (see [AGENTS.md](../../AGENTS.md) **File and artifact rules**).
- **Not** a container for all source: sources remain `.robo` files on disk or in overlay buffers.

## Expected contents (v1 direction)

Conceptual fields (exact serialization may be JSON, XML, or another chosen format when implemented):

- **Project format / version** — reject unknown major versions early.
- **Name** — display name.
- **Startup / entry** — path to the main `.robo` file (or explicit entry document id in workspace terms).
- **Source files** — glob or explicit list of compilation inputs.
- **Active builtin profile** — lesson/profile name for semantic gating ([../semantics/builtins-and-profiles.md](../semantics/builtins-and-profiles.md)).
- **Optional world path** — terrain/world JSON for runs ([../world/world-model.md](../world/world-model.md)).
- **Default configuration** — `Debug` vs `Release` ([v1-toolchain-spec.md](v1-toolchain-spec.md) §3).

## Validation

On open, the toolchain validates format, resolves paths via the workspace IO layer, and fails with clear diagnostics if files are missing ([v1-toolchain-spec.md](v1-toolchain-spec.md) §5).

## Related

- [build-process.md](build-process.md)
- [artifact-layout.md](artifact-layout.md)
- [../workspaces/project-loading.md](../workspaces/project-loading.md)
- [v1-toolchain-spec.md](v1-toolchain-spec.md)
