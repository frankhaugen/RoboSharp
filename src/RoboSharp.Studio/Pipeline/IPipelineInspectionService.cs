using RoboSharp.World;

namespace RoboSharp.Studio.Pipeline;

/// <summary>
/// Teaching seam: runs the visible Language pipeline stages on source text.
/// </summary>
public interface IPipelineInspectionService
{
    /// <summary>Lex → parse → compile through lowering. Does not execute the interpreter.</summary>
    PipelineSnapshot InspectBuildOnly(string source);

    /// <summary>Compile (implicit build), then step the interpreter with optional delay between steps.</summary>
    Task<PipelineSnapshot> InspectBuildAndRunAsync(
        string source,
        StudioRunSpeed speed,
        IProgress<RobotWorldSnapshot>? worldProgress,
        CancellationToken cancellationToken);
}
