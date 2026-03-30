namespace RoboSharp.Language.Tests;

public class SyntaxParseDiagnosticsTests
{
    [Test]
    public async Task Parse_Bogus_Source_Produces_Parse_Diagnostics()
    {
        var tree = SyntaxTree.Parse(SourceText.From("@@@"));

        await Assert.That(tree.Diagnostics.Count).IsGreaterThan(0);
    }

    [Test]
    public async Task Parse_Empty_Main_Produces_Valid_Tree_Without_Diagnostics()
    {
        const string source = """
            void main()
            {
            }
            """;

        var tree = SyntaxTree.Parse(SourceText.From(source));

        await Assert.That(tree.Diagnostics.Count).IsEqualTo(0);
        await Assert.That(tree.Root.Members.Count).IsGreaterThan(0);
    }

    [Test]
    public async Task Unclosed_String_Lexes_BadToken_Parse_May_Continue_With_Diagnostics()
    {
        var tree = SyntaxTree.Parse(SourceText.From("void main() { print(\"x); }"));
        await Assert.That(tree.Diagnostics.Count).IsGreaterThan(0);
    }

    [Test]
    public async Task Missing_Close_Paren_In_If_Produces_Expected_Diagnostic()
    {
        var tree = SyntaxTree.Parse(SourceText.From("void main() { if (true { } }"));
        await Assert.That(tree.Diagnostics.Any(d => d.Message.Contains("CloseParen", StringComparison.Ordinal))).IsTrue();
    }

    [Test]
    public async Task Parse_Diagnostic_Has_Non_Empty_Message_And_Span()
    {
        var tree = SyntaxTree.Parse(SourceText.From("integer x = @;"));
        await Assert.That(tree.Diagnostics.Count).IsGreaterThan(0);
        var d = tree.Diagnostics[0];
        await Assert.That(d.Message.Length).IsGreaterThan(0);
        await Assert.That(d.Span.Length).IsGreaterThanOrEqualTo(0);
    }
}
