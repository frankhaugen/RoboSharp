namespace RoboSharp.Application;

/// <summary>Process exit codes aligned with <c>docs/toolchain/v1-toolchain-spec.md</c> §11.</summary>
public enum RoboSharpExitCode
{
    Success = 0,
    BuildFailure = 1,
    InvalidExecutableOrProject = 2,
    RuntimeFault = 3,
    InvalidArguments = 4,
}
