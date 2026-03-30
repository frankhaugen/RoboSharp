namespace RoboSharp.Studio.Pipeline;

/// <summary>
/// Teaching seam: runs the visible Language pipeline stages on source text.
/// </summary>
public interface IPipelineInspectionService
{
    /// <summary>Lex + parse; returns immutable snapshot for all inspection panels.</summary>
    PipelineSnapshot Inspect(string source);
}
