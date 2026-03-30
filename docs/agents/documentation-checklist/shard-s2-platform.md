# Shard S2 — Architecture (folder), IO, workspaces

## Ownership (disjoint)

- `docs/architecture/**` (folder only; not root `docs/architecture.md`)
- `docs/io/**`
- `docs/workspaces/**`

## Commands (repo root)

```powershell
pwsh -File tools/doc-checklist.ps1 -Command DocMetrics -Shard S2
```

## Src ↔ doc map (Implemented)

| Doc prefix | Primary project |
| ---------- | ----------------- |
| `io/` | `RoboSharp.IO` |
| `workspaces/` | `RoboSharp.Workspaces` |
| `architecture/` | cross-cutting; often **N/A** or **Partial** (boundary docs) |

Use **SrcMetrics**: high **CsFileCount** on `RoboSharp.IO` supports **Yes** for IO rows; **Workspaces** still **No** if count is 0.

## Deliverable

```markdown
### MERGE_ARTIFACT shard=S2

| RelativePath | NonEmptyLines | SuggestedHave | TableHave | ProposedImplemented | Notes |
| ------------ | ------------- | ------------- | --------- | ------------------- | ----- |

<!-- MISMATCHES_ONLY: -->
```

Do **not** edit `documentation-todo.md`.

## Task prompt (copy into subagent)

You are **shard S2** (platform: `docs/architecture/**`, `docs/io/**`, `docs/workspaces/**`).

1. From repo root: `pwsh -File tools/doc-checklist.ps1 -Command DocMetrics -Shard S2`.
2. Propose **Implemented** for IO and workspaces rows using parent **SrcMetrics** (`RoboSharp.IO`, `RoboSharp.Workspaces` Cs counts) plus the map above.
3. Return only the `MERGE_ARTIFACT` block per `shard-s2-platform.md`. Report-only; do not edit `documentation-todo.md` unless the user ordered fixes.
