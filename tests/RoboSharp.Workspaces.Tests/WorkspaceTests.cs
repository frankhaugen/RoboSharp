using Microsoft.Extensions.DependencyInjection;
using RoboSharp.IO;
using RoboSharp.Workspaces;

namespace RoboSharp.Workspaces.Tests;

public class WorkspaceTests
{
    [Test]
    public async Task ProjectLoader_RejectsStartupNotListedInSources()
    {
        var paths = new RoboPathService();
        var fs = new InMemoryRoboFileSystem(
            new UriBuilder(RoboUriSchemes.Memory, string.Empty, -1, "/proj/").Uri,
            pathService: paths);
        await fs.Root.GetFile("Main.robo").WriteAllTextAsync("//");

        var projectFile = fs.Root.GetFile("app.robosharp");
        await projectFile.WriteAllTextAsync(
            """
            {
              "startupFile": "Missing.robo",
              "sourceFiles": [ "Main.robo" ]
            }
            """);

        var loader = new ProjectLoader();
        await Assert.That(async () => await loader.LoadAsync(projectFile)).Throws<ProjectLoadException>();
    }

    [Test]
    public async Task ProjectLoader_SaveThenLoad_RoundTrips()
    {
        var paths = new RoboPathService();
        var fs = new InMemoryRoboFileSystem(
            new UriBuilder(RoboUriSchemes.Memory, string.Empty, -1, "/p/").Uri,
            pathService: paths);
        var projectFile = fs.Root.GetFile("x.robosharp");
        var original = new RoboSharpProject("Demo", ["a.robo", "b/c.robo"], "a.robo");
        var loader = new ProjectLoader();
        await loader.SaveAsync(original, projectFile);
        var loaded = await loader.LoadAsync(projectFile);

        await Assert.That(loaded.Name).IsEqualTo("Demo");
        await Assert.That(loaded.StartupFileRelative).IsEqualTo("a.robo");
        await Assert.That(loaded.SourceFilesRelative.Count).IsEqualTo(2);
        await Assert.That(loaded.SourceFilesRelative[0]).IsEqualTo("a.robo");
        await Assert.That(loaded.SourceFilesRelative[1]).IsEqualTo("b/c.robo");
    }

    [Test]
    public async Task WorkspaceLoader_LoadsWhenSourcesExist()
    {
        var temp = Directory.CreateTempSubdirectory("robosharp_ws_");
        try
        {
            var fs = new PhysicalRoboFileSystem(new DirectoryInfo(temp.FullName));
            var paths = new RoboPathService();
            await fs.Root.GetFile("Hello.robo").WriteAllTextAsync("//");
            var projectFile = fs.Root.GetFile("App.robosharp");
            await projectFile.WriteAllTextAsync(
                """
                {
                  "startupFile": "Hello.robo",
                  "sourceFiles": [ "Hello.robo" ]
                }
                """);

            var loader = new WorkspaceLoader(pathService: paths);
            var workspace = await loader.LoadAsync(fs, projectFile);

            await Assert.That(workspace).IsAssignableTo(typeof(RoboProjectWorkspace));
            await Assert.That(workspace.Project.StartupFileRelative).IsEqualTo("Hello.robo");
            await Assert.That(workspace.GetSourceFiles().Count).IsEqualTo(1);
            await Assert.That(workspace.GetStartupSourceFile().Exists()).IsTrue();
        }
        finally
        {
            temp.Delete(recursive: true);
        }
    }

    [Test]
    public async Task WorkspaceLoader_ThrowsWhenSourceMissing()
    {
        var paths = new RoboPathService();
        var fs = new InMemoryRoboFileSystem(
            new UriBuilder(RoboUriSchemes.Memory, string.Empty, -1, "/q/").Uri,
            pathService: paths);
        var projectFile = fs.Root.GetFile("p.robosharp");
        await projectFile.WriteAllTextAsync(
            """
            {
              "startupFile": "Only.robo",
              "sourceFiles": [ "Only.robo" ]
            }
            """);

        var loader = new WorkspaceLoader(pathService: paths);
        await Assert.That(async () => await loader.LoadAsync(fs, projectFile)).Throws<ProjectLoadException>();
    }

    [Test]
    public async Task ArtifactLayout_MatchesTeachingFileNames()
    {
        var paths = new RoboPathService();
        var layout = new DefaultBuildArtifactLayout();
        var fs = new InMemoryRoboFileSystem(
            new UriBuilder(RoboUriSchemes.Memory, string.Empty, -1, "/w/").Uri,
            pathService: paths);
        await fs.Root.GetFile("src/Entry.robo").WriteAllTextAsync("//");
        var projectFile = fs.Root.GetFile("Lesson.robosharp");
        await projectFile.WriteAllTextAsync(
            """
            {
              "startupFile": "src/Entry.robo",
              "sourceFiles": [ "src/Entry.robo" ]
            }
            """);

        var loader = new WorkspaceLoader(artifactLayout: layout, pathService: paths);
        var workspace = await loader.LoadAsync(fs, projectFile);

        var syntax = workspace.GetArtifactFile(ArtifactKind.SyntaxTree, "Debug", "src/Entry.robo");
        await Assert.That(syntax.Name).IsEqualTo("Entry.roboast.json");

        var bound = workspace.GetArtifactFile(ArtifactKind.BoundProgram, "Debug");
        await Assert.That(bound.Name).IsEqualTo("Lesson.robobind.json");

        var il = workspace.GetArtifactFile(ArtifactKind.Il, "Debug");
        await Assert.That(il.Name).IsEqualTo("Lesson.roboil.json");

        var pdb = workspace.GetArtifactFile(ArtifactKind.DebugSymbols, "Debug");
        await Assert.That(pdb.Name).IsEqualTo("Lesson.robo.pdb.json");

        var exe = workspace.GetArtifactFile(ArtifactKind.Executable, "Release");
        await Assert.That(exe.Name).IsEqualTo("Lesson.roboexe");
    }

    [Test]
    public async Task RoboTemporaryWorkspace_CreateInMemoryAsync_ProducesRunnableLayout()
    {
        var project = new RoboSharpProject("Scratch", ["Main.robo"], "Main.robo");
        var workspace = await RoboTemporaryWorkspace.CreateInMemoryAsync(project);

        await Assert.That(workspace).IsAssignableTo(typeof(RoboTemporaryWorkspace));
        await Assert.That(workspace.GetSourceFiles()[0].Exists()).IsTrue();
        await Assert.That(workspace.ProjectFile.Exists()).IsTrue();
    }

    [Test]
    public async Task AddRoboSharpWorkspaces_RegistersCoreServices()
    {
        var services = new ServiceCollection();
        services.AddRoboSharpWorkspaces();

        using var provider = services.BuildServiceProvider();
        await Assert.That(provider.GetRequiredService<IWorkspaceLoader>()).IsNotNull();
        await Assert.That(provider.GetRequiredService<IProjectLoader>()).IsNotNull();
        await Assert.That(provider.GetRequiredService<IBuildArtifactLayout>()).IsNotNull();
        await Assert.That(provider.GetRequiredService<IRoboPathService>()).IsNotNull();
    }
}
