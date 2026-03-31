using System.IO;
using RoboSharp.Runtime;
using RoboSharp.Toolchain;
using RoboSharp.World;

namespace RoboSharp.Integration.Tests;

public class InterpreterSessionIntegrationTests
{
    [Test]
    public async Task StepUntilComplete_TopLevelAndProcedure_ReachesExpectedWorldState()
    {
        const string source = """
            TurnAndStep()
            {
                turnLeft();
                move();
            }

            TurnAndStep();
            """;

        var compiled = RoboSharpCompiler.Compile(source);
        await Assert.That(compiled.Succeeded).IsTrue();

        var world = RobotWorldFactory.CreateBorderedEmpty(6, 6);
        var session = new RoboInterpreterSession();
        session.Start(compiled.Program!, world, TextWriter.Null, TextWriter.Null);

        var steps = 0;
        const int maxSteps = 50_000;
        while (steps < maxSteps)
        {
            var r = session.Step();
            steps++;
            if (r.Kind == InterpreterStepKind.Completed)
                break;
            if (r.Kind == InterpreterStepKind.Faulted)
                throw new InvalidOperationException(r.Fault?.Message);
        }

        await Assert.That(session.IsComplete).IsTrue();
        var actorId = world.Metadata.PrimaryActorId ?? 1;
        var robot = world.ActorsById[actorId];
        await Assert.That(robot.Direction).IsEqualTo(Direction.North);
        await Assert.That(robot.Position.X).IsEqualTo(1);
        await Assert.That(robot.Position.Y).IsEqualTo(1);
    }
}
