# Documentation authoring order

Many paths under `docs/` are described in [README.md](README.md). Use this list as a working checklist: top to bottom follows **teaching flow** (why → shape of the repo → language → compile → run → world → presentation → tooling). Language and semantics specs are split under `docs/language/` and `docs/semantics/` (former `language/README.md` monolith). Some Studio pages came from `general-specs.md`; others remain stubs.

## Governance

1. [governance/mission.md](governance/mission.md)
2. [governance/design-principles.md](governance/design-principles.md)
3. [governance/dependency-policy.md](governance/dependency-policy.md)
4. [governance/implementation-order.md](governance/implementation-order.md)

## Architecture

5. [architecture/solution-structure.md](architecture/solution-structure.md)
6. [architecture/dependency-rules.md](architecture/dependency-rules.md)
7. [architecture/dependency-injection.md](architecture/dependency-injection.md)
8. [architecture/io-workspace-overview.md](architecture/io-workspace-overview.md) — boundary, goals, project split, core rule
9. [architecture/workspace-model.md](architecture/workspace-model.md) — points at [workspaces/README.md](workspaces/README.md)
10. [architecture/io-abstractions.md](architecture/io-abstractions.md) — redirect to overview + `io/` + `workspaces/`
11. [architecture/runtime-hosts.md](architecture/runtime-hosts.md)

### IO layer (`docs/io/`)

Split from the former `architecture/io-abstractions.md` monolith. Index: [io/README.md](io/README.md).

- [io/principles.md](io/principles.md)
- [io/canonical-abstractions.md](io/canonical-abstractions.md)
- [io/physical-storage.md](io/physical-storage.md)
- [io/in-memory-storage.md](io/in-memory-storage.md)
- [io/overlay-storage.md](io/overlay-storage.md)
- [io/optional-storage-seam.md](io/optional-storage-seam.md)
- [io/helpers-and-errors.md](io/helpers-and-errors.md)

### Workspaces layer (`docs/workspaces/`)

Index: [workspaces/README.md](workspaces/README.md).

- [workspaces/principles.md](workspaces/principles.md)
- [workspaces/kinds.md](workspaces/kinds.md)
- [workspaces/contracts.md](workspaces/contracts.md)
- [workspaces/project-loading.md](workspaces/project-loading.md)
- [workspaces/artifact-layout.md](workspaces/artifact-layout.md)
- [workspaces/sessions-and-documents.md](workspaces/sessions-and-documents.md)
- [workspaces/configuration.md](workspaces/configuration.md)
- [workspaces/lesson-metadata.md](workspaces/lesson-metadata.md)
- [workspaces/build-pipeline-integration.md](workspaces/build-pipeline-integration.md)
- [workspaces/temporary-workspace.md](workspaces/temporary-workspace.md)
- [workspaces/studio-overlay-and-save.md](workspaces/studio-overlay-and-save.md)
- [workspaces/concrete-types-and-di.md](workspaces/concrete-types-and-di.md)
- [workspaces/non-goals-and-summary.md](workspaces/non-goals-and-summary.md)

## Language (`RoboSharp.Language`)

Split from the former `language/README.md` monolith. Index: [language/README.md](language/README.md).

12. [language/README.md](language/README.md)
13. [language/language-overview.md](language/language-overview.md)
14. [language/syntax.md](language/syntax.md)
15. [language/functions.md](language/functions.md)
16. [language/statements.md](language/statements.md)
17. [language/expressions.md](language/expressions.md)
18. [language/types.md](language/types.md)
19. [language/arrays.md](language/arrays.md)
20. [language/built-in-functions.md](language/built-in-functions.md)
21. [language/source-model.md](language/source-model.md)
22. [language/syntax-kinds-and-facts.md](language/syntax-kinds-and-facts.md)
23. [language/tokens.md](language/tokens.md)
24. [language/lexer.md](language/lexer.md)
25. [language/parser.md](language/parser.md)
26. [language/syntax-tree.md](language/syntax-tree.md)
27. [language/public-api.md](language/public-api.md)
28. [language/project-layout.md](language/project-layout.md)

## Semantics (`RoboSharp.Semantics`)

Index: [semantics/README.md](semantics/README.md).

29. [semantics/README.md](semantics/README.md)
30. [semantics/overview.md](semantics/overview.md)
31. [semantics/type-system.md](semantics/type-system.md)
32. [semantics/symbols-and-scopes.md](semantics/symbols-and-scopes.md)
33. [semantics/builtins-and-profiles.md](semantics/builtins-and-profiles.md)
34. [semantics/binding-and-bound-tree.md](semantics/binding-and-bound-tree.md)
35. [semantics/conversions.md](semantics/conversions.md)
36. [semantics/control-flow-and-conditions.md](semantics/control-flow-and-conditions.md)
37. [semantics/arrays.md](semantics/arrays.md)
38. [semantics/operators.md](semantics/operators.md)
39. [semantics/diagnostics.md](semantics/diagnostics.md)
40. [semantics/semantic-model-output.md](semantics/semantic-model-output.md)
41. [semantics/public-api.md](semantics/public-api.md)
42. [semantics/project-layout.md](semantics/project-layout.md)
43. [semantics/summary.md](semantics/summary.md)

## Compiler

44. [compiler/compilation-pipeline.md](compiler/compilation-pipeline.md)
45. [compiler/lexical-analysis.md](compiler/lexical-analysis.md)
46. [compiler/parsing.md](compiler/parsing.md)
47. [compiler/syntax-tree.md](compiler/syntax-tree.md)
48. [compiler/semantic-analysis.md](compiler/semantic-analysis.md)
49. [compiler/diagnostics.md](compiler/diagnostics.md)
50. [compiler/il-generation.md](compiler/il-generation.md)

## Runtime

51. [runtime/il-instruction-set.md](runtime/il-instruction-set.md)
52. [runtime/interpreter.md](runtime/interpreter.md)
53. [runtime/execution-model.md](runtime/execution-model.md)
54. [runtime/runtime-state.md](runtime/runtime-state.md)
55. [runtime/error-handling.md](runtime/error-handling.md)
56. [runtime/standard-output.md](runtime/standard-output.md)

## World

57. [world/world-model.md](world/world-model.md)
58. [world/terrain-grid.md](world/terrain-grid.md)
59. [world/item-grid.md](world/item-grid.md)
60. [world/actor-grid.md](world/actor-grid.md)
61. [world/world-actions.md](world/world-actions.md)
62. [world/movement-rules.md](world/movement-rules.md)
63. [world/metrics-and-analysis.md](world/metrics-and-analysis.md)

## Rendering

64. [rendering/render-projection.md](rendering/render-projection.md)
65. [rendering/ascii-renderer.md](rendering/ascii-renderer.md)
66. [rendering/sprite-renderer.md](rendering/sprite-renderer.md)

## Toolchain

67. [toolchain/project-format.md](toolchain/project-format.md)
68. [toolchain/roboexe-format.md](toolchain/roboexe-format.md)
69. [toolchain/build-process.md](toolchain/build-process.md)
70. [toolchain/artifact-layout.md](toolchain/artifact-layout.md)

## Debugger

71. [debugger/debugger-architecture.md](debugger/debugger-architecture.md)
72. [debugger/breakpoints.md](debugger/breakpoints.md)
73. [debugger/stepping.md](debugger/stepping.md)
74. [debugger/state-inspection.md](debugger/state-inspection.md)
75. [debugger/metrics-view.md](debugger/metrics-view.md)

## Studio

Suggested order for Studio specs (split from the former `general-specs.md`):

76. [studio/README.md](studio/README.md)
77. [studio/overview.md](studio/overview.md)
78. [studio/technology-stack.md](studio/technology-stack.md)
79. [studio/referenced-solution-shape.md](studio/referenced-solution-shape.md)
80. [studio/composition-and-domain.md](studio/composition-and-domain.md)
81. [studio/ide-layout.md](studio/ide-layout.md)
82. [studio/workspace-integration.md](studio/workspace-integration.md)
83. [studio/editor-behavior.md](studio/editor-behavior.md)
84. [studio/build-and-analysis.md](studio/build-and-analysis.md)
85. [studio/inspection-panels.md](studio/inspection-panels.md)
86. [studio/visualization.md](studio/visualization.md)
87. [studio/output-and-state-panels.md](studio/output-and-state-panels.md)
88. [studio/lesson-profiles.md](studio/lesson-profiles.md)
89. [studio/menus-and-commands.md](studio/menus-and-commands.md)
90. [studio/settings.md](studio/settings.md)
91. [studio/theming.md](studio/theming.md)
92. [studio/syntax-highlighting.md](studio/syntax-highlighting.md)
93. [studio/extensibility.md](studio/extensibility.md)
94. [studio/project-modules.md](studio/project-modules.md)
95. [studio/testing-strategy.md](studio/testing-strategy.md)
96. [studio/performance.md](studio/performance.md)
97. [studio/scope-mvp-and-non-goals.md](studio/scope-mvp-and-non-goals.md)
98. [studio/general-specs.md](studio/general-specs.md) (redirect to index)
