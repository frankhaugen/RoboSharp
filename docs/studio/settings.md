# Settings and preferences

## Application settings

- **UI locale (Studio):** **dynamically changeable at runtime** — use **Settings → Language** to switch **English** or **Latin (demo)** without restarting Studio; menus, toolbar, sidebar, inspector panels, and pipeline text refresh immediately (the current build snapshot is re-applied in the new language). The choice is saved under **LocalApplicationData** as `RoboSharp/Studio/user-settings.json` (`localeId`: `en` or `la`). On first launch, if that file has no `localeId`, Studio falls back to the optional environment variable `ROBOSHARP_LOCALE` (`la` / `latin` → Latin pack; otherwise English). Sources: `src/RoboSharp.Locales/English/` and `Latin/`; `TeachingLocaleStringGuard` tests ensure no blank strings ship for either pack.
- theme
- font size
- default layout
- auto-save behavior
- live analysis on/off
- ASCII world preview visible
- debug pause-at-entry
- max snapshots retained
- max steps default

## Project settings

Mostly live in `.robosharp`:

- source files
- output paths
- active builtin profile
- world file
- build flags

## Persistence

User-global settings separate from project settings.
