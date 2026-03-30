using System.Text;
using Microsoft.Extensions.Logging;
using RoboSharp.Language;
using RoboSharp.Runtime;
using RoboSharp.Semantics;
using RoboSharp.Toolchain;
using RoboSharp.World;

namespace RoboSharp.Studio.Pipeline;

public sealed class PipelineInspectionService : IPipelineInspectionService
{
    private const int MaxInterpreterSteps = 500_000;

    private readonly ILogger<PipelineInspectionService> _logger;

    public PipelineInspectionService(ILogger<PipelineInspectionService> logger) =>
        _logger = logger;

    public PipelineSnapshot InspectBuildOnly(string source)
    {
        try
        {
            var built = CompileThroughLowering(source);
            var templateWorld = RobotWorldFactory.CreateBorderedEmpty(16, 16);
            return FinalizeSnapshot(
                built,
                stdout: null,
                stderr: null,
                runtimeOk: null,
                fault: null,
                worldSummary: null,
                worldVis: templateWorld.CreateSnapshot());
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
        IProgress<RobotWorldSnapshot>? worldProgress,
        CancellationToken cancellationToken)
    {
        try
        {
            var built = CompileThroughLowering(source);
            var templateWorld = RobotWorldFactory.CreateBorderedEmpty(16, 16);

            if (!built.Compile.Succeeded || built.Compile.Program is null)
            {
                return FinalizeSnapshot(
                    built,
                    null,
                    null,
                    null,
                    null,
                    null,
                    templateWorld.CreateSnapshot());
            }

            var world = RobotWorldFactory.CreateBorderedEmpty(16, 16);
            using var swOut = new StringWriter();
            using var swErr = new StringWriter();

            string? stdout;
            string? stderr;
            bool? runtimeOk;
            string? fault;
            string? worldSummary;
            RobotWorldSnapshot worldVis;

            try
            {
                var session = new RoboInterpreterSession();
                session.Start(built.Compile.Program, world, swOut, swErr);
                worldProgress?.Report(world.CreateSnapshot());

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
                        stepFault = new RuntimeFault("Interpreter step limit exceeded (safety cap).", -1, -1);
                        break;
                    }

                    var step = session.Step();
                    worldProgress?.Report(world.CreateSnapshot());

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
                            stepFault = new RuntimeFault($"Unexpected step outcome: {step.Kind}.", -1, -1);
                            goto RunFinished;
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();

            RunFinished:
                stdout = swOut.ToString();
                stderr = swErr.ToString();
                worldSummary = FormatWorldSummary(world);
                worldVis = world.CreateSnapshot();

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
                    fault = "Interpreter stopped without completion or fault (unexpected).";
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

            return FinalizeSnapshot(built, stdout, stderr, runtimeOk, fault, worldSummary, worldVis);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Pipeline run failed");
            throw;
        }
    }

    private BuiltPipeline CompileThroughLowering(string source)
    {
        var text = SourceText.From(source);
        var tokens = Lexer.Tokenize(text);
        var syntaxTree = SyntaxTree.Parse(text);
        var parseDiagnostics = syntaxTree.Diagnostics;

        var compile = RoboSharpCompiler.Compile(source);

        List<string> semanticLines;
        if (compile.SemanticModel is null)
        {
            semanticLines = [];
        }
        else
        {
            semanticLines = compile.SemanticModel.Diagnostics
                .Select(d => $"@{d.Span.Start}:{d.Span.Length}  {d.Message}")
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
                boundText = $"(Could not format bound tree: {ex.Message})";
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
        RobotWorldSnapshot? worldVis) =>
        new(
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
            worldVis);

    private static string FormatWorldSummary(RobotWorld world)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{world.Terrain.Width}×{world.Terrain.Height} bordered grid");
        if (world.ActorsById.TryGetValue(1, out var actor))
        {
            sb.AppendLine($"Primary robot (id {actor.Id}): tile ({actor.Position.X}, {actor.Position.Y}), facing {actor.Direction}");
        }
        else
        {
            sb.AppendLine("No actor id 1 in world.");
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
