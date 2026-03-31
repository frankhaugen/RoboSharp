using System.IO;
using RoboSharp.Runtime;
using RoboSharp.Toolchain;
using RoboSharp.World;
using RoboSharp.Workspaces;

namespace RoboSharp.Integration.Tests;

public class WorkspaceBuildDeserializeRunTests
{
    [Test]
    public async Task BuildWritesJsonExecutable_DeserializeAndRun_PrintsOutput()
    {
        const string source = """
            print(77);
            """;

        var project = new RoboSharpProject("Integration", ["App.robo"], "App.robo");
        var workspace = await RoboTemporaryWorkspace.CreateInMemoryAsync(project);
        await workspace.GetSourceFiles()[0].WriteAllTextAsync(source);

        var build = new WorkspaceBuildService();
        var result = await build.BuildAsync(workspace);

        await Assert.That(result.Success).IsTrue();

        var exeFile = workspace.GetArtifactFile(ArtifactKind.Executable, workspace.ActiveConfiguration);
        await Assert.That(exeFile.Exists()).IsTrue();

        var json = await exeFile.ReadAllTextAsync();
        var executable = RoboExecutableJsonSerializer.Deserialize(json);

        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        using var stdout = new StringWriter();
        var interpreter = new RoboInterpreter();
        var run = interpreter.Run(executable.Program, world, stdout, TextWriter.Null);

        await Assert.That(run.Succeeded).IsTrue();
        await Assert.That(stdout.ToString().Trim()).IsEqualTo("77");
    }
}
