# Extensibility model

Do not over-engineer plugins in v1. Leave clean seams.

## Likely extension seams

- world renderer
- lesson source/provider
- file system backend
- command registration
- panel registration
- artifact viewers

## Avoid

Do not build a general MEF-style extension system yet.
