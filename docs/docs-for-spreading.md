# Redirect: do not add specification here

New v1 **normative** drafts that lived in this file are split out. Use the targets below; update those pages instead of growing this one.

## v1 normative specs (compiler / runtime / toolchain)

| Topic | Location |
| ----- | -------- |
| Compiler phases, lowering model, artifacts, `IProjectCompiler` shape | [compiler/v1-compiler-spec.md](compiler/v1-compiler-spec.md) |
| Runtime load, stepping, faults, snapshots, host interfaces | [runtime/v1-runtime-spec.md](runtime/v1-runtime-spec.md) |
| Studio vs Player, build lifecycle, CLI, toolchain interfaces | [toolchain/v1-toolchain-spec.md](toolchain/v1-toolchain-spec.md) |

## Lessons and lesson-adjacent

**Start here:** [lessons/README.md](lessons/README.md)

| Topic | Location |
| ----- | -------- |
| Profiles, provider, starter names | [lessons/builtin-profiles.md](lessons/builtin-profiles.md) |
| Goals, evaluation, source-shape goals | [lessons/goals-and-evaluation.md](lessons/goals-and-evaluation.md) |
| Hints, lesson record, UI policy, progression | [lessons/lesson-model.md](lessons/lesson-model.md) |
| Content packs, sessions, metrics | [lessons/content-packs-sessions-and-metrics.md](lessons/content-packs-sessions-and-metrics.md) |
| JSON examples | [lessons/json-formats.md](lessons/json-formats.md) |
| Semantic built-in + profile bridge | [semantics/builtins-and-profiles.md](semantics/builtins-and-profiles.md) |
| Workspace seam | [workspaces/lesson-metadata.md](workspaces/lesson-metadata.md) |
| Studio behavior | [studio/lesson-profiles.md](studio/lesson-profiles.md) |
| Player lesson mode | [player/README.md](player/README.md) |

## Product mission and pipeline doctrine

| Topic | Location |
| ----- | -------- |
| Teaching goals, mission | [governance/mission.md](governance/mission.md) |
| Pipeline integrity, parser neutrality | [governance/design-principles.md](governance/design-principles.md) |
| Layer ownership, `Call` vs `CallBuiltin` | [architecture/pipeline-boundaries.md](architecture/pipeline-boundaries.md) |
| Compiler stages + links | [compiler/compilation-pipeline.md](compiler/compilation-pipeline.md) |
| Syntax-to-IL examples | [compiler/syntax-to-il-lowering.md](compiler/syntax-to-il-lowering.md) |
| IL opcode inventory + implementation pointers | [runtime/il-instruction-set.md](runtime/il-instruction-set.md) |
| Code vs intent | [implementation-gaps.md](implementation-gaps.md) |
| Topics still lacking a full written spec | [missing-specs.md](missing-specs.md) |
