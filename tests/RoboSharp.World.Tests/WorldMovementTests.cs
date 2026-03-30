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
}
