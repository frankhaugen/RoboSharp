# Workspace model in Studio

The Studio should consume the layering described in [`AGENTS.md`](../../AGENTS.md):

- storage/file system below
- workspace above storage

The Studio opens a workspace over an `IRoboFileSystem`, not over raw paths. This keeps unsaved/overlay scenarios viable.

See also: [IO and workspaces overview](../architecture/io-workspace-overview.md), [docs/io/](../io/README.md), [docs/workspaces/](../workspaces/README.md), and [architecture/workspace-model.md](../architecture/workspace-model.md).

## File system strategy

The Studio should support:

- physical file system
- in-memory file system
- overlay file system

Practical design:

```text
Physical file system
       +
In-memory modified documents
       ↓
Overlay file system
       ↓
Workspace session
```

This makes “dirty but not saved” documents natural.

## Workspace session responsibilities

`WorkspaceSession` should expose:

- current project
- source documents
- artifact directories
- active file
- build configuration
- current build results
- current lesson/profile/world settings

It should not contain rendering or compiler logic itself.
