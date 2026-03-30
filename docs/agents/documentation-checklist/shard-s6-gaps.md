# Shard S6 — Gap scan (unlinked docs)

## Ownership

Global **readonly** scan: any `docs/**/*.md` not appearing as a linked path in `documentation-todo.md` checklist tables.

**Does not overlap** S1–S5 table rows; runs in parallel with them.

## Commands (repo root)

```powershell
pwsh -File tools/doc-checklist.ps1 -Command DocGap
# JSON:
pwsh -File tools/doc-checklist.ps1 -Command DocGap -Json
```

## Deliverable

```markdown
### MERGE_ARTIFACT shard=S6

Unlinked markdown (candidates to add to checklist or exclude intentionally):

- `agents/...`
- ...

<!-- RECOMMENDATION: add-to-checklist | index-only | ignore -->
```

Do **not** edit `documentation-todo.md` unless the user explicitly asked to add rows.

## Task prompt (copy into subagent)

You are **shard S6** (gap scan only).

1. From repo root run `pwsh -File tools/doc-checklist.ps1 -Command DocGap` (add `-Json` if the parent wants machine output).
2. Classify: should each path be linked from [documentation-todo.md](../../documentation-todo.md), indexed from [README.md](../../README.md) only, or intentionally omitted?
3. Return `MERGE_ARTIFACT shard=S6` only. Do not edit files unless the user requested checklist expansion.
