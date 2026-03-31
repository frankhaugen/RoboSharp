using RoboSharp.Language;
using RoboSharp.Semantics;

namespace RoboSharp.Semantics.Tests;

public class BinderBindingTests
{
    [Test]
    public async Task Bind_UserMain_Reports_Error_And_No_EntryPoint()
    {
        const string source = """
            void main()
            {
                print(42);
            }
            """;

        var tree = SyntaxTree.Parse(SourceText.From(source));
        await Assert.That(tree.Diagnostics.Count).IsEqualTo(0);

        var model = new Binder(new FullBuiltinProfileProvider()).Bind(tree.Root);

        await Assert.That(model.Diagnostics.Any(d => d.Message.Contains("main", StringComparison.Ordinal))).IsTrue();
        await Assert.That(model.Root.EntryPoint).IsNull();
    }

    [Test]
    public async Task Bind_MissingMain_Reports_Error_And_No_EntryPoint()
    {
        const string source = """
            void other()
            {
            }
            """;

        var tree = SyntaxTree.Parse(SourceText.From(source));
        await Assert.That(tree.Diagnostics.Count).IsEqualTo(0);

        var model = new Binder(new FullBuiltinProfileProvider()).Bind(tree.Root);

        await Assert.That(model.Diagnostics.Count).IsGreaterThan(0);
        await Assert.That(model.Root.EntryPoint).IsNull();
        await Assert.That(model.Diagnostics.Any(d => d.Message.Contains("top-level", StringComparison.Ordinal))).IsTrue();
    }

    [Test]
    public async Task Bind_TopLevelStatements_UseSyntheticEntryNamedMainInternal()
    {
        const string source = """
            move();
            """;

        var tree = SyntaxTree.Parse(SourceText.From(source));
        await Assert.That(tree.Diagnostics.Count).IsEqualTo(0);

        var model = new Binder(new FullBuiltinProfileProvider()).Bind(tree.Root);

        await Assert.That(model.Diagnostics.Count).IsEqualTo(0);
        await Assert.That(model.Root.EntryPoint).IsNotNull();
        await Assert.That(model.Root.EntryPoint!.Name).IsEqualTo(CompilationArtifacts.TopLevelStatementsFunctionName);
        await Assert.That(model.Root.Functions.Count).IsEqualTo(1);
    }

    [Test]
    public async Task Bind_TopLevelAndUserFunction_EntryFirst()
    {
        const string source = """
            integer bump(integer x)
            {
                return x + 1;
            }

            print(bump(1));
            """;

        var tree = SyntaxTree.Parse(SourceText.From(source));
        await Assert.That(tree.Diagnostics.Count).IsEqualTo(0);

        var model = new Binder(new FullBuiltinProfileProvider()).Bind(tree.Root);

        await Assert.That(model.Diagnostics.Count).IsEqualTo(0);
        await Assert.That(model.Root.Functions[0].Symbol.Name).IsEqualTo(CompilationArtifacts.TopLevelStatementsFunctionName);
        await Assert.That(model.Root.Functions[1].Symbol.Name).IsEqualTo("bump");
    }

    [Test]
    public async Task Bind_TopLevelWithVoidMain_ReportsReservedMain_And_EntryStillTopLevel()
    {
        const string source = """
            void main()
            {
            }

            move();
            """;

        var tree = SyntaxTree.Parse(SourceText.From(source));
        await Assert.That(tree.Diagnostics.Count).IsEqualTo(0);

        var model = new Binder(new FullBuiltinProfileProvider()).Bind(tree.Root);

        await Assert.That(model.Diagnostics.Any(d => d.Message.Contains("main", StringComparison.Ordinal))).IsTrue();
        await Assert.That(model.Root.EntryPoint).IsNotNull();
        await Assert.That(model.Root.EntryPoint!.Name).IsEqualTo(CompilationArtifacts.TopLevelStatementsFunctionName);
    }

    [Test]
    public async Task Bind_Duplicate_Function_Reports_Error()
    {
        const string source = """
            void dup() { }
            void dup() { }
            """;

        var tree = SyntaxTree.Parse(SourceText.From(source));
        await Assert.That(tree.Diagnostics.Count).IsEqualTo(0);

        var model = new Binder(new FullBuiltinProfileProvider()).Bind(tree.Root);

        await Assert.That(model.Diagnostics.Any(d => d.Message.Contains("Duplicate function", StringComparison.Ordinal))).IsTrue();
    }
}
