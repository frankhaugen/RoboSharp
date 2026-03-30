using System.Text;
using RoboSharp.Application;
using RoboSharp.IO;
using RoboSharp.Toolchain;
using RoboSharp.World;

namespace RoboSharp.Player;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        if (args.Length != 1)
        {
            await Console.Error.WriteLineAsync("Usage: RoboSharp.Player <path-to-.roboexe>").ConfigureAwait(false);
            return (int)RoboSharpExitCode.InvalidArguments;
        }

        var fullPath = Path.GetFullPath(args[0]);
        var fileInfo = new FileInfo(fullPath);
        if (!fileInfo.Exists)
        {
            await Console.Error.WriteLineAsync($"File not found: {fullPath}").ConfigureAwait(false);
            return (int)RoboSharpExitCode.InvalidExecutableOrProject;
        }

        var parent = fileInfo.Directory ?? throw new InvalidOperationException("Executable path has no parent directory.");
        var fs = new PhysicalRoboFileSystem(parent);
        var paths = new RoboPathService();
        var roboFile = fs.GetFile(paths.Combine(fs.Root.Uri, fileInfo.Name));

        string json;
        try
        {
            json = await roboFile.ReadAllTextAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Could not read executable: {ex.Message}").ConfigureAwait(false);
            return (int)RoboSharpExitCode.InvalidExecutableOrProject;
        }

        await using var stdout = new StreamWriter(Console.OpenStandardOutput(), Encoding.UTF8) { AutoFlush = true };
        await using var stderr = new StreamWriter(Console.OpenStandardError(), Encoding.UTF8) { AutoFlush = true };

        var world = RobotWorldFactory.CreateBorderedEmpty(16, 16);
        var execution = new RoboSharpExecutionService(new WorkspaceBuildService());
        var result = await execution.RunExecutableJsonAsync(json, world, stdout, stderr).ConfigureAwait(false);

        return (int)result.ExitCode;
    }
}
