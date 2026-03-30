# Performance rules (Studio)

The Studio is not a game engine, but responsiveness matters.

## Rules

- debounce live compilation
- avoid reparsing every panel separately
- cache compilation products per document revision
- never let panel rendering trigger recompilation
- snapshots immutable and cheap to diff
- large artifact viewers virtualized if needed

## Build cancellation

Typing should cancel outdated background analysis.
