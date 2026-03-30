using RoboSharp.IL;
using RoboSharp.World;

namespace RoboSharp.Runtime;

/// <summary>Instruction-stepping session over a <see cref="RoboProgram"/> (v1 runtime spec: docs/runtime/v1-runtime-spec.md).</summary>
public sealed class RoboInterpreterSession
{
    private readonly RoboInterpreterEngine _engine = new();
    private RoboProgram? _program;

    public void Start(RoboProgram program, RobotWorld world, TextWriter stdout, TextWriter stderr)
    {
        ArgumentNullException.ThrowIfNull(program);
        ArgumentNullException.ThrowIfNull(world);
        _program = program;
        var err = _engine.Initialize(program, world, stdout, stderr);
        if (err is not null)
            throw new InvalidOperationException(err.Fault?.Message ?? "Initialization failed.");
    }

    public int? CurrentFunctionIndex => _engine.CurrentFunctionIndex;

    public int? InstructionPointer => _engine.CurrentInstructionPointer;

    public bool IsComplete => !_engine.HasActiveFrames;

    /// <summary>Execute exactly one instruction.</summary>
    public InterpreterStepResult Step()
    {
        var program = _program ?? throw new InvalidOperationException("Call Start first.");
        var r = _engine.ExecuteNext(program);
        if (r is null)
            return InterpreterStepResult.Advanced;
        if (r.Succeeded)
            return InterpreterStepResult.Completed;
        return InterpreterStepResult.Faulted(r.Fault!);
    }

    /// <summary>Run until completion, fault, or <paramref name="maxSteps"/> instructions.</summary>
    public ExecutionResult RunToEnd(int maxSteps)
    {
        var program = _program ?? throw new InvalidOperationException("Call Start first.");
        for (var i = 0; i < maxSteps; i++)
        {
            var r = _engine.ExecuteNext(program);
            if (r is null)
                continue;
            if (r.Succeeded)
                return ExecutionResult.Completed;
            return r;
        }

        return ExecutionResult.Failed(new RuntimeFault("Step limit exceeded.", _engine.CurrentFunctionIndex ?? -1, _engine.CurrentInstructionPointer ?? -1));
    }
}
