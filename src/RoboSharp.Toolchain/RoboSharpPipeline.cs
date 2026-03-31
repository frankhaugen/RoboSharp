using System.Text;

using RoboSharp.Runtime;
using RoboSharp.World;

namespace RoboSharp.Toolchain;

/// <summary>Parses, binds, lowers, and runs a single compilation unit against a world (teaching pipeline entry).</summary>
public static class RoboSharpPipeline
{
    public static PipelineResult CompileAndRun(
        string source,
        RobotWorld world,
        TextWriter? stdout = null,
        TextWriter? stderr = null)
    {
        stdout ??= TextWriter.Null;
        stderr ??= TextWriter.Null;

        var compiled = RoboSharpCompiler.Compile(source);
        if (!compiled.Succeeded)
        {
            return new PipelineResult
            {
                SyntaxTree = compiled.SyntaxTree,
                SemanticModel = compiled.SemanticModel,
                FailureStage = compiled.ReachedPhase == CompilePhase.Parse ? PipelineStage.Parse : PipelineStage.Semantics,
            };
        }

        var program = compiled.Program!;
        var interpreter = new RoboInterpreter();
        var exec = interpreter.Run(program, world, stdout, stderr);

        return new PipelineResult
        {
            SyntaxTree = compiled.SyntaxTree,
            SemanticModel = compiled.SemanticModel,
            Program = program,
            Executable = compiled.Executable,
            Execution = exec,
            FailureStage = exec.Succeeded ? null : PipelineStage.Runtime,
        };
    }

    public static string FormatDiagnostics(PipelineResult result)
    {
        var sb = new StringBuilder();
        if (result.SyntaxTree is not null)
        {
            foreach (var d in result.SyntaxTree.Diagnostics)
                sb.AppendLine($"parse: {d.Message}");
        }

        if (result.SemanticModel is not null)
        {
            foreach (var d in result.SemanticModel.Diagnostics)
                sb.AppendLine($"semantic: {d.Message}");
        }

        if (result.Execution?.Fault is { } f)
            sb.AppendLine($"runtime: {f.Message}");

        return sb.ToString().TrimEnd();
    }
}