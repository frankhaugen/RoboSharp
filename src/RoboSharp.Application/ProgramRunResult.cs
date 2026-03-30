using RoboSharp.Runtime;

namespace RoboSharp.Application;

public sealed class ProgramRunResult
{
    public required bool Succeeded { get; init; }

    public required RoboSharpExitCode ExitCode { get; init; }

    public RuntimeFault? Fault { get; init; }
}
