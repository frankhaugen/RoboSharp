# In-memory implementation

Main purpose:

- tests
- temporary workspaces
- examples
- unsaved scratch environments
- future browser/server-side scenarios

Recommended types:

```text
InMemoryRoboFileSystem
InMemoryRoboDirectory
InMemoryRoboFile
```

## Rules

- preserve directory/file hierarchy
- preserve timestamps if useful
- content stored as UTF-8 text or bytes
- fully deterministic
- zero OS assumptions
