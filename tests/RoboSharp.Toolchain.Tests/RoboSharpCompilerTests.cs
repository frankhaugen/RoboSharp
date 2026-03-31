namespace RoboSharp.Toolchain.Tests;

public class RoboSharpCompilerTests
{
    [Test]
    public async Task Compile_ValidSource_Reaches_Lowered_With_Executable()
    {
        const string source = """
            move();
            """;

        var result = RoboSharpCompiler.Compile(source);

        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(result.ReachedPhase).IsEqualTo(CompilePhase.Lowered);
        await Assert.That(result.Executable).IsNotNull();
        await Assert.That(result.Program).IsNotNull();
    }

    [Test]
    public async Task Compile_Invalid_Lex_Parse_Stops_Before_Semantics()
    {
        var result = RoboSharpCompiler.Compile("### not robosharp");

        await Assert.That(result.Succeeded).IsFalse();
        await Assert.That(result.ReachedPhase).IsEqualTo(CompilePhase.Parse);
        await Assert.That(result.SyntaxTree).IsNotNull();
        await Assert.That(result.SyntaxTree!.Diagnostics.Count).IsGreaterThan(0);
    }

    [Test]
    public async Task Compile_Semantic_Error_Reaches_Semantics_With_Model_And_No_Executable()
    {
        const string source = """
            unknownBuiltin();
            """;

        var result = RoboSharpCompiler.Compile(source);

        await Assert.That(result.Succeeded).IsFalse();
        await Assert.That(result.ReachedPhase).IsEqualTo(CompilePhase.Semantics);
        await Assert.That(result.SemanticModel).IsNotNull();
        await Assert.That(result.SemanticModel!.Diagnostics.Count).IsGreaterThan(0);
        await Assert.That(result.Executable).IsNull();
        await Assert.That(result.Program).IsNull();
    }
}
