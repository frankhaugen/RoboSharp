namespace RoboSharp.Language.Tests;

/// <summary>Lexeme positions in source — useful for diagnostics and IDE features.</summary>
public class LexerTokenSpanTests
{
    [Test]
    public async Task Token_Spans_Are_Contiguous_For_Simple_Statement()
    {
        const string src = "void main();";
        var text = SourceText.From(src);
        var tokens = Lexer.Tokenize(text);
        for (var i = 0; i < tokens.Count - 1; i++)
        {
            var cur = tokens[i];
            var next = tokens[i + 1];
            if (cur.Kind == SyntaxKind.EndOfFileToken)
                break;
            await Assert.That(cur.Span.End).IsLessThanOrEqualTo(next.Span.Start);
        }
    }

    [Test]
    public async Task Identifier_Span_Covers_Full_Text()
    {
        var tok = Lexer.Tokenize(SourceText.From("  foo  "))[0];
        await Assert.That(tok.Kind).IsEqualTo(SyntaxKind.IdentifierToken);
        await Assert.That(tok.Span.Start).IsEqualTo(2);
        await Assert.That(tok.Span.Length).IsEqualTo(3);
    }

    [Test]
    public async Task String_Literal_Span_Includes_Quotes()
    {
        const string src = "\"ab\"";
        var tok = Lexer.Tokenize(SourceText.From(src))[0];
        await Assert.That(tok.Kind).IsEqualTo(SyntaxKind.StringLiteralToken);
        await Assert.That(tok.Span.Length).IsEqualTo(4);
    }

    [Test]
    public async Task Two_Char_Operator_Span_Length_Is_Two()
    {
        var tok = Lexer.Tokenize(SourceText.From("!="))[0];
        await Assert.That(tok.Kind).IsEqualTo(SyntaxKind.BangEqualsToken);
        await Assert.That(tok.Span.Length).IsEqualTo(2);
    }
}
