# IDE shell layout and navigation

## High-level Studio architecture

```text
RoboSharpStudio
 ├─ Host / Composition Root
 ├─ Shell
 │   ├─ Main Window
 │   ├─ Dock/Panel Layout
 │   ├─ Menus / Toolbars / Status
 │   └─ Dialog orchestration
 ├─ Workspace Layer
 │   ├─ Project open/save
 │   ├─ Source document lifecycle
 │   ├─ Build artifact layout
 │   └─ File system integration
 ├─ Editing Layer
 │   ├─ Source editor
 │   ├─ Diagnostics view
 │   ├─ Symbol navigation
 │   └─ Outline
 ├─ Inspection Layer
 │   ├─ Tokens
 │   ├─ Syntax tree
 │   ├─ Bound tree
 │   ├─ IL
 │   └─ Build outputs
 ├─ Debug Layer
 │   ├─ Run
 │   ├─ Step debugger
 │   ├─ Breakpoints
 │   ├─ Call stack
 │   ├─ Locals
 │   ├─ Heap/arrays
 │   ├─ stdout/stderr
 │   └─ Runtime fault display
 ├─ World Layer
 │   ├─ World view
 │   ├─ ASCII preview
 │   ├─ Sprite view later
 │   └─ Route / metrics panel
 └─ Lesson Layer
     ├─ Active builtin profile
     ├─ Goal/lesson metadata
     └─ Lesson-aware help
```

## Main window — top-level layout

```text
+----------------------------------------------------------------------------------+
| Menu Bar                                                                         |
+----------------------------------------------------------------------------------+
| Toolbar / Command Bar                                                            |
+----------------------+--------------------------------------+--------------------+
| Solution/Workspace   | Document Tabs                        | Inspector Switcher |
| Explorer             |                                      |                    |
|                      | Source / IL / AST / World / etc.     |                    |
|                      |                                      |                    |
+----------------------+----------------------+---------------+--------------------+
| Bottom Panel: Diagnostics / stdout / stderr / Build / Search / Debug Output     |
+----------------------+----------------------+------------------------------------+
| Left Bottom Optional | Right Bottom Optional| Status Bar                         |
| Call Stack / Outline | Locals / Heap        |                                    |
+----------------------------------------------------------------------------------+
```

## Minimum panes

The Studio should ship with these panes:

- Workspace Explorer
- Editor/Document Tabs
- Diagnostics
- Tokens
- Syntax Tree
- Semantic/Bound Tree
- IL View
- World View
- Debug Controls
- Call Stack
- Locals
- Arrays/Heap
- stdout
- stderr
- Route/Metrics
- Status Bar

Not all need to be visible by default.

## Default layout

Default learner-friendly layout:

- center: source editor
- right: world view + IL view tabs
- bottom: stdout/stderr + diagnostics
- left: workspace explorer
- hidden but dockable: tokens, syntax tree, bound tree, heap, route metrics

Default advanced layout can expose more.

## Shell navigation model

Use a document/panel hybrid model.

### Document tabs

For editable or focused artifacts:

- `.robo`
- `.robosharp`
- `.world.json`
- `.roboil.json`
- `.roboast.json`
- `.robobind.json`

### Tool windows

For contextual views:

- diagnostics
- call stack
- locals
- heap
- output
- runtime metrics
- lesson help

This maps well to IDE expectations.

See [menus-and-commands.md](menus-and-commands.md) for menus and toggles.
