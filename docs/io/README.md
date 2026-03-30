# RoboSharp.IO documentation

Typed filesystem abstractions: where bytes live. Project and session semantics live in [`docs/workspaces/`](../workspaces/README.md).

**Start with the boundary:** [IO and workspaces overview](../architecture/io-workspace-overview.md).

| Topic | Document |
| ----- | -------- |
| Design principles | [principles.md](principles.md) |
| `IRoboFileSystem` and related contracts; `Uri` identity | [canonical-abstractions.md](canonical-abstractions.md) |
| Physical (`DirectoryInfo` / `FileInfo`) | [physical-storage.md](physical-storage.md) |
| In-memory | [in-memory-storage.md](in-memory-storage.md) |
| Overlay (base + mutable layer) | [overlay-storage.md](overlay-storage.md) |
| Optional `IRoboStorage` seam | [optional-storage-seam.md](optional-storage-seam.md) |
| Path helpers, encoding, error policy | [helpers-and-errors.md](helpers-and-errors.md) |
