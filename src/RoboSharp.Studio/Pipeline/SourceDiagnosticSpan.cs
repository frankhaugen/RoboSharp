namespace RoboSharp.Studio.Pipeline;

/// <summary>Character span in <see cref="PipelineSnapshot.Source"/> for parse/semantic error squiggles in the editor.</summary>
public readonly record struct SourceDiagnosticSpan(int Start, int Length);
