using System.IO;
using Microsoft.Extensions.DependencyInjection;
using RoboSharp.Application;
using RoboSharp.World;

namespace RoboSharp.Application.Tests;

public class ExecutionServiceMaxInstructionsTests
{
    [Test]
    public async Task RunSource_WithMaxInstructions_StopsOnInfiniteLoop()
    {
        const string source = """
            while (true)
            {
                print(1);
            }
            """;

        var services = new ServiceCollection();
        services.AddRoboSharpApplication();
        using var provider = services.BuildServiceProvider();
        var execution = provider.GetRequiredService<IRoboSharpExecutionService>();

        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        using var stdout = new StringWriter();
        using var stderr = new StringWriter();

        var result = execution.RunSource(
            source,
            world,
            stdout,
            stderr,
            new RunExecutionOptions { MaxInstructions = 30 });

        await Assert.That(result.Succeeded).IsFalse();
        await Assert.That(result.ExitCode).IsEqualTo(RoboSharpExitCode.RuntimeFault);
        await Assert.That(result.Fault!.Message).Contains("Step limit");
    }
}
