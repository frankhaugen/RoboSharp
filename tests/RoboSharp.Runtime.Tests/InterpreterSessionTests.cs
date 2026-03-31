using System.IO;
using RoboSharp.Runtime;
using RoboSharp.Toolchain;
using RoboSharp.World;

namespace RoboSharp.Runtime.Tests;

public class InterpreterSessionTests
{
    [Test]
    public async Task Step_Until_Completed_For_Print_Main()
    {
        const string source = """
            print(1);
            """;

        var compiled = RoboSharpCompiler.Compile(source);
        await Assert.That(compiled.Succeeded).IsTrue();

        var session = new RoboInterpreterSession();
        session.Start(compiled.Program!, RobotWorldFactory.CreateBorderedEmpty(4, 4), TextWriter.Null, TextWriter.Null);

        var steps = 0;
        while (true)
        {
            var r = session.Step();
            steps++;
            if (r.Kind == InterpreterStepKind.Completed)
                break;
            if (r.Kind == InterpreterStepKind.Faulted)
                throw new InvalidOperationException(r.Fault?.Message);
        }

        await Assert.That(steps).IsGreaterThan(1);
        await Assert.That(session.IsComplete).IsTrue();
    }

    [Test]
    public async Task RunToEnd_Respects_Step_Limit()
    {
        const string source = """
            while (true)
            {
                print(1);
            }
            """;

        var compiled = RoboSharpCompiler.Compile(source);
        await Assert.That(compiled.Succeeded).IsTrue();

        var session = new RoboInterpreterSession();
        session.Start(compiled.Program!, RobotWorldFactory.CreateBorderedEmpty(4, 4), TextWriter.Null, TextWriter.Null);

        var r = session.RunToEnd(maxSteps: 20);
        await Assert.That(r.Succeeded).IsFalse();
        await Assert.That(r.Fault!.Message).Contains("Step limit");
    }
}
