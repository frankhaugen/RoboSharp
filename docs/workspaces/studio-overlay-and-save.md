# Studio overlay flow, save behavior, validation

## Studio overlay flow

Canonical Studio behavior:

```text
PhysicalRoboFileSystem
        +
InMemoryRoboFileSystem (open edits)
        ↓
OverlayRoboFileSystem
        ↓
RoboProjectWorkspace
        ↓
WorkspaceSession
        ↓
Compiler / Studio panels / debugger startup
```

That gives you:

- unsaved edits included in analysis/build/debug
- physical files untouched until save
- no special hacks in compiler/runtime code
- clean document reversion

This is a major reason to keep IO and Workspaces separate.

See [`docs/io/overlay-storage.md`](../io/overlay-storage.md).

## Save behavior

### Source document save

- writes current overlay content to physical file
- clears dirty state
- overlay node may remain but becomes identical to base or be compacted away

### Save all

- flushes all dirty editable docs
- optionally project file too

### Revert

- discard overlay content for that document
- restore base file view

### Build without save

- build should use overlay-backed workspace state
- this is critical for a real IDE feel

## Workspace validation

A workspace load/refresh should validate:

- project file format/version
- root/project path consistency
- startup file exists
- source files resolve and are unique
- output/intermediate paths are legal
- source files do not point into `obj/` or `bin/` by mistake
- project does not reference generated artifacts as source

This should produce workspace diagnostics, not language diagnostics.
