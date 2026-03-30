# Overlay implementation

This is likely the most important one for Studio.

Recommended shape:

```text
OverlayRoboFileSystem
OverlayRoboDirectory
OverlayRoboFile
```

It combines:

- base filesystem
- mutable overlay filesystem

## Resolution rule

Read:

1. if overlay has node, use overlay
2. else use base

Write:

- always goes to overlay

Delete:

- represented as tombstone in overlay

## Why this matters

This gives you the clean Studio shape:

```text
Physical file system
        +
In-memory open-document overlay
        ↓
Composite/overlay file system
        ↓
Workspace
```

That is the best answer to unsaved editor state.

See also [../workspaces/studio-overlay-and-save.md](../workspaces/studio-overlay-and-save.md).
