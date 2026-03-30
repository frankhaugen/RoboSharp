# IO and workspaces — overview

The key constraint: **IO decides where bytes live; workspace decides how a RoboSharp project or session is organized on top of that**. Workspace must not choose virtual versus physical storage; that belongs in the IO layer.

This overview matches [`AGENTS.md`](../../AGENTS.md). If the repo consolidates IO implementations into a single `RoboSharp.IO` assembly instead of separate `RoboSharp.IO.Physical` / `.InMemory` / `.Overlay` projects, keep the **conceptual** split below.

## Purpose

This material defines two adjacent subsystems:

- `RoboSharp.IO`
- `RoboSharp.Workspaces`

They must stay separate.

The split is:

- **IO** — files, directories, streams, text, existence, enumeration, persistence
- **Workspace** — project/session semantics, source sets, obj/bin layout, open documents, build views, overlays, current configuration

That separation supports Studio and headless tooling with the right dependency direction.

## Design goals

The combined design should:

- support physical and in-memory storage equally well
- support an overlay filesystem for unsaved editor changes
- avoid raw-path string soup in higher layers
- make `.robosharp` projects easy to open and reason about
- allow headless compiler/runtime tools without UI dependencies
- support deterministic builds and clean artifact layout
- keep the Studio document/session model out of the low-level IO layer
- preserve the “real-feeling toolchain” structure with source, obj, bin, and compiled artifacts

## Project layout

Recommended project split:

```text
src/
  RoboSharp.IO/
  RoboSharp.IO.Physical/
  RoboSharp.IO.InMemory/
  RoboSharp.IO.Overlay/
  RoboSharp.Workspaces/
```

### Responsibility split

`RoboSharp.IO`

- contracts
- shared helpers
- normalization utilities
- path/URI policy abstractions

`RoboSharp.IO.Physical`

- `DirectoryInfo` / `FileInfo` backed implementations
- OS-backed persistence

`RoboSharp.IO.InMemory`

- test-friendly ephemeral filesystem
- pure in-memory files/directories

`RoboSharp.IO.Overlay`

- merges base FS + in-memory edits
- ideal for Studio unsaved changes

`RoboSharp.Workspaces`

- project loading
- workspace/session model
- document session model
- artifact layout
- project-relative lookup
- build/configuration views

## Core architectural rule

The host creates a filesystem. The workspace consumes it.

```csharp
IRoboFileSystem fileSystem = new PhysicalRoboFileSystem(...);
// or
IRoboFileSystem fileSystem = new InMemoryRoboFileSystem(...);
// or
IRoboFileSystem fileSystem = new OverlayRoboFileSystem(baseFileSystem, overlayFileSystem);

IRoboWorkspace workspace = new RoboProjectWorkspace(fileSystem, projectFile);
```

## Topic indexes

- **IO layer:** [`docs/io/README.md`](../io/README.md)
- **Workspace layer:** [`docs/workspaces/README.md`](../workspaces/README.md)

The former monolithic page is retired; see [`io-abstractions.md`](io-abstractions.md) for the redirect.
