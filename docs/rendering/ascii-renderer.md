# ASCII renderer

ASCII output is first-class for tests and headless runs.

## Suggested glyph mapping

- **Terrain:** `.` empty, `#` wall, `G` goal  
- **Items:** `*` power-up, `K` key, `B` movable block  
- **Actors:** `^` north, `>` east, `v` south, `<` west  

When layers overlap visually, the **actor glyph wins** (see [render projection](render-projection.md) for priority). Snapshot data still carries full layer information.

## Modes

- **Normal ASCII:** one character per tile.
- **Debug ASCII:** grid layout plus side metadata (optional teaching/IDE feature).

Implementations live in host-specific adapter projects; the world layer only supplies snapshots and contracts.
