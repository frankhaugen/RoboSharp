using System.IO;
using RoboSharp.IL;
using RoboSharp.Runtime;
using RoboSharp.Semantics;
using RoboSharp.Toolchain;
using RoboSharp.World;

namespace RoboSharp.Integration.Tests;

public class FullPipelineCompileRunTests
{
    [Test]
    public async Task TopLevelStatements_UseTopLevelEntryFunction()
    {
        const string source = """
            move();
            """;

        var compiled = RoboSharpCompiler.Compile(source);
        await Assert.That(compiled.Succeeded).IsTrue();

        var program = compiled.Program!;
        var entry = program.Functions[program.EntryFunctionIndex];
        await Assert.That(entry.Name).IsEqualTo(CompilationArtifacts.TopLevelStatementsFunctionName);
    }

    [Test]
    public async Task UserMain_IsSemanticError()
    {
        const string source = """
            void main()
            {
                move();
            }
            """;

        var compiled = RoboSharpCompiler.Compile(source);
        await Assert.That(compiled.Succeeded).IsFalse();
        await Assert.That(compiled.ReachedPhase).IsEqualTo(CompilePhase.Semantics);
    }

    [Test]
    public async Task ProcedureWithoutLeadingVoid_CompilesAndRuns()
    {
        const string source = """
            MoveFive()
            {
                move();
                move();
                move();
                move();
                move();
            }

            MoveFive();
            """;

        var world = RobotWorldFactory.CreateBorderedEmpty(8, 4);
        using var stdout = new StringWriter();
        using var stderr = new StringWriter();

        var pipeline = RoboSharpPipeline.CompileAndRun(source, world, stdout, stderr);
        await Assert.That(pipeline.Succeeded).IsTrue();

        var actorId = world.Metadata.PrimaryActorId ?? 1;
        var robot = world.ActorsById[actorId];
        await Assert.That(robot.Position.X).IsEqualTo(6);
        await Assert.That(robot.Position.Y).IsEqualTo(1);
    }

    [Test]
    public async Task TopLevelWhile_PrintsExpectedSequence()
    {
        const string source = """
            integer i = 0;
            while (i < 3)
            {
                print(i);
                i = i + 1;
            }
            """;

        var world = RobotWorldFactory.CreateBorderedEmpty(4, 4);
        using var stdout = new StringWriter();
        using var stderr = new StringWriter();

        var pipeline = RoboSharpPipeline.CompileAndRun(source, world, stdout, stderr);
        await Assert.That(pipeline.Succeeded).IsTrue();
        var outText = stdout.ToString().ReplaceLineEndings("\n").Trim();
        await Assert.That(outText).IsEqualTo("0\n1\n2");
    }

    [Test]
    public async Task ParseFailure_StopsAtParsePhase()
    {
        const string source = """
            void main(
            """;

        var r = RoboSharpCompiler.Compile(source);
        await Assert.That(r.Succeeded).IsFalse();
        await Assert.That(r.ReachedPhase).IsEqualTo(CompilePhase.Parse);
        await Assert.That(r.SyntaxTree!.Diagnostics.Count).IsGreaterThan(0);
    }

    [Test]
    public async Task SemanticFailure_StopsAtSemanticsPhase()
    {
        const string source = """
            notARealBuiltin();
            """;

        var r = RoboSharpCompiler.Compile(source);
        await Assert.That(r.Succeeded).IsFalse();
        await Assert.That(r.ReachedPhase).IsEqualTo(CompilePhase.Semantics);
        await Assert.That(r.SemanticModel!.Diagnostics.Count).IsGreaterThan(0);
    }

    [Test]
    public async Task ReservedTopLevelName_IsSemanticError()
    {
        const string source = """
            void TopLevel()
            {
            }

            void main()
            {
            }
            """;

        var r = RoboSharpCompiler.Compile(source);
        await Assert.That(r.Succeeded).IsFalse();
        await Assert.That(r.ReachedPhase).IsEqualTo(CompilePhase.Semantics);
    }
}
