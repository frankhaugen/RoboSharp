# Shard S4 — Runtime, world, rendering, toolchain

## Ownership (disjoint)

- `docs/runtime/**`
- `docs/world/**`
- `docs/rendering/**`
- `docs/toolchain/**`

## Commands (repo root)

```powershell
pwsh -File tools/doc-checklist.ps1 -Command DocMetrics -Shard S4
```

## Src ↔ doc map (Implemented)

| Doc prefix | Primary project |
| ---------- | ----------------- |
| `runtime/` | `RoboSharp.Runtime` |
| `world/` | `RoboSharp.World` |
| `toolchain/` | `RoboSharp.Toolchain` |
| `rendering/` | no dedicated project (adapters); **No** / **Partial** until host or world projection code exists |

## Deliverable

```markdown
### MERGE_ARTIFACT shard=S4

| RelativePath | NonEmptyLines | SuggestedHave | TableHave | ProposedImplemented | Notes |
| ------------ | ------------- | ------------- | --------- | ------------------- | ----- |

<!-- MISMATCHES_ONLY: -->
```

Do **not** edit `documentation-todo.md`.

## Task prompt (copy into subagent)

You are **shard S4** (runtime, world, rendering, toolchain docs).

1. `pwsh -File tools/doc-checklist.ps1 -Command DocMetrics -Shard S4` from repo root.
2. Propose **Implemented** from parent **SrcMetrics** and the map above; rendering is usually **No** until implementation exists.
3. Return only `MERGE_ARTIFACT`. Report-only unless user asked for doc edits.
