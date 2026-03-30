namespace RoboSharp.World;

/// <summary>Deterministic world mutations used by runtime built-in handlers.</summary>
public static class RobotWorldCommands
{
    public static bool TryGetPrimaryActor(RobotWorld world, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ActorState? actor)
    {
        actor = null;
        var id = world.Metadata.PrimaryActorId;
        if (id is null)
            return false;

        return world.ActorsById.TryGetValue(id.Value, out actor);
    }

    public static bool TryMoveForward(RobotWorld world, ActorState actor)
    {
        var next = actor.Position with
        {
            X = actor.Position.X + DirectionHelpers.GetForwardOffset(actor.Direction).X,
            Y = actor.Position.Y + DirectionHelpers.GetForwardOffset(actor.Direction).Y,
        };

        if (!world.IsEnterableForMove(next, actor.Id))
            return false;

        world.Actors.Set(actor.Position, ActorCell.Empty);
        actor.Position = next;
        world.Actors.Set(next, new ActorCell(actor.Id));
        return true;
    }

    public static void TurnLeft(RobotWorld world, ActorState actor) =>
        actor.Direction = DirectionHelpers.TurnLeft(actor.Direction);

    public static void TurnRight(RobotWorld world, ActorState actor) =>
        actor.Direction = DirectionHelpers.TurnRight(actor.Direction);

    public static bool IsClearInDirection(RobotWorld world, ActorState actor, Direction direction)
    {
        var next = actor.Position with
        {
            X = actor.Position.X + DirectionHelpers.GetForwardOffset(direction).X,
            Y = actor.Position.Y + DirectionHelpers.GetForwardOffset(direction).Y,
        };

        return world.IsEnterableForMove(next, actor.Id);
    }

    public static bool FrontIsClear(RobotWorld world, ActorState actor) =>
        IsClearInDirection(world, actor, actor.Direction);

    public static bool LeftIsClear(RobotWorld world, ActorState actor) =>
        IsClearInDirection(world, actor, DirectionHelpers.TurnLeft(actor.Direction));

    public static bool RightIsClear(RobotWorld world, ActorState actor) =>
        IsClearInDirection(world, actor, DirectionHelpers.TurnRight(actor.Direction));

    public static bool TryPick(RobotWorld world, ActorState actor)
    {
        var cell = world.Items.Get(actor.Position);
        if (!cell.IsPickup)
            return false;

        actor.InventoryCount++;
        world.Items.Set(actor.Position, new ItemCell(ItemCellKind.None));
        return true;
    }

    public static bool TryDrop(RobotWorld world, ActorState actor)
    {
        if (actor.InventoryCount <= 0)
            return false;

        var here = world.Items.Get(actor.Position);
        if (here.HasItem)
            return false;

        actor.InventoryCount--;
        world.Items.Set(actor.Position, new ItemCell(ItemCellKind.Key));
        return true;
    }
}
