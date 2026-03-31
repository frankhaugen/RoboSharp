using System.IO;
using RoboSharp.IL;
using RoboSharp.Runtime;
using RoboSharp.Toolchain;
using RoboSharp.World;

namespace RoboSharp.Integration.Tests;

public class ExecutableJsonRoundTripTests
{
    [Test]
    public async Task SerializeDeserialize_RunMatchesDirectProgram()
    {
        const string source = """
            print(42);
            """;

        var compiled = RoboSharpCompiler.Compile(source);
        await Assert.That(compiled.Succeeded).IsTrue();

        var json = RoboExecutableJsonSerializer.Serialize(compiled.Executable!);
        await Assert.That(json.Length).IsGreaterThan(50);

        var roundTripped = RoboExecutableJsonSerializer.Deserialize(json);
        await Assert.That(roundTripped.FormatVersion).IsEqualTo(RoboExecutable.CurrentFormatVersion);

        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        using var stdoutA = new StringWriter();
        using var stdoutB = new StringWriter();

        var interpreter = new RoboInterpreter();
        var direct = interpreter.Run(compiled.Program!, world, stdoutA, TextWriter.Null);
        await Assert.That(direct.Succeeded).IsTrue();

        var world2 = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        var viaJson = interpreter.Run(roundTripped.Program, world2, stdoutB, TextWriter.Null);
        await Assert.That(viaJson.Succeeded).IsTrue();

        await Assert.That(stdoutB.ToString().Trim()).IsEqualTo(stdoutA.ToString().Trim());
        await Assert.That(stdoutB.ToString().Trim()).IsEqualTo("42");
    }
}
