# Lesson and profile awareness in workspace

The workspace should expose project/runtime metadata, but not implement lesson semantics.

For example:

- active builtin profile name
- active world file path
- max steps
- studio preferences from `.robosharp`

Those belong in project data. Actual lesson/profile behavior belongs elsewhere, consistent with the lesson/profile split in [`AGENTS.md`](../../AGENTS.md).

So:

- workspace loads project metadata
- lesson/profile services interpret it
