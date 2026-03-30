using RoboSharp.IL;
using RoboSharp.Toolchain;
using RoboSharp.World;
using RoboSharp.Workspaces;

namespace RoboSharp.Application;

public interface IRoboSharpExecutionService
{
    ProgramRunResult RunSource(string source, RobotWorld world, TextWriter stdout, TextWriter stderr);

    ProgramRunResult RunRoboProgram(RoboProgram program, RobotWorld world, TextWriter stdout, TextWriter stderr);

    ValueTask<ProgramRunResult> RunExecutableJsonAsync(
        string json,
        RobotWorld world,
        TextWriter stdout,
        TextWriter stderr,
        CancellationToken cancellationToken = default);

    ValueTask<WorkspaceBuildResult> BuildWorkspaceAsync(IRoboWorkspace workspace, CancellationToken cancellationToken = default);

    ValueTask<ProgramRunResult> BuildAndRunWorkspaceAsync(
        IRoboWorkspace workspace,
        RobotWorld world,
        TextWriter stdout,
        TextWriter stderr,
        CancellationToken cancellationToken = default);
}
