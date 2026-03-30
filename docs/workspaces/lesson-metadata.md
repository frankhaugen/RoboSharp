# Lesson and profile awareness in workspace

The workspace should expose project/runtime metadata, but not implement lesson semantics.

For example:

- active builtin profile name
- active world file path
- max steps
- studio preferences from `.robosharp`

Those belong in project data. Actual lesson/profile behavior belongs elsewhere, consistent with the lesson/profile split in [`AGENTS.md`](../../AGENTS.md).

So:

- workspace loads project metadata
- lesson/profile services interpret it

## How this relates to a `LessonDefinition`

The cross-layer lesson record (see [../lessons/lesson-model.md](../lessons/lesson-model.md)) carries fields such as profile name, world file path, goals, hints, help, and UI policy. The workspace is responsible for **paths, discovery, and loading bytes**; interpretation of that data (goal evaluation, which panels to show) lives in application and host layers.

Keep a clear seam:

| Concern | Typical owner |
| ------- | ------------- |
| `.robosharp` / document paths / artifacts | `RoboSharp.Workspaces` |
| Lesson JSON, pack layout on disk | IO + workspace or content loader |
| Builtin profile for bind, world for runtime | orchestration using lesson definition |
| Pass/fail and Studio chrome | goals + Studio |

Full educational spec: [../lessons/README.md](../lessons/README.md).
