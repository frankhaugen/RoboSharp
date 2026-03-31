using RoboSharp.World;

namespace RoboSharp.World.Tests;

public class WorldMovementTests
{
    [Test]
    public async Task TryMoveForward_Moves_Primary_Robot_East_When_Facing_East()
    {
        var world = RobotWorldFactory.CreateBorderedEmpty(6, 6);
        var actor = world.ActorsById[1];
        await Assert.That(actor.Position.X).IsEqualTo(1);
        await Assert.That(actor.Direction).IsEqualTo(Direction.East);

        var moved = RobotWorldCommands.TryMoveForward(world, actor);
        await Assert.That(moved).IsTrue();
        await Assert.That(actor.Position.X).IsEqualTo(2);
        await Assert.That(actor.Position.Y).IsEqualTo(1);
    }

    [Test]
    public async Task CreateSnapshot_Lists_Tiles_And_Actors()
    {
        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        var snap = world.CreateSnapshot();
        await Assert.That(snap.Width).IsEqualTo(4);
        await Assert.That(snap.Height).IsEqualTo(4);
        await Assert.That(snap.Tiles).HasCount(16);
        await Assert.That(snap.Actors).HasCount(1);
    }

    [Test]
    public async Task TryMoveForward_Returns_False_When_Blocked_By_Wall()
    {
        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        var actor = world.ActorsById[1];
        actor.Direction = Direction.North;

        var moved = RobotWorldCommands.TryMoveForward(world, actor);
        await Assert.That(moved).IsFalse();
        await Assert.That(actor.Position.X).IsEqualTo(1);
        await Assert.That(actor.Position.Y).IsEqualTo(1);
    }

    [Test]
    public async Task TurnLeft_Rotates_East_To_North()
    {
        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        var actor = world.ActorsById[1];
        actor.Direction = Direction.East;

        RobotWorldCommands.TurnLeft(world, actor);

        await Assert.That(actor.Direction).IsEqualTo(Direction.North);
    }

    [Test]
    public async Task TurnRight_Rotates_East_To_South()
    {
        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        var actor = world.ActorsById[1];
        actor.Direction = Direction.East;

        RobotWorldCommands.TurnRight(world, actor);

        await Assert.That(actor.Direction).IsEqualTo(Direction.South);
    }

    [Test]
    public async Task FrontIsClear_False_When_Facing_Wall()
    {
        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        var actor = world.ActorsById[1];
        actor.Direction = Direction.North;

        await Assert.That(RobotWorldCommands.FrontIsClear(world, actor)).IsFalse();
    }

    [Test]
    public async Task FrontIsClear_True_When_Open_Ahead()
    {
        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        var actor = world.ActorsById[1];
        actor.Direction = Direction.East;

        await Assert.That(RobotWorldCommands.FrontIsClear(world, actor)).IsTrue();
    }
}
