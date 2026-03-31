using RoboSharp.IL;
using RoboSharp.Language;
using RoboSharp.Semantics;

namespace RoboSharp.Toolchain;

public sealed class CompileResult
{
    public SyntaxTree? SyntaxTree { get; init; }
    public SemanticModel? SemanticModel { get; init; }
    public RoboProgram? Program { get; init; }
    public RoboExecutable? Executable { get; init; }
    public CompilePhase ReachedPhase { get; init; }

    public bool Succeeded => Executable is not null && ReachedPhase == CompilePhase.Lowered;
}