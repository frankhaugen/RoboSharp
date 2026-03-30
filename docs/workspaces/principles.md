# Workspace design principles

A workspace is a project/session abstraction over a filesystem.

It owns:

- which project is open
- which source files belong to it
- where artifacts go
- which configuration is active
- what documents are open/dirty
- how build views are exposed

It does **not** own:

- compiler semantics
- runtime execution
- rendering
- built-in runtime implementations
- raw byte persistence strategy
