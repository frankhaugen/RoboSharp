# Runtime hosts

**Hosts** load programs and present them to users: Studio (authoring + debug), Player (artifact-focused run), Web (Blazor Server interactive), or future consoles.

## Shared core

All hosts should sit on the same **application / toolchain / runtime** services:

- Compile or load `RoboExecutable`
- Construct `RobotWorld` and interpreters (or debug sessions)
- Surface stdout, stderr, snapshots, and faults without duplicating IL semantics

## Studio

- Edits sources, runs build pipeline, shows pipeline panes.
- Avalonia-only; see [../studio/technology-stack.md](../studio/technology-stack.md).

## Player

- Loads `.roboexe`, validates, runs or debugs with minimal UI.
- Spec: [../player/README.md](../player/README.md), [../toolchain/v1-toolchain-spec.md](../toolchain/v1-toolchain-spec.md) §2.2.

## Web

- True **Blazor Server** interactive components; no hybrid MVC/Razor Pages patterns ([AGENTS.md](../../AGENTS.md) UI rules).

## Thin host rule

Hosts **orchestrate**; they do not embed lexer/parser logic or world rules. Keep host projects small.

## Related

- [dependency-injection.md](dependency-injection.md)
- [../runtime/v1-runtime-spec.md](../runtime/v1-runtime-spec.md) §16 — `IRuntimeHost`
- [../toolchain/v1-toolchain-spec.md](../toolchain/v1-toolchain-spec.md)
