# Parent orchestration: parallel shards (map → reduce)

Use this when refreshing or auditing [documentation-todo.md](../../documentation-todo.md) at scale. The pattern is **map-reduce for agents**: **parallel map** (independent subagents, disjoint scopes) → **serial reduce** (parent applies one edit to the checklist).

## Non-negotiables

1. **Subagents do not edit `documentation-todo.md`.** They return a **merge artifact** (markdown fragment or JSON). Only the **parent** applies patches to avoid merge conflicts and overlapping edits.
2. **Shards are disjoint.** Each path under `docs/` linked from the checklist tables belongs to exactly one of S1–S5 (see shard files). S6 is a global gap scan, not a table slice.
3. **Parallelism is real:** launch **S1–S5 in the same parent turn** (e.g. multiple `Task` invocations). Optionally add **S6** in parallel if you need orphan-doc discovery.

## Map phase (parallel)

From repository root, each shard subagent runs **only** its commands (cheap, reproducible):

| Shard | DocMetrics command |
| ----- | ------------------- |
| S1 | `pwsh -File tools/doc-checklist.ps1 -Command DocMetrics -Shard S1` |
| S2 | `pwsh -File tools/doc-checklist.ps1 -Command DocMetrics -Shard S2` |
| S3 | `pwsh -File tools/doc-checklist.ps1 -Command DocMetrics -Shard S3` |
| S4 | `pwsh -File tools/doc-checklist.ps1 -Command DocMetrics -Shard S4` |
| S5 | `pwsh -File tools/doc-checklist.ps1 -Command DocMetrics -Shard S5` |
| S6 (optional) | `pwsh -File tools/doc-checklist.ps1 -Command DocGap` |

**Implemented** signals: either the parent runs once `pwsh -File tools/doc-checklist.ps1 -Command SrcMetrics -Json` and pastes the JSON into each subagent prompt, **or** each subagent runs **SrcMetrics** independently (duplicated work, maximum isolation). Prefer **one SrcMetrics** in the parent and attach it to every Task message when the host allows.

Each subagent follows its **shard-*.md** file: scope, rubric, and **deliverable schema**.

## Reduce phase (serial, parent only)

1. Collect merge artifacts from S1–S5 (and S6 notes if used).
2. Reconcile **Have content** cells with script suggestions unless the table intentionally overrides policy (then align [documentation-todo.md](../../documentation-todo.md) legend and `tools/doc-checklist.ps1` thresholds together).
3. Apply **Implemented** using SrcMetrics + shard rubrics; resolve cross-cutting rows (e.g. `compiler/`) by reading multiple projects.
4. Single commit / single edit session on `documentation-todo.md`.

## Failure and partial results

If one shard fails, others remain valid. Parent merges successful shards first, retries failed shard only.

## Cursor rule

See [`.cursor/rules/documentation-checklist.mdc`](../../../.cursor/rules/documentation-checklist.mdc) — bulk checklist work should default to this fan-out pattern, not a single monolithic agent pass.
