using System.IO;
using RoboSharp.Toolchain;
using RoboSharp.World;

namespace RoboSharp.Toolchain.Tests;

public class PipelineDiagnosticsTests
{
    [Test]
    public async Task FormatDiagnostics_Includes_Parse_Lines_When_Parse_Fails()
    {
        const string source = "void main(  ";
        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        var result = RoboSharpPipeline.CompileAndRun(source, world, TextWriter.Null, TextWriter.Null);

        await Assert.That(result.FailureStage).IsEqualTo(PipelineStage.Parse);
        var text = RoboSharpPipeline.FormatDiagnostics(result);
        await Assert.That(text).Contains("parse:");
    }

    [Test]
    public async Task FormatDiagnostics_Includes_Semantic_Lines_When_Binding_Fails()
    {
        const string source = """
            notARealSymbol();
            """;

        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        var result = RoboSharpPipeline.CompileAndRun(source, world, TextWriter.Null, TextWriter.Null);

        await Assert.That(result.FailureStage).IsEqualTo(PipelineStage.Semantics);
        var text = RoboSharpPipeline.FormatDiagnostics(result);
        await Assert.That(text).Contains("semantic:");
    }

    [Test]
    public async Task FormatDiagnostics_Includes_Runtime_Line_When_Execution_Faults()
    {
        const string source = """
            integer z = 0;
            print(1 / z);
            """;

        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        var result = RoboSharpPipeline.CompileAndRun(source, world, TextWriter.Null, TextWriter.Null);

        await Assert.That(result.FailureStage).IsEqualTo(PipelineStage.Runtime);
        await Assert.That(result.Execution?.Succeeded).IsFalse();
        var text = RoboSharpPipeline.FormatDiagnostics(result);
        await Assert.That(text).Contains("runtime:");
        await Assert.That(text).Contains("zero", StringComparison.OrdinalIgnoreCase);
    }
}
