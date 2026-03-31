using RoboSharp.IL;
using RoboSharp.Language;
using RoboSharp.Toolchain;
using RoboSharp.World;

namespace RoboSharp.Studio.Pipeline;

/// <summary>Lexer → parser → compile phases → optional interpreter run for Studio inspection panes.</summary>
public sealed record PipelineSnapshot(
    string Source,
    IReadOnlyList<SyntaxToken> Tokens,
    SyntaxTree SyntaxTree,
    IReadOnlyList<ParseDiagnostic> ParseDiagnostics,
    CompilePhase CompileReachedPhase,
    IReadOnlyList<string> SemanticDiagnosticLines,
    string? BoundTreeText,
    string? IlDisassemblyText,
    string? RuntimeStdout,
    string? RuntimeStderr,
    bool? RuntimeSucceeded,
    string? RuntimeFaultMessage,
    string? WorldAfterRunSummary,
    RobotWorldSnapshot? WorldVisualization,
    IReadOnlyList<SourceDiagnosticSpan> SourceDiagnosticSpans,
    string? LessonProfileLabel = null,
    string? WorldPresetLabel = null,
    string? LessonProfileHelpText = null,
    string? LessonOutcomeSummary = null,
    int? LessonScore = null,
    int? IlInstructionsExecuted = null,
    string? IlExecutionFootnote = null,
    RoboProgram? IlProgram = null);
