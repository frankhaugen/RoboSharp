using RoboSharp.Language;
using RoboSharp.Semantics;

namespace RoboSharp.IL.Tests;

public class IlLoweringTests
{
    [Test]
    public async Task Lower_PrintMain_EmitsCallBuiltin_And_Entry_Index()
    {
        const string source = """
            void main()
            {
                print(7);
            }
            """;

        var tree = SyntaxTree.Parse(SourceText.From(source));
        await Assert.That(tree.Diagnostics.Count).IsEqualTo(0);

        var model = new Binder(new FullBuiltinProfileProvider()).Bind(tree.Root);
        await Assert.That(model.Diagnostics.Count).IsEqualTo(0);
        await Assert.That(model.Root.EntryPoint).IsNotNull();

        var program = new IlLowerer().Lower(model.Root);

        await Assert.That(program.Functions.Count).IsGreaterThan(0);
        await Assert.That(program.EntryFunctionIndex).IsGreaterThanOrEqualTo(0);
        await Assert.That(program.EntryFunctionIndex).IsLessThan(program.Functions.Count);

        var mainFn = program.Functions[program.EntryFunctionIndex];
        await Assert.That(mainFn.Name).IsEqualTo("main");
        await Assert.That(mainFn.Instructions.Any(i => i.Op == RoboOpcode.CallBuiltin)).IsTrue();
    }
}
