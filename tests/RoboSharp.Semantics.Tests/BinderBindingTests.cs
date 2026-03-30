using RoboSharp.Language;

namespace RoboSharp.Semantics.Tests;

public class BinderBindingTests
{
    [Test]
    public async Task Bind_ValidMain_ProducesEntryPoint_And_No_Diagnostics()
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

        await Assert.That(model.Diagnostics.Count).IsEqualTo(0);
        await Assert.That(model.Root.EntryPoint).IsNotNull();
        await Assert.That(model.Root.EntryPoint!.Name).IsEqualTo("main");
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
        await Assert.That(model.Diagnostics.Any(d => d.Message.Contains("main", StringComparison.Ordinal))).IsTrue();
    }

    [Test]
    public async Task Bind_Duplicate_Function_Reports_Error()
    {
        const string source = """
            void dup() { }
            void dup() { }
            void main()
            {
            }
            """;

        var tree = SyntaxTree.Parse(SourceText.From(source));
        await Assert.That(tree.Diagnostics.Count).IsEqualTo(0);

        var model = new Binder(new FullBuiltinProfileProvider()).Bind(tree.Root);

        await Assert.That(model.Diagnostics.Any(d => d.Message.Contains("Duplicate function", StringComparison.Ordinal))).IsTrue();
    }
}
