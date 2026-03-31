using RoboSharp.IL;
using RoboSharp.Language;
using RoboSharp.Runtime;
using RoboSharp.Semantics;

namespace RoboSharp.Toolchain;

public sealed class PipelineResult
{
    public SyntaxTree? SyntaxTree { get; init; }
    public SemanticModel? SemanticModel { get; init; }
    public RoboProgram? Program { get; init; }
    public RoboExecutable? Executable { get; init; }
    public ExecutionResult? Execution { get; init; }
    public PipelineStage? FailureStage { get; init; }

    public bool Succeeded => FailureStage is null && Execution?.Succeeded == true;
}