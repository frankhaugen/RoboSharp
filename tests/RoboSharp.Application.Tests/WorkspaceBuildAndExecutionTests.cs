using System.IO;
using Microsoft.Extensions.DependencyInjection;
using RoboSharp.Application;
using RoboSharp.Toolchain;
using RoboSharp.World;
using RoboSharp.Workspaces;

namespace RoboSharp.Application.Tests;

public class WorkspaceBuildAndExecutionTests
{
    [Test]
    public async Task WorkspaceBuildService_WritesExecutableAndIl_ForValidProject()
    {
        const string source = """
            print(1);
            """;

        var project = new RoboSharpProject("T", ["Main.robo"], "Main.robo");
        var workspace = await RoboTemporaryWorkspace.CreateInMemoryAsync(project);
        await workspace.GetSourceFiles()[0].WriteAllTextAsync(source);

        var build = new WorkspaceBuildService();
        var result = await build.BuildAsync(workspace);

        await Assert.That(result.Success).IsTrue();
        var exe = workspace.GetArtifactFile(ArtifactKind.Executable, workspace.ActiveConfiguration);
        var il = workspace.GetArtifactFile(ArtifactKind.Il, workspace.ActiveConfiguration);
        await Assert.That(exe.Exists()).IsTrue();
        await Assert.That(il.Exists()).IsTrue();
        var exeText = await exe.ReadAllTextAsync();
        await Assert.That(exeText.Length).IsGreaterThan(20);
    }

    [Test]
    public async Task ExecutionService_BuildAndRunWorkspace_PrintsOutput()
    {
        const string source = """
            print(99);
            """;

        var project = new RoboSharpProject("App", ["Entry.robo"], "Entry.robo");
        var workspace = await RoboTemporaryWorkspace.CreateInMemoryAsync(project);
        await workspace.GetSourceFiles()[0].WriteAllTextAsync(source);

        var services = new ServiceCollection();
        services.AddRoboSharpApplication();
        using var provider = services.BuildServiceProvider();
        var execution = provider.GetRequiredService<IRoboSharpExecutionService>();

        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        using var stdout = new StringWriter();
        using var stderr = new StringWriter();
        var run = await execution.BuildAndRunWorkspaceAsync(workspace, world, stdout, stderr);

        await Assert.That(run.Succeeded).IsTrue();
        await Assert.That(run.ExitCode).IsEqualTo(RoboSharpExitCode.Success);
        await Assert.That(stdout.ToString().Trim()).IsEqualTo("99");
    }
}
