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
            void main()
            {
                print(5);
            }
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
}
