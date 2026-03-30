# Shard S3 — Language, semantics, compiler

## Ownership (disjoint)

- `docs/language/**`
- `docs/semantics/**`
- `docs/compiler/**`

## Commands (repo root)

```powershell
pwsh -File tools/doc-checklist.ps1 -Command DocMetrics -Shard S3
```

## Src ↔ doc map (Implemented)

| Doc prefix | Primary projects |
| ---------- | ---------------- |
| `language/` | `RoboSharp.Language` |
| `semantics/` | `RoboSharp.Semantics` |
| `compiler/` | `RoboSharp.Language`, `RoboSharp.Semantics`, `RoboSharp.IL`, `RoboSharp.Toolchain` (treat as **No** until multiple layers have code) |

Compiler rows describe a **pipeline stage**, not one folder — parent may set **No** / **Partial** even if one project has files.

## Deliverable

```markdown
### MERGE_ARTIFACT shard=S3

| RelativePath | NonEmptyLines | SuggestedHave | TableHave | ProposedImplemented | Notes |
| ------------ | ------------- | ------------- | --------- | ------------------- | ----- |

<!-- MISMATCHES_ONLY: -->
```

Do **not** edit `documentation-todo.md`.

## Task prompt (copy into subagent)

You are **shard S3** (`language`, `semantics`, `compiler` under `docs/`).

1. `pwsh -File tools/doc-checklist.ps1 -Command DocMetrics -Shard S3` from repo root.
2. Propose **Implemented** using parent **SrcMetrics** for Language, Semantics, IL, Toolchain counts and the map above.
3. Return only `MERGE_ARTIFACT` per this file. Report-only unless user asked for doc edits.
