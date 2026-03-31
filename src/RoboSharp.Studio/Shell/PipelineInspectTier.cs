namespace RoboSharp.Studio.Shell;

/// <summary>Pipeline abstraction level — drives inspector accent color so stages read as a ladder, not a flat tab list.</summary>
public enum PipelineInspectTier
{
    Toolbox,
    Lexical,
    Syntax,
    Diagnostics,
    Semantic,
    VirtualIl,
    Assembly,
    MachineEncoding,
    RuntimeSummary,
}
