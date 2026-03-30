# Physical implementation

The physical layer should use `DirectoryInfo` and `FileInfo`, not raw string-heavy `System.IO` calls wherever possible.

Recommended main types:

```text
PhysicalRoboFileSystem
PhysicalRoboDirectory
PhysicalRoboFile
```

## Rules

- resolve all nodes beneath a configured root
- normalize directory/file URIs consistently
- do not silently escape the root via `..`
- keep path normalization centralized
- return typed wrappers, not naked paths
