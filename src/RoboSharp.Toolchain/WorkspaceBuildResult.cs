namespace RoboSharp.Toolchain;

/// <summary>Outcome of compiling all workspace sources and writing toolchain artifacts.</summary>
public sealed class WorkspaceBuildResult
{
    public required bool Success { get; init; }

    public required CompileResult CompileResult { get; init; }
}
