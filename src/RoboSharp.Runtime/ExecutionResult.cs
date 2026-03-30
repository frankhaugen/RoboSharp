namespace RoboSharp.Runtime;

public sealed class ExecutionResult
{
    public static ExecutionResult Completed { get; } = new() { Succeeded = true };

    public static ExecutionResult Failed(RuntimeFault fault) => new() { Succeeded = false, Fault = fault };

    public bool Succeeded { get; private init; }
    public RuntimeFault? Fault { get; private init; }
}
