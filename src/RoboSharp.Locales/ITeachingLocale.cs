namespace RoboSharp.Locales;

/// <summary>
/// Culture-specific teaching copy for RoboSharp hosts. Implementations use hardcoded strings in C# — no .resx —
/// so the text stays versioned, grep-friendly, and easy to inspect in class.
/// </summary>
public interface ITeachingLocale
{
    /// <summary>BCP 47-style tag, e.g. <c>en</c>.</summary>
    string LocaleId { get; }

    IStudioShellTexts Shell { get; }
    IStudioSidebarTexts Sidebar { get; }
    IStudioPanelTexts Panels { get; }
    IPipelineTeachingTexts Pipeline { get; }
}

public interface IStudioShellTexts
{
    string WindowTitleSuffix { get; }
    string FormatWindowTitle(string fileDisplayName, bool dirty);
    string UntitledFileName { get; }
    string FileMenuHeader { get; }
    string MenuNew { get; }
    string MenuOpen { get; }
    string MenuSave { get; }
    string MenuSaveAs { get; }
    string MenuExit { get; }
    string HelpMenuHeader { get; }
    string MenuAbout { get; }
    string RoboFileTypeDescription { get; }
    string OpenFilePickerTitle { get; }
    string SaveFilePickerTitle { get; }
    string DialogOk { get; }
    string SaveFailedTitle { get; }
    string OpenFailedTitle { get; }
    string SaveNoLocalPathMessage { get; }
    string UnsavedDialogTitle { get; }
    string UnsavedDialogHeading { get; }
    string UnsavedDialogBody { get; }
    string ButtonSave { get; }
    string ButtonDontSave { get; }
    string ButtonCancel { get; }
    string AboutTitle { get; }
    string AboutAppName { get; }
    string AboutBody { get; }
    string ToolbarBuild { get; }
    string ToolbarRun { get; }
    string ToolbarStepSpeed { get; }
    string ToolbarAppTitle { get; }
    string ToolbarSubtitle { get; }
    string RunSpeedRealtime { get; }
    string RunSpeedSlow { get; }
    string RunSpeedGlacial { get; }
    string DefaultLiveRunStatus { get; }
    string LiveRunInProgress { get; }
    string LiveRunFinished { get; }
    string LiveRunFaultFallback { get; }
    string FormatLiveRunProgress(long steps, string? instructionHint);
    string FormatLessonOutcomeLine(string story, int? score);
    string InterpreterUnexpectedStop { get; }
}

public interface IStudioSidebarTexts
{
    string LessonAndMapHeading { get; }
    string ProfileCaption { get; }
    string WorldCaption { get; }
    string KarelWorldHeading { get; }
    string KarelWorldHint { get; }
}

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

public interface IPipelineTeachingTexts
{
    string FormatParseDiagnosticLine(int start, int length, string location, string message);
    string FormatSemanticDiagnosticLine(int start, int length, string location, string message);
    string BoundTreeFormatFailed(string exceptionMessage);
    string BuildProfileHelp(string profileLabel, string worldLabel, string builtinsBody);
    string WorldGridLine(int width, int height, string metadataName);
    string WorldGoalLine(int x, int y);
    string WorldRobotLine(int x, int y, string directionDisplay);
    string WorldNoPrimaryRobotLine { get; }
    string InterpreterStepLimitFault { get; }
    string InterpreterUnexpectedStepKind(string kindName);
    string IlTraceFootnote(int instructionsExecuted, string? lastInstructionDescription);
    string ProfileHelpYouCanCall { get; }
}
