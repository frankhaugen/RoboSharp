# Testing strategy (Studio)

The Studio should be testable without booting real windows for most logic.

## Unit-test heavily

- command handlers
- layout state reducers
- document session logic
- workspace session logic
- debug orchestration
- panel view models

## UI tests

Only a thinner layer:

- window opens
- basic document switching
- breakpoint interactions
- debug stepping visible
- panes update correctly

## Determinism

Debugger integration tests should use known `.roboexe` fixtures and snapshot assertions.

Tests use **TUnit** per [`AGENTS.md`](../../AGENTS.md).
