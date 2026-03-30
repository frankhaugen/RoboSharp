using RoboSharp.Language;

namespace RoboSharp.Language.Tests;

public class LexerTests
{
    [Test]
    public async Task Keyword_If_Is_Classified_As_IfKeyword_Not_Identifier()
    {
        var tokens = Lexer.Tokenize(SourceText.From("if"));
        await Assert.That(tokens).HasCount(2);
        await Assert.That(tokens[0].Kind).IsEqualTo(SyntaxKind.IfKeyword);
        await Assert.That(tokens[0].Text).IsEqualTo("if");
        await Assert.That(tokens[1].Kind).IsEqualTo(SyntaxKind.EndOfFileToken);
    }

    [Test]
    public async Task Identifier_That_Prefixes_Keyword_Is_IdentifierToken()
    {
        var tokens = Lexer.Tokenize(SourceText.From("iffy"));
        await Assert.That(tokens[0].Kind).IsEqualTo(SyntaxKind.IdentifierToken);
        await Assert.That(tokens[0].Text).IsEqualTo("iffy");
    }

    [Test]
    public async Task True_And_False_Are_Keyword_Tokens()
    {
        var t = Lexer.Tokenize(SourceText.From("true false"));
        await Assert.That(t[0].Kind).IsEqualTo(SyntaxKind.TrueKeyword);
        await Assert.That(t[1].Kind).IsEqualTo(SyntaxKind.FalseKeyword);
    }

    [Test]
    public async Task IntegerLiteral_Parses_Value_As_Int()
    {
        var tokens = Lexer.Tokenize(SourceText.From("  42 "));
        await Assert.That(tokens[0].Kind).IsEqualTo(SyntaxKind.IntegerLiteralToken);
        await Assert.That(tokens[0].Value).IsEqualTo(42);
        await Assert.That(tokens[0].Text).IsEqualTo("42");
    }

    [Test]
    public async Task StringLiteral_Supports_Minimal_Escapes()
    {
        var tokens = Lexer.Tokenize(SourceText.From("\"a\\\"b\\\\c\""));
        await Assert.That(tokens[0].Kind).IsEqualTo(SyntaxKind.StringLiteralToken);
        await Assert.That(tokens[0].Value).IsEqualTo("a\"b\\c");
    }

    [Test]
    public async Task Line_Comment_Is_Leading_Trivia_Of_Following_Token()
    {
        var tokens = Lexer.Tokenize(SourceText.From("// hello\nx"));
        await Assert.That(tokens[0].Kind).IsEqualTo(SyntaxKind.IdentifierToken);
        await Assert.That(tokens[0].Text).IsEqualTo("x");
        await Assert.That(tokens[0].LeadingTrivia).HasCount(2);
        await Assert.That(tokens[0].LeadingTrivia[0].Kind).IsEqualTo(SyntaxKind.CommentTrivia);
        await Assert.That(tokens[0].LeadingTrivia[0].Text).IsEqualTo("// hello");
        await Assert.That(tokens[0].LeadingTrivia[1].Kind).IsEqualTo(SyntaxKind.EndOfLineTrivia);
    }

    [Test]
    public async Task EqualsEquals_Is_Single_Token_Distinct_From_Equals()
    {
        var eqEq = Lexer.Tokenize(SourceText.From("=="));
        await Assert.That(eqEq[0].Kind).IsEqualTo(SyntaxKind.EqualsEqualsToken);
        await Assert.That(eqEq[0].Text).IsEqualTo("==");

        var eq = Lexer.Tokenize(SourceText.From("="));
        await Assert.That(eq[0].Kind).IsEqualTo(SyntaxKind.EqualsToken);
        await Assert.That(eq[0].Text).IsEqualTo("=");
    }

    [Test]
    public async Task BadToken_Single_Char_Then_Recovers_To_Next_Token()
    {
        var tokens = Lexer.Tokenize(SourceText.From("@a"));
        await Assert.That(tokens[0].Kind).IsEqualTo(SyntaxKind.BadToken);
        await Assert.That(tokens[0].Text).IsEqualTo("@");
        await Assert.That(tokens[1].Kind).IsEqualTo(SyntaxKind.IdentifierToken);
        await Assert.That(tokens[1].Text).IsEqualTo("a");
    }

    [Test]
    public async Task EndOfFileToken_Has_Zero_Width_At_Text_End()
    {
        var text = SourceText.From("a");
        var tokens = Lexer.Tokenize(text);
        var eof = tokens[^1];
        await Assert.That(eof.Kind).IsEqualTo(SyntaxKind.EndOfFileToken);
        await Assert.That(eof.Span.Start).IsEqualTo(text.Text.Length);
        await Assert.That(eof.Span.Length).IsEqualTo(0);
    }

    [Test]
    public async Task Void_Is_VoidKeyword()
    {
        var tokens = Lexer.Tokenize(SourceText.From("void"));
        await Assert.That(tokens[0].Kind).IsEqualTo(SyntaxKind.VoidKeyword);
        await Assert.That(tokens[0].Text).IsEqualTo("void");
    }
}
