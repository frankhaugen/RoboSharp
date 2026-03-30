# World view in Studio

## Rendering model

Studio should render from `RobotWorldSnapshot`, not from live runtime internals. Projection to render tiles should be separate so runtime stays UI-independent.

## Layer model

World visualization must respect:

- `TerrainGrid`
- `ItemGrid`
- `ActorGrid`

See [../world/world-model.md](../world/world-model.md) and [../rendering/render-projection.md](../rendering/render-projection.md).

## Required world views

### Sprite/grid view

Primary learner view.

### ASCII view

First-class, not debug-only. Useful for headless tests and side-by-side comparison.

### Tile inspector

When selecting a tile, show:

- terrain
- item
- actor id/state
- coordinates
- metadata if relevant

## Debug overlays

Useful overlays:

- coordinates
- visited tiles
- route history
- goal tiles
- blocked attempts
- current actor facing
