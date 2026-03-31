namespace RoboSharp.Locales;

public interface IStudioPanelTexts
{
    string LessonToolboxTitle { get; }
    string LessonToolboxSubtitle { get; }
    string LessonToolboxLead { get; }
    string LessonToolboxGuide { get; }
    string LessonToolboxFooter { get; }
    string LessonToolboxBuildPrompt { get; }

    string TokensTitle { get; }
    string TokensSubtitle { get; }
    string TokensLead { get; }
    string TokensGuide { get; }
    string TokensColumnHeader { get; }
    string TokensFootnote { get; }

    string SyntaxTreeTitle { get; }
    string SyntaxTreeSubtitle { get; }
    string SyntaxTreeLead { get; }
    string SyntaxTreeGuide { get; }
    string SyntaxTreeFootnote { get; }

    string DiagnosticsTitle { get; }
    string DiagnosticsSubtitle { get; }
    string DiagnosticsPreamble { get; }
    string DiagnosticsNone { get; }
    string DiagnosticsRuntimePrefix { get; }
    string BoundTreeTitle { get; }
    string BoundTreeSubtitle { get; }
    string BoundTreeLead { get; }
    string BoundTreeGuide { get; }
    string BoundTreeFootnote { get; }
    string BoundTreeNeedParseFirst { get; }
    string BoundTreeSemanticsStopped { get; }
    string BoundTreeUnexpectedEmpty { get; }
    string BoundTreeBuildPrompt { get; }

    string IlTitle { get; }
    string IlSubtitle { get; }
    string IlLead { get; }
    string IlGuide { get; }
    string IlWaitingForLowering { get; }
    string IlNoTextUnexpected { get; }
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
