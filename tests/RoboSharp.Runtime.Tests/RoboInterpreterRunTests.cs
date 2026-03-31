using System.IO;
using RoboSharp.Toolchain;
using RoboSharp.World;

namespace RoboSharp.Runtime.Tests;

public class RoboInterpreterRunTests
{
    [Test]
    public async Task Run_Completes_With_Print_To_Stdout()
    {
        const string source = """
            print(5);
            """;

        var compiled = RoboSharpCompiler.Compile(source);
        await Assert.That(compiled.Succeeded).IsTrue();

        var interpreter = new RoboInterpreter();
        using var stdout = new StringWriter();
        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        var result = interpreter.Run(compiled.Program!, world, stdout, TextWriter.Null);

        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(stdout.ToString().Trim()).IsEqualTo("5");
    }

    [Test]
    public async Task Run_Prints_String_Literal_To_Stdout()
    {
        const string source = """
            print("hello");
            """;

        var compiled = RoboSharpCompiler.Compile(source);
        await Assert.That(compiled.Succeeded).IsTrue();

        var interpreter = new RoboInterpreter();
        using var stdout = new StringWriter();
        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        var result = interpreter.Run(compiled.Program!, world, stdout, TextWriter.Null);

        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(stdout.ToString().Trim()).IsEqualTo("hello");
    }

    [Test]
    public async Task Run_TurnLeft_Updates_Robot_Direction()
    {
        const string source = """
            turnLeft();
            """;

        var compiled = RoboSharpCompiler.Compile(source);
        await Assert.That(compiled.Succeeded).IsTrue();

        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        var actor = world.ActorsById[1];
        await Assert.That(actor.Direction).IsEqualTo(Direction.East);

        var interpreter = new RoboInterpreter();
        var result = interpreter.Run(compiled.Program!, world, TextWriter.Null, TextWriter.Null);

        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(actor.Direction).IsEqualTo(Direction.North);
    }

    [Test]
    public async Task Run_TurnRight_Updates_Robot_Direction()
    {
        const string source = """
            turnRight();
            """;

        var compiled = RoboSharpCompiler.Compile(source);
        await Assert.That(compiled.Succeeded).IsTrue();

        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        var actor = world.ActorsById[1];

        var interpreter = new RoboInterpreter();
        var result = interpreter.Run(compiled.Program!, world, TextWriter.Null, TextWriter.Null);

        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(actor.Direction).IsEqualTo(Direction.South);
    }
}
