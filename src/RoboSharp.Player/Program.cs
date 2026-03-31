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
        if (args.Any(IsHelpSwitch))
        {
            await PrintHelpAsync().ConfigureAwait(false);
            return (int)RoboSharpExitCode.Success;
        }

        if (args.Length == 0)
        {
            await PrintHelpAsync().ConfigureAwait(false);
            return (int)RoboSharpExitCode.InvalidArguments;
        }

        if (!TryParseArgs(args, out var fullPath, out var maxInstructions, out var plainMode, out var parseError))
        {
            await Console.Error.WriteLineAsync(parseError).ConfigureAwait(false);
            return (int)RoboSharpExitCode.InvalidArguments;
        }

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

        var world = RobotWorldFactory.CreateBorderedEmpty(16, 16);
        var execution = new RoboSharpExecutionService(new WorkspaceBuildService());
        RunExecutionOptions? options = maxInstructions is { } n
            ? new RunExecutionOptions { MaxInstructions = n }
            : null;

        if (plainMode)
        {
            await using var stdout = new StreamWriter(Console.OpenStandardOutput(), Encoding.UTF8) { AutoFlush = true };
            await using var stderr = new StreamWriter(Console.OpenStandardError(), Encoding.UTF8) { AutoFlush = true };
            var result = await execution.RunExecutableJsonAsync(json, world, stdout, stderr, options).ConfigureAwait(false);
            return (int)result.ExitCode;
        }

        using var swOut = new StringWriter();
        using var swErr = new StringWriter();
        var runResult = await execution.RunExecutableJsonAsync(json, world, swOut, swErr, options).ConfigureAwait(false);
        var ascii = RobotWorldSnapshotAscii.Format(world.CreateSnapshot());
        PlayerTui.ShowResult(fullPath, runResult, swOut.ToString(), swErr.ToString(), ascii, maxInstructions);
        return (int)runResult.ExitCode;
    }

    private static async Task PrintHelpAsync()
    {
        await Console.Error.WriteLineAsync(
            """
            RoboSharp.Player — run a v1 JSON .roboexe (teaching fake executable).

            Usage:
              RoboSharp.Player [options] <path-to-file.roboexe>

            Options:
              --plain           Stream program stdout/stderr to the console (for piping / CI). Skips the TUI.
              --max-steps <n>   Run at most n IL instructions (then exit with runtime fault if still running).
              --headless        Same as --plain (alias for scripts).
              -h, --help        Show this help.

            Default (no --plain): Spectre.Console TUI with world grid, captured stdout/stderr, and exit status.

            Example (from repository root):
              dotnet run --project src/RoboSharp.Player/RoboSharp.Player.csproj -- samples/hello.roboexe

            See docs/build.md and docs/player/README.md.
            """).ConfigureAwait(false);
    }

    private static bool TryParseArgs(string[] args, out string fullPath, out int? maxInstructions, out bool plainMode, out string error)
    {
        fullPath = "";
        maxInstructions = null;
        plainMode = false;
        error = "";

        var positionals = new List<string>();
        for (var i = 0; i < args.Length; i++)
        {
            var a = args[i];
            if (IsHelpSwitch(a))
                continue;

            if (a is "--headless" or "--plain")
            {
                plainMode = true;
                continue;
            }

            if (a == "--max-steps")
            {
                if (i + 1 >= args.Length)
                {
                    error = "--max-steps requires a positive integer.";
                    return false;
                }

                if (!int.TryParse(args[++i], out var n) || n < 1)
                {
                    error = "--max-steps must be a positive integer.";
                    return false;
                }

                maxInstructions = n;
                continue;
            }

            if (a.StartsWith('-'))
            {
                error = $"Unknown option: {a}";
                return false;
            }

            positionals.Add(a);
        }

        if (positionals.Count != 1)
        {
            error = positionals.Count == 0
                ? "Expected a path to a .roboexe file."
                : "Expected exactly one .roboexe path.";
            return false;
        }

        fullPath = Path.GetFullPath(positionals[0]);
        return true;
    }

    private static bool IsHelpSwitch(string arg) =>
        string.Equals(arg, "-h", StringComparison.OrdinalIgnoreCase)
        || string.Equals(arg, "--help", StringComparison.OrdinalIgnoreCase)
        || string.Equals(arg, "/?", StringComparison.Ordinal);
}
