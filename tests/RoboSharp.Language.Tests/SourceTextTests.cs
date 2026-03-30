namespace RoboSharp.Language.Tests;

public class SourceTextTests
{
    private static string Slice(SourceText st, TextLine line) => st.Text.Substring(line.Start, line.Length);

    [Test]
    public async Task From_Preserves_Raw_Text()
    {
        const string raw = "void main() {\r\n}\n";
        var st = SourceText.From(raw);
        await Assert.That(st.Text).IsEqualTo(raw);
    }

    [Test]
    public async Task Lines_Split_On_Crlf_And_Lf_And_Lone_Cr()
    {
        var st = SourceText.From("a\nb\r\nc\rd");
        await Assert.That(st.Lines.Count).IsEqualTo(4);
        await Assert.That(Slice(st, st.Lines[0])).IsEqualTo("a\n");
        await Assert.That(Slice(st, st.Lines[1])).IsEqualTo("b\r\n");
        await Assert.That(Slice(st, st.Lines[2])).IsEqualTo("c\r");
        await Assert.That(Slice(st, st.Lines[3])).IsEqualTo("d");
    }

    [Test]
    public async Task Empty_String_Has_Single_Line()
    {
        var st = SourceText.From("");
        await Assert.That(st.Lines.Count).IsEqualTo(1);
        await Assert.That(Slice(st, st.Lines[0])).IsEqualTo("");
    }

    [Test]
    public async Task Trailing_Newline_Produces_Empty_Last_Line()
    {
        var st = SourceText.From("x\n");
        await Assert.That(st.Lines.Count).IsEqualTo(2);
        await Assert.That(Slice(st, st.Lines[0])).IsEqualTo("x\n");
        await Assert.That(Slice(st, st.Lines[1])).IsEqualTo("");
    }
}
