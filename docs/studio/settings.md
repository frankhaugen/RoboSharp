# Settings and preferences

## Application settings

- **UI locale (Studio):** optional environment variable `ROBOSHARP_LOCALE`. Set to `la` or `latin` to load the playful **Latin** demo pack (sources under `src/RoboSharp.Locales/Latin/`). Omit or use any other value for **English** (`src/RoboSharp.Locales/English/`). `TeachingLocaleStringGuard` tests ensure no blank strings ship for either pack. Example (PowerShell): `$env:ROBOSHARP_LOCALE='la'; dotnet run --project src/RoboSharp.Studio/RoboSharp.Studio.csproj`.
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
