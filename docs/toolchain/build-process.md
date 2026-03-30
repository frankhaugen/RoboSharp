# Build process

End-to-end **build** turns a loaded project into artifacts under `obj/` and `bin/`, driven by the toolchain and workspace layout.

## Happy path

```text
.robosharp + .robo sources
  → resolve paths (workspace / IO)
  → RoboSharpCompiler.Compile (or pipeline equivalent)
  → emit per-phase artifacts
  → write .roboexe to bin/
```

Phases inside compile match [v1-compiler-spec.md](../compiler/v1-compiler-spec.md) §2–3: lex, parse, bind, lower, package.

## Configurations

- **Debug:** may emit syntax, bound, IL, PDB JSON, and executable ([v1-toolchain-spec.md](v1-toolchain-spec.md) §3).
- **Release:** must emit at least `.roboexe`; debug intermediates optional.

## Clean / rebuild

- **Clean:** remove `obj/` and configuration-specific `bin/` outputs for the project.
- **Rebuild:** clean then build ([v1-toolchain-spec.md](v1-toolchain-spec.md) §4).

## Incremental policy

v1 may **full rebuild** every time; acceptable for small teaching projects ([v1-toolchain-spec.md](v1-toolchain-spec.md) §6).

## Failure ordering

If compilation fails with errors, do not write later-stage artifacts as successful outputs ([v1-toolchain-spec.md](v1-toolchain-spec.md) §7). Partial dumps for teaching may be explicitly marked incomplete if the host chooses to support that.

## Studio vs CLI

Studio may compile from **unsaved buffers** via workspace overlay semantics ([../workspaces/studio-overlay-and-save.md](../workspaces/studio-overlay-and-save.md)); CLI builds read from persisted files.

## Related

- [project-format.md](project-format.md)
- [artifact-layout.md](artifact-layout.md)
- [../compiler/compilation-pipeline.md](../compiler/compilation-pipeline.md)
- [../workspaces/build-pipeline-integration.md](../workspaces/build-pipeline-integration.md)
