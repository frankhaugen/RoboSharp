using System.Text;
using Microsoft.Extensions.Logging;
using RoboSharp.Language;
using RoboSharp.Locales;
using RoboSharp.Runtime;
using RoboSharp.Semantics;
using RoboSharp.Toolchain;
using RoboSharp.World;

namespace RoboSharp.Application.Teaching;

public sealed class PipelineInspectionService : IPipelineInspectionService
{
    private const int MaxInterpreterSteps = 500_000;

    private readonly ILogger<PipelineInspectionService> _logger;
    private readonly ITeachingLocale _locale;

    public PipelineInspectionService(ILogger<PipelineInspectionService> logger, ITeachingLocale locale)
    {
        _logger = logger;
        _locale = locale;
    }

    public PipelineSnapshot InspectBuildOnly(string source, StudioPipelineOptions options)
    {
        try
        {
            var built = CompileThroughLowering(source, options.BuiltinProfile);
            var templateWorld = options.CreateRunWorld();
            var profileHelp = BuildProfileHelp(options);
            return FinalizeSnapshot(
                built,
                stdout: null,
                stderr: null,
                runtimeOk: null,
                fault: null,
                worldSummary: null,
                worldVis: templateWorld.CreateSnapshot(),
                options.ProfileLabel,
                options.WorldPresetLabel,
                profileHelp,
                lessonOutcome: null,
                lessonScore: null,
                ilSteps: null,
                ilFootnote: null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Pipeline inspection failed");
            throw;
        }
    }

    public async Task<PipelineSnapshot> InspectBuildAndRunAsync(
        string source,
        StudioRunSpeed speed,
        StudioPipelineOptions options,
        IProgress<StudioRunProgress>? runProgress,
        CancellationToken cancellationToken)
    {
        try
        {
            var built = CompileThroughLowering(source, options.BuiltinProfile);
            var templateWorld = options.CreateRunWorld();
            var profileHelp = BuildProfileHelp(options);

            if (!built.Compile.Succeeded || built.Compile.Program is null)
            {
                return FinalizeSnapshot(
                    built,
                    null,
                    null,
                    null,
                    null,
                    null,
                    templateWorld.CreateSnapshot(),
                    options.ProfileLabel,
                    options.WorldPresetLabel,
                    profileHelp,
                    null,
                    null,
                    null,
                    null);
            }

            var world = options.CreateRunWorld();
            using var swOut = new StringWriter();
            using var swErr = new StringWriter();

            string? stdout;
            string? stderr;
            bool? runtimeOk;
            string? fault;
            string? worldSummary;
            RobotWorldSnapshot worldVis;
            string? lessonOutcome = null;
            int? lessonScore = null;
            int? ilSteps = null;
            string? ilFootnote = null;

            try
            {
                var session = new RoboInterpreterSession();
                session.Start(built.Compile.Program, world, swOut, swErr);
                ReportProgress(runProgress, session, world);

                var delayMs = speed.StepDelayMilliseconds();
                RuntimeFault? stepFault = null;
                var completed = false;
                var steps = 0;

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (delayMs > 0)
                        await Task.Delay(delayMs, cancellationToken).ConfigureAwait(false);
                    else
                        await Task.Yield();

                    if (++steps > MaxInterpreterSteps)
                    {
                        stepFault = new RuntimeFault(_locale.Pipeline.InterpreterStepLimitFault, -1, -1);
                        break;
                    }

                    var step = session.Step();
                    ReportProgress(runProgress, session, world);

                    switch (step.Kind)
                    {
                        case InterpreterStepKind.Completed:
                            completed = true;
                            goto RunFinished;
                        case InterpreterStepKind.Faulted:
                            stepFault = step.Fault;
                            goto RunFinished;
                        case InterpreterStepKind.Advanced:
                            continue;
                        default:
                            stepFault = new RuntimeFault(_locale.Pipeline.InterpreterUnexpectedStepKind(step.Kind.ToString()), -1, -1);
                            goto RunFinished;
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();

            RunFinished:
                stdout = swOut.ToString();
                stderr = swErr.ToString();
                worldSummary = FormatWorldSummary(world);
                worldVis = world.CreateSnapshot();
                ilSteps = session.InstructionsExecuted;
                ilFootnote = _locale.Pipeline.IlTraceFootnote(
                    session.InstructionsExecuted,
                    session.CurrentInstructionDescription);

                var goal = LessonGoalEvaluator.Evaluate(world, session.InstructionsExecuted);
                lessonOutcome = goal.SummaryForKids;
                lessonScore = goal.Score;

                if (stepFault is not null)
                {
                    runtimeOk = false;
                    fault = stepFault.Message;
                    if (string.IsNullOrEmpty(stderr))
                        stderr = stepFault.Message;
                }
                else if (completed)
                {
                    runtimeOk = true;
                    fault = null;
                }
                else
                {
                    runtimeOk = false;
                    fault = _locale.Shell.InterpreterUnexpectedStop;
                }
            }
            catch (InvalidOperationException ex)
            {
                stdout = swOut.ToString();
                stderr = swErr.ToString();
                runtimeOk = false;
                fault = ex.Message;
                worldSummary = FormatWorldSummary(world);
                worldVis = world.CreateSnapshot();
            }

            return FinalizeSnapshot(
                built,
                stdout,
                stderr,
                runtimeOk,
                fault,
                worldSummary,
                worldVis,
                options.ProfileLabel,
                options.WorldPresetLabel,
                profileHelp,
                lessonOutcome,
                lessonScore,
                ilSteps,
                ilFootnote);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Pipeline run failed");
            throw;
        }
    }

    private static void ReportProgress(
        IProgress<StudioRunProgress>? runProgress,
        RoboInterpreterSession session,
        RobotWorld world)
    {
        if (runProgress is null)
            return;
        runProgress.Report(new StudioRunProgress(
            world.CreateSnapshot(),
            session.InstructionsExecuted,
            session.CurrentInstructionDescription,
            session.ProgressHighlightFunctionIndex,
            session.ProgressHighlightInstructionIndex));
    }

    private string BuildProfileHelp(StudioPipelineOptions options) =>
        _locale.Pipeline.BuildProfileHelp(
            options.ProfileLabel,
            options.WorldPresetLabel,
            LessonBuiltinProfiles.DescribeBuiltinsForHelp(options.BuiltinProfile));

    private BuiltPipeline CompileThroughLowering(string source, IBuiltinProfileProvider profile)
    {
        var text = SourceText.From(source);
        var tokens = Lexer.Tokenize(text);
        var syntaxTree = SyntaxTree.Parse(text);
        var parseDiagnostics = syntaxTree.Diagnostics;

        var compile = RoboSharpCompiler.Compile(source, profile);

        List<string> semanticLines;
        if (compile.SemanticModel is null)
        {
            semanticLines = [];
        }
        else
        {
            semanticLines = compile.SemanticModel.Diagnostics
                .Select(d =>
                    _locale.Pipeline.FormatSemanticDiagnosticLine(
                        d.Span.Start,
                        d.Span.Length,
                        SourceLocationFormatter.FormatLine(source, d.Span),
                        d.Message))
                .ToList();
        }

        string? boundText = null;
        if (compile.SemanticModel is not null)
        {
            try
            {
                boundText = BoundTreeTeachingFormatter.Format(compile.SemanticModel.Root);
            }
            catch (Exception ex)
            {
                boundText = _locale.Pipeline.BoundTreeFormatFailed(ex.Message);
                _logger.LogWarning(ex, "Bound tree formatting failed");
            }
        }

        string? ilText = compile.Program is not null ? IlTeachingFormatter.Format(compile.Program) : null;

        return new BuiltPipeline(source, tokens, syntaxTree, parseDiagnostics, compile, semanticLines, boundText, ilText);
    }

    private static PipelineSnapshot FinalizeSnapshot(
        BuiltPipeline built,
        string? stdout,
        string? stderr,
        bool? runtimeOk,
        string? fault,
        string? worldSummary,
        RobotWorldSnapshot? worldVis,
        string? lessonProfileLabel,
        string? worldPresetLabel,
        string? lessonProfileHelp,
        string? lessonOutcome,
        int? lessonScore,
        int? ilSteps,
        string? ilFootnote)
    {
        var spans = CollectDiagnosticSpans(built);
        return new(
            built.Source,
            built.Tokens,
            built.SyntaxTree,
            built.ParseDiagnostics,
            built.Compile.ReachedPhase,
            built.SemanticLines,
            built.BoundTreeText,
            built.IlDisassemblyText,
            stdout,
            stderr,
            runtimeOk,
            fault,
            worldSummary,
            worldVis,
            spans,
            lessonProfileLabel,
            worldPresetLabel,
            lessonProfileHelp,
            lessonOutcome,
            lessonScore,
            ilSteps,
            ilFootnote,
            built.Compile.Program);
    }

    private static IReadOnlyList<SourceDiagnosticSpan> CollectDiagnosticSpans(BuiltPipeline built)
    {
        var list = new List<SourceDiagnosticSpan>();
        foreach (var d in built.ParseDiagnostics)
        {
            if (d.Span.Length > 0)
                list.Add(new SourceDiagnosticSpan(d.Span.Start, d.Span.Length));
        }

        if (built.Compile.SemanticModel is not null)
        {
            foreach (var d in built.Compile.SemanticModel.Diagnostics)
            {
                if (d.Span.Length > 0)
                    list.Add(new SourceDiagnosticSpan(d.Span.Start, d.Span.Length));
            }
        }

        return list;
    }

    private string FormatWorldSummary(RobotWorld world)
    {
        var sb = new StringBuilder();
        sb.AppendLine(_locale.Pipeline.WorldGridLine(world.Terrain.Width, world.Terrain.Height, world.Metadata.Name));
        if (world.Metadata.PrimaryGoalPosition is { } g)
            sb.AppendLine(_locale.Pipeline.WorldGoalLine(g.X, g.Y));
        if (world.ActorsById.TryGetValue(1, out var actor))
        {
            sb.AppendLine(_locale.Pipeline.WorldRobotLine(actor.Position.X, actor.Position.Y, actor.Direction.ToString()));
        }
        else
        {
            sb.AppendLine(_locale.Pipeline.WorldNoPrimaryRobotLine);
        }

        return sb.ToString().TrimEnd();
    }

    private sealed record BuiltPipeline(
        string Source,
        IReadOnlyList<SyntaxToken> Tokens,
        SyntaxTree SyntaxTree,
        IReadOnlyList<ParseDiagnostic> ParseDiagnostics,
        CompileResult Compile,
        IReadOnlyList<string> SemanticLines,
        string? BoundTreeText,
        string? IlDisassemblyText);
}
