using RoboSharp.IL;
using RoboSharp.Toolchain;
using RoboSharp.World;
using RoboSharp.Workspaces;

namespace RoboSharp.Application;

public interface IRoboSharpExecutionService
{
    ProgramRunResult RunSource(
        string source,
        RobotWorld world,
        TextWriter stdout,
        TextWriter stderr,
        RunExecutionOptions? options = null);

    ProgramRunResult RunRoboProgram(
        RoboProgram program,
        RobotWorld world,
        TextWriter stdout,
        TextWriter stderr,
        RunExecutionOptions? options = null);

    ValueTask<ProgramRunResult> RunExecutableJsonAsync(
        string json,
        RobotWorld world,
        TextWriter stdout,
        TextWriter stderr,
        RunExecutionOptions? options = null,
        CancellationToken cancellationToken = default);

    ValueTask<WorkspaceBuildResult> BuildWorkspaceAsync(IRoboWorkspace workspace, CancellationToken cancellationToken = default);

    ValueTask<ProgramRunResult> BuildAndRunWorkspaceAsync(
        IRoboWorkspace workspace,
        RobotWorld world,
        TextWriter stdout,
        TextWriter stderr,
        RunExecutionOptions? options = null,
        CancellationToken cancellationToken = default);
}
