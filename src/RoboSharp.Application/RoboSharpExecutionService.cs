using System.Text.Json;
using RoboSharp.IL;
using RoboSharp.Runtime;
using RoboSharp.Toolchain;
using RoboSharp.World;
using RoboSharp.Workspaces;

namespace RoboSharp.Application;

public sealed class RoboSharpExecutionService(WorkspaceBuildService workspaceBuild) : IRoboSharpExecutionService
{
    private readonly WorkspaceBuildService _workspaceBuild = workspaceBuild ?? throw new ArgumentNullException(nameof(workspaceBuild));

    public ProgramRunResult RunSource(string source, RobotWorld world, TextWriter stdout, TextWriter stderr)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(world);
        ArgumentNullException.ThrowIfNull(stdout);
        ArgumentNullException.ThrowIfNull(stderr);

        var pipeline = RoboSharpPipeline.CompileAndRun(source, world, stdout, stderr);
        return MapPipelineResult(pipeline);
    }

    public ProgramRunResult RunRoboProgram(RoboProgram program, RobotWorld world, TextWriter stdout, TextWriter stderr)
    {
        ArgumentNullException.ThrowIfNull(program);
        ArgumentNullException.ThrowIfNull(world);
        ArgumentNullException.ThrowIfNull(stdout);
        ArgumentNullException.ThrowIfNull(stderr);

        var interpreter = new RoboInterpreter();
        var execution = interpreter.Run(program, world, stdout, stderr);
        return MapExecution(execution);
    }

    public async ValueTask<ProgramRunResult> RunExecutableJsonAsync(
        string json,
        RobotWorld world,
        TextWriter stdout,
        TextWriter stderr,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(json);
        ArgumentNullException.ThrowIfNull(world);
        ArgumentNullException.ThrowIfNull(stdout);
        ArgumentNullException.ThrowIfNull(stderr);

        RoboExecutable executable;
        try
        {
            executable = await Task.Run(() => RoboExecutableJsonSerializer.Deserialize(json), cancellationToken)
                .ConfigureAwait(false);
        }
        catch (JsonException)
        {
            return new ProgramRunResult
            {
                Succeeded = false,
                ExitCode = RoboSharpExitCode.InvalidExecutableOrProject,
            };
        }

        return RunRoboProgram(executable.Program, world, stdout, stderr);
    }

    public async ValueTask<WorkspaceBuildResult> BuildWorkspaceAsync(
        IRoboWorkspace workspace,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        return await _workspaceBuild.BuildAsync(workspace, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<ProgramRunResult> BuildAndRunWorkspaceAsync(
        IRoboWorkspace workspace,
        RobotWorld world,
        TextWriter stdout,
        TextWriter stderr,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(world);
        ArgumentNullException.ThrowIfNull(stdout);
        ArgumentNullException.ThrowIfNull(stderr);

        var build = await _workspaceBuild.BuildAsync(workspace, cancellationToken).ConfigureAwait(false);
        if (!build.Success)
        {
            return MapFailedCompile(build.CompileResult);
        }

        var program = build.CompileResult.Executable!.Program;
        return RunRoboProgram(program, world, stdout, stderr);
    }

    private static ProgramRunResult MapPipelineResult(PipelineResult pipeline)
    {
        if (pipeline.Succeeded)
        {
            return new ProgramRunResult { Succeeded = true, ExitCode = RoboSharpExitCode.Success };
        }

        if (pipeline.FailureStage is PipelineStage.Parse or PipelineStage.Semantics)
        {
            return new ProgramRunResult { Succeeded = false, ExitCode = RoboSharpExitCode.BuildFailure };
        }

        return new ProgramRunResult
        {
            Succeeded = false,
            ExitCode = RoboSharpExitCode.RuntimeFault,
            Fault = pipeline.Execution?.Fault,
        };
    }

    private static ProgramRunResult MapExecution(ExecutionResult execution)
    {
        if (execution.Succeeded)
        {
            return new ProgramRunResult { Succeeded = true, ExitCode = RoboSharpExitCode.Success };
        }

        return new ProgramRunResult
        {
            Succeeded = false,
            ExitCode = RoboSharpExitCode.RuntimeFault,
            Fault = execution.Fault,
        };
    }

    private static ProgramRunResult MapFailedCompile(CompileResult compile)
    {
        _ = compile;
        return new ProgramRunResult { Succeeded = false, ExitCode = RoboSharpExitCode.BuildFailure };
    }
}
