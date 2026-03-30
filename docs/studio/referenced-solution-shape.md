# Referenced solution shape (illustrative)

> **Authoritative layout:** Project names and layering in this repository are defined in [`AGENTS.md`](../../AGENTS.md). The tree below is an **illustrative** breakdown from an earlier Studio spec. Prefer names like `RoboSharp.Language`, `RoboSharp.Semantics`, `RoboSharp.Runtime`, … over hypothetical aggregates such as `RoboSharp.Core` unless the codebase explicitly introduces them.

Recommended solution shape (conceptual):

```text
RoboSharp.slnx

src/
  RoboSharp.Core/
  RoboSharp.Language/
  RoboSharp.Semantics/
  RoboSharp.IL/
  RoboSharp.Runtime/
  RoboSharp.Debugging/
  RoboSharp.World/
  RoboSharp.Rendering/
  RoboSharp.IO/
  RoboSharp.Workspaces/
  RoboSharp.Lessons/
  RoboSharp.ProjectSystem/
  RoboSharp.Studio/
  RoboSharp.Studio.Shell/
  RoboSharp.Studio.Documents/
  RoboSharp.Studio.Panels/
  RoboSharp.Studio.Theming/
  RoboSharp.Studio.Commands/

tests/
  RoboSharp.Core.Tests/
  RoboSharp.Language.Tests/
  ...
```

## Intent

`RoboSharp.Studio` should not own compiler/runtime logic. It should mostly own:

- composition
- screen/view model orchestration
- IDE commands
- IDE state persistence
- panel composition
- document lifecycle
- application shell behavior

That keeps the Studio replaceable.
