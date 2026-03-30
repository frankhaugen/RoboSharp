namespace RoboSharp.Runtime;

public readonly record struct InterpreterStepResult(InterpreterStepKind Kind, RuntimeFault? Fault = null)
{
    public static InterpreterStepResult Advanced { get; } = new(InterpreterStepKind.Advanced);
    public static InterpreterStepResult Completed { get; } = new(InterpreterStepKind.Completed);
    public static InterpreterStepResult Faulted(RuntimeFault fault) => new(InterpreterStepKind.Faulted, fault);
    public static InterpreterStepResult LimitExceeded { get; } = new(InterpreterStepKind.StepLimitExceeded);
}
