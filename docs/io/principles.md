# IO design principles

## No UI concerns

The IO layer must know nothing about:

- editor tabs
- dirty markers
- syntax trees
- build diagnostics
- lesson metadata
- runtime/debugger state

## No project semantics

The IO layer must not understand:

- `.robosharp`
- source files
- obj/bin
- build configurations

That belongs in Workspaces.

## Strong object model over naked paths

Prefer typed file/directory handles over path strings flowing everywhere.

## Async by default for content operations

Enumeration can remain sync-friendly, but read/write APIs should be async-first.
