namespace RoboSharp.Locales;

public interface IStudioPanelTexts
{
    string ColoredSourceTitle { get; }
    string ColoredSourceSubtitle { get; }
    string ColoredSourcePreamble { get; }
    string ColoredSourceEmpty { get; }
    string LessonToolboxTitle { get; }
    string LessonToolboxSubtitle { get; }
    string LessonToolboxPreamble { get; }
    string LessonToolboxBuildPrompt { get; }
    string TokensTitle { get; }
    string TokensSubtitle { get; }
    string TokensPreamble { get; }
    string SyntaxTreeTitle { get; }
    string SyntaxTreeSubtitle { get; }
    string SyntaxTreePreamble { get; }
    string DiagnosticsTitle { get; }
    string DiagnosticsSubtitle { get; }
    string DiagnosticsPreamble { get; }
    string DiagnosticsNone { get; }
    /// <summary>Prefix for a runtime fault line in the diagnostics panel (before the message).</summary>
    string DiagnosticsRuntimePrefix { get; }
    string BoundTreeTitle { get; }
    string BoundTreeSubtitle { get; }
    string BoundTreePreamble { get; }
    string BoundTreeNeedParseFirst { get; }
    string BoundTreeSemanticsStopped { get; }
    string BoundTreeUnexpectedEmpty { get; }
    string BoundTreeBuildPrompt { get; }
    string IlTitle { get; }
    string IlSubtitle { get; }
    string IlPreamble { get; }
    string IlWaitingForLowering { get; }
    string IlNoTextUnexpected { get; }
    /// <summary>Label for copying full IL disassembly text to the clipboard.</summary>
    string IlCopyDisassembly { get; }
    string WorldRuntimeTitle { get; }
    string WorldRuntimeSubtitle { get; }
    string FormatWorldRuntimePanel(
        bool? runtimeSucceeded,
        bool hasRunnableIl,
        string? lessonOutcome,
        int? lessonScore,
        string? worldAfterRunSummary,
        string? runtimeFaultMessage,
        string? runtimeStdout,
        string? runtimeStderr);
}