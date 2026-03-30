namespace RoboSharp.World;

/// <summary>Authoritative mutable world state. Hosts observe <see cref="RobotWorldSnapshot"/> only.</summary>
public sealed class RobotWorld
{
    public required TerrainGrid Terrain { get; init; }
    public required ItemGrid Items { get; init; }
    public required ActorGrid Actors { get; init; }

    public required Dictionary<int, ActorState> ActorsById { get; init; }

    public required WorldMetadata Metadata { get; init; }

    /// <summary>v1 walkability: terrain, item layer, and actor occupancy (push not implemented).</summary>
    public bool IsWalkable(GridPosition position)
    {
        if (position.X < 0 || position.Y < 0 || position.X >= Terrain.Width || position.Y >= Terrain.Height)
            return false;

        var terrain = Terrain.Get(position);
        var item = Items.Get(position);
        var actor = Actors.Get(position);
        return terrain.IsWalkable && !item.BlocksMovement && !actor.HasActor;
    }

    /// <summary>True if an actor could move onto <paramref name="position"/> ignoring a specific occupant (same tile as mover).</summary>
    public bool IsEnterableForMove(GridPosition position, int movingActorId)
    {
        if (position.X < 0 || position.Y < 0 || position.X >= Terrain.Width || position.Y >= Terrain.Height)
            return false;

        var terrain = Terrain.Get(position);
        var item = Items.Get(position);
        var actor = Actors.Get(position);
        if (!terrain.IsWalkable || item.BlocksMovement)
            return false;

        if (!actor.HasActor || actor.ActorId == movingActorId)
            return true;

        return false;
    }

    public RobotWorldSnapshot CreateSnapshot()
    {
        var tiles = new List<WorldTileSnapshot>(Terrain.Width * Terrain.Height);
        for (var y = 0; y < Terrain.Height; y++)
        {
            for (var x = 0; x < Terrain.Width; x++)
            {
                var p = new GridPosition(x, y);
                var ac = Actors.Get(p);
                tiles.Add(new WorldTileSnapshot(
                    x,
                    y,
                    Terrain.Get(p).Kind,
                    Items.Get(p).Kind,
                    ac.HasActor ? ac.ActorId : null));
            }
        }

        var actors = ActorsById.Values
            .OrderBy(a => a.Id)
            .Select(a => new ActorSnapshot(a.Id, a.Kind, a.Position.X, a.Position.Y, a.Direction, a.InventoryCount))
            .ToList();

        return new RobotWorldSnapshot(Terrain.Width, Terrain.Height, tiles, actors);
    }
}
