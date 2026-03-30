# Shard S5 — Debugger, Studio

## Ownership (disjoint)

- `docs/debugger/**`
- `docs/studio/**`

## Commands (repo root)

```powershell
pwsh -File tools/doc-checklist.ps1 -Command DocMetrics -Shard S5
```

## Src ↔ doc map (Implemented)

| Doc prefix | Primary project / note |
| ---------- | ---------------------- |
| `studio/` | `RoboSharp.Studio` — **Partial** if only host shell (e.g. single `Program.cs`) |
| `debugger/` | no dedicated project today; **No** until debugger services exist in Application/Studio |

Also consider `RoboSharp.Web`, `RoboSharp.Player` from SrcMetrics when reasoning about “hosts” for parent’s “Missing doc trees” section (this shard may **note** gaps; parent edits that section).

## Deliverable

```markdown
### MERGE_ARTIFACT shard=S5

| RelativePath | NonEmptyLines | SuggestedHave | TableHave | ProposedImplemented | Notes |
| ------------ | ------------- | ------------- | --------- | ------------------- | ----- |

<!-- MISMATCHES_ONLY: -->

<!-- HOST_NOTES: optional bullets for parent about Web/Player/Studio vs specs -->
```

Do **not** edit `documentation-todo.md`.

## Task prompt (copy into subagent)

You are **shard S5** (`docs/debugger/**`, `docs/studio/**`).

1. `pwsh -File tools/doc-checklist.ps1 -Command DocMetrics -Shard S5` from repo root.
2. Use parent **SrcMetrics** for Studio/Web/Player counts; propose **Implemented** per rubric above.
3. Return `MERGE_ARTIFACT` and optional `HOST_NOTES`. Report-only unless user asked for doc edits.
