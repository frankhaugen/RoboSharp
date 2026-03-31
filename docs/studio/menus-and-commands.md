# Menus and commands

## Top-level menus

Recommended:

- File
- Edit
- View
- Build
- Run
- Debug
- World
- Lesson
- Tools
- Help

## Must-have commands

### File

- New Project
- Open Project
- Save
- Save All
- Close Project
- Recent Projects

### Build

- Analyze
- Build Debug
- Build Release
- Rebuild
- Open Output Folder

### Run/Debug

- Run
- Debug
- Pause
- Stop
- Reset
- Step Into
- Step Over
- Step Out

### View

- Toggle Tokens
- Toggle Syntax Tree
- Toggle Semantic Tree
- Toggle IL
- Toggle Diagnostics
- Toggle World
- Toggle stdout/stderr
- Toggle Call Stack
- Toggle Locals
- Toggle Heap
- Toggle Metrics

## RoboSharp Studio (current host)

The Avalonia shell implements a minimal menu bar today:

- **File** — New, Open, Save, Save As, Exit (see `MainWindow.BuildMenu()`).
- **Settings → Language** — **English (en)** or **Latin — demo (la)**; **runtime switch** (no restart); persists to `user-settings.json` (see [settings.md](settings.md)).
- **Help** — About.
