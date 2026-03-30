# Documentation checklist — parallel shards

This folder defines **disjoint shards** so a **parent agent** can fan out **five (or six) parallel subagent tasks** (map phase), then **merge** results into [documentation-todo.md](../../documentation-todo.md) in one place (reduce phase).

| File | Role |
| ---- | ---- |
| [parent-orchestration.md](parent-orchestration.md) | Fan-out / gather-merge contract; when to run which shard |
| [shard-s1-meta.md](shard-s1-meta.md) | S1 — entry, diagrams, governance |
| [shard-s2-platform.md](shard-s2-platform.md) | S2 — architecture, IO, workspaces |
| [shard-s3-language-pipeline.md](shard-s3-language-pipeline.md) | S3 — language, semantics, compiler |
| [shard-s4-runtime-tooling.md](shard-s4-runtime-tooling.md) | S4 — runtime, world, rendering, toolchain |
| [shard-s5-hosts-debugger.md](shard-s5-hosts-debugger.md) | S5 — debugger, Studio |
| [shard-s6-gaps.md](shard-s6-gaps.md) | S6 — optional; unlinked `docs/**/*.md` (readonly gap scan) |

Tooling: [tools/doc-checklist.ps1](../../../tools/doc-checklist.ps1) supports `-Shard S1` … `S5` for scoped **DocMetrics**, plus `-Command DocGap` for S6.

Legacy pointer: [documentation-checklist-subagents.md](../documentation-checklist-subagents.md) redirects here.
