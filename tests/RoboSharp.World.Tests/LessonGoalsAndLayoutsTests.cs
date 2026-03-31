using RoboSharp.World;

namespace RoboSharp.World.Tests;

public class LessonGoalsAndLayoutsTests
{
    [Test]
    public async Task WorldLayoutParser_Places_Robot_Goal_And_Walls()
    {
        var lines = new[]
        {
            "###",
            "#@G",
            "###",
        };
        var r = WorldLayoutParser.Parse(lines);
        await Assert.That(r.Terrain[1, 1]).IsEqualTo(TerrainCellKind.Empty);
        await Assert.That(r.Terrain[2, 1]).IsEqualTo(TerrainCellKind.Goal);
        await Assert.That(r.RobotStart).IsEqualTo(new GridPosition(1, 1));
        await Assert.That(r.PrimaryGoal).IsEqualTo(new GridPosition(2, 1));
    }

    [Test]
    public async Task LessonGoalEvaluator_ReachesGoal_Gives_PositiveScore()
    {
        var world = RobotWorldPresets.Create(RobotWorldPresets.GoalCornerId);
        var g = world.Metadata.PrimaryGoalPosition!.Value;
        world.ActorsById[1].Position = g;
        var r = LessonGoalEvaluator.Evaluate(world, ilInstructionSteps: 24);
        await Assert.That(r.ReachedGoal).IsTrue();
        await Assert.That(r.Score).IsGreaterThan(0);
    }

    [Test]
    public async Task LessonGoalEvaluator_NoGoal_FreePlay_Summary()
    {
        var world = RobotWorldFactory.CreateBorderedEmpty(6, 6);
        var r = LessonGoalEvaluator.Evaluate(world, ilInstructionSteps: 5);
        await Assert.That(r.ReachedGoal).IsTrue();
        await Assert.That(r.SummaryForKids).Contains("Free play", StringComparison.Ordinal);
    }
}
