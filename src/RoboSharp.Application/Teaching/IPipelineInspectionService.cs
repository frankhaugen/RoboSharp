namespace RoboSharp.Application.Teaching;

/// <summary>Teaching seam: runs visible Language pipeline stages on source text.</summary>
public interface IPipelineInspectionService
{
    /// <summary>Lex → parse → compile through lowering. Does not execute the interpreter.</summary>
    PipelineSnapshot InspectBuildOnly(string source, StudioPipelineOptions options);

    /// <summary>Compile, then step the interpreter with optional delay between steps.</summary>
    Task<PipelineSnapshot> InspectBuildAndRunAsync(
        string source,
        StudioRunSpeed speed,
        StudioPipelineOptions options,
        IProgress<StudioRunProgress>? runProgress,
        CancellationToken cancellationToken);
}
