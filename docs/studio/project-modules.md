# Studio project breakdown

Modular Studio projects without fragmentation:

## `RoboSharp.Studio`

Application composition root and startup.

## `RoboSharp.Studio.Shell`

Main window, shell state, docking, command surfaces.

## `RoboSharp.Studio.Documents`

Document session types, editors, viewers, tabs.

## `RoboSharp.Studio.Panels`

Diagnostics, IL, syntax tree, output, call stack, locals, heap, world.

## `RoboSharp.Studio.Commands`

Command definitions and handlers.

## `RoboSharp.Studio.Theming`

Theme services, brushes/resources, editor coloring.

> **Note:** Whether these exist as separate assemblies or folders inside a single host project follows repo conventions in [`AGENTS.md`](../../AGENTS.md).
