using System.IO;
using RoboSharp.Toolchain;
using RoboSharp.World;

namespace RoboSharp.Toolchain.Tests;

public class PipelineTests
{
    [Test]
    public async Task CompileAndRun_Prints_Integer()
    {
        const string source = """
            void main()
            {
                print(42);
            }
            """;

        var world = RobotWorldFactory.CreateBorderedEmpty(5, 5);
        var stdout = new StringWriter();
        var result = RoboSharpPipeline.CompileAndRun(source, world, stdout, TextWriter.Null);

        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(stdout.ToString().Trim()).IsEqualTo("42");
    }

    [Test]
    public async Task CompileAndRun_Move_Updates_World()
    {
        const string source = """
            void main()
            {
                move();
            }
            """;

        var world = RobotWorldFactory.CreateBorderedEmpty(6, 6);
        var actor = world.ActorsById[1];
        await Assert.That(actor.Position.X).IsEqualTo(1);

        var result = RoboSharpPipeline.CompileAndRun(source, world, TextWriter.Null, TextWriter.Null);

        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(actor.Position.X).IsEqualTo(2);
    }
}
