# Shard S1 — Meta, diagrams, governance

## Ownership (disjoint)

- `docs/README.md`, `docs/build.md`, `docs/repository-layout.md`, `docs/nuget.md`, `docs/architecture.md`
- `docs/diagrams/**`
- `docs/governance/**`

Do **not** read or report on `docs/architecture/**` (that is **S2**). Root `architecture.md` is S1; folder `architecture/` is S2.

## Commands (repo root)

```powershell
pwsh -File tools/doc-checklist.ps1 -Command DocMetrics -Shard S1
```

For **Implemented**: these rows are almost always **N/A** (meta) or **Partial** (repo layout). Use parent-supplied **SrcMetrics** JSON if available; do not guess Cs counts by hand.

## Rubric hints

| Row kind | Implemented |
| -------- | ----------- |
| build, nuget, diagrams | N/A |
| repository-layout, architecture (root) | Partial (solution exists; product code varies) |

## Deliverable (paste to parent)

Use this header so the parent can grep-merge:

```markdown
### MERGE_ARTIFACT shard=S1

| RelativePath | NonEmptyLines | SuggestedHave | TableHave | ProposedImplemented | Notes |
| ------------ | ------------- | ------------- | --------- | ------------------- | ----- |
| ... | ... | ... | ... | ... | ... |

<!-- MISMATCHES_ONLY: list paths where SuggestedHave != TableHave -->
```

Rules:

- One table row per **linked** doc in this shard (mirror script output).
- **ProposedImplemented** only when you are changing the column; else leave empty or repeat current.
- **Do not** edit `documentation-todo.md`.

## Task prompt (copy into subagent)

You are **shard S1** for the RoboSharp documentation checklist. Repository root is given in the user message.

1. Run: `pwsh -File tools/doc-checklist.ps1 -Command DocMetrics -Shard S1` from the repo root.
2. If the parent attached **SrcMetrics** JSON, use it for **Implemented** proposals on meta rows; otherwise output **Have content** analysis only and mark Implemented as “needs parent SrcMetrics”.
3. Return **only** the `MERGE_ARTIFACT` markdown block defined in `docs/agents/documentation-checklist/shard-s1-meta.md`.
4. Do not modify any files unless the user explicitly asked you to fix doc bodies; default is **report-only**.
