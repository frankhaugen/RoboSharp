# RoboSharp.Workspaces documentation

Project and session semantics on top of [`RoboSharp.IO`](../io/README.md). **IO** chooses where bytes live; **workspace** does not.

**Start with the boundary:** [IO and workspaces overview](../architecture/io-workspace-overview.md).

| Topic | Document |
| ----- | -------- |
| Design principles | [principles.md](principles.md) |
| `RoboProjectWorkspace`, `RoboTemporaryWorkspace` | [kinds.md](kinds.md) |
| `IRoboWorkspace` and extensions | [contracts.md](contracts.md) |
| Loading `.robosharp` | [project-loading.md](project-loading.md) |
| `IBuildArtifactLayout`, artifact kinds | [artifact-layout.md](artifact-layout.md) |
| `IWorkspaceSession`, document sessions | [sessions-and-documents.md](sessions-and-documents.md) |
| Debug/Release configuration | [configuration.md](configuration.md) |
| Lesson/profile metadata on the project | [lesson-metadata.md](lesson-metadata.md) |
| `ICompilerPipeline` and workspace | [build-pipeline-integration.md](build-pipeline-integration.md) |
| Temporary / scratch workspace | [temporary-workspace.md](temporary-workspace.md) |
| Studio overlay stack, save, validation | [studio-overlay-and-save.md](studio-overlay-and-save.md) |
| Concrete types and DI | [concrete-types-and-di.md](concrete-types-and-di.md) |
| Non-goals and summary | [non-goals-and-summary.md](non-goals-and-summary.md) |
