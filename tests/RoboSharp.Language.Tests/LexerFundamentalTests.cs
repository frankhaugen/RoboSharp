namespace RoboSharp.Language.Tests;

/// <summary>Usage-driven lexer coverage: tokens learners actually write in source.</summary>
public class LexerFundamentalTests
{
    private static IReadOnlyList<SyntaxToken> T(string source) => Lexer.Tokenize(SourceText.From(source));

    private static async Task AssertTokenKindsAsync(string source, params SyntaxKind[] expected)
    {
        var kinds = T(source).TakeWhile(t => t.Kind != SyntaxKind.EndOfFileToken).Select(t => t.Kind).ToArray();
        await Assert.That(kinds.Length).IsEqualTo(expected.Length);
        for (var i = 0; i < expected.Length; i++)
            await Assert.That(kinds[i]).IsEqualTo(expected[i]);
    }

    [Test]
    [Arguments("0", 0)]
    [Arguments("42", 42)]
    [Arguments("007", 7)]
    public async Task Integer_Literal_Value_Round_Trips(string text, int value)
    {
        var tok = T(text)[0];
        await Assert.That(tok.Kind).IsEqualTo(SyntaxKind.IntegerLiteralToken);
        await Assert.That(tok.Value).IsEqualTo(value);
    }

    [Test]
    public async Task Number_Literal_Zero_Point_Zero()
    {
        var tok = T("0.0")[0];
        await Assert.That(tok.Kind).IsEqualTo(SyntaxKind.NumberLiteralToken);
        await Assert.That(tok.Value).IsEqualTo(0.0);
    }

    [Test]
    public async Task Number_Literal_Pi_Like()
    {
        var tok = T("3.14")[0];
        await Assert.That(tok.Kind).IsEqualTo(SyntaxKind.NumberLiteralToken);
        await Assert.That(tok.Value).IsEqualTo(3.14);
    }

    [Test]
    public async Task Number_Literal_Trailing_Dot_Without_Fraction()
    {
        var tok = T("1.")[0];
        await Assert.That(tok.Kind).IsEqualTo(SyntaxKind.NumberLiteralToken);
        await Assert.That(tok.Value).IsEqualTo(1.0);
    }

    [Test]
    public async Task Adjacent_Operators_Tokenize_Separately()
    {
        var t = T("+-*/");
        await Assert.That(t[0].Kind).IsEqualTo(SyntaxKind.PlusToken);
        await Assert.That(t[1].Kind).IsEqualTo(SyntaxKind.MinusToken);
        await Assert.That(t[2].Kind).IsEqualTo(SyntaxKind.StarToken);
        await Assert.That(t[3].Kind).IsEqualTo(SyntaxKind.SlashToken);
    }

    [Test]
    [Arguments("==", SyntaxKind.EqualsEqualsToken)]
    [Arguments("!=", SyntaxKind.BangEqualsToken)]
    [Arguments("<=", SyntaxKind.LessOrEqualsToken)]
    [Arguments(">=", SyntaxKind.GreaterOrEqualsToken)]
    [Arguments("&&", SyntaxKind.AmpersandAmpersandToken)]
    [Arguments("||", SyntaxKind.PipePipeToken)]
    public async Task Two_Character_Operators_Are_Single_Tokens(string text, SyntaxKind kind)
    {
        var tok = T(text)[0];
        await Assert.That(tok.Kind).IsEqualTo(kind);
        await Assert.That(tok.Text).IsEqualTo(text);
    }

    [Test]
    public async Task Single_Char_Comparison_Operators_Distinct_From_Two_Char()
    {
        await Assert.That(T("<")[0].Kind).IsEqualTo(SyntaxKind.LessToken);
        await Assert.That(T(">")[0].Kind).IsEqualTo(SyntaxKind.GreaterToken);
        await Assert.That(T("!")[0].Kind).IsEqualTo(SyntaxKind.BangToken);
    }

    [Test]
    public async Task Identifier_Allows_Underscore_Start_And_Digits_Inside()
    {
        var tok = T("_a1")[0];
        await Assert.That(tok.Kind).IsEqualTo(SyntaxKind.IdentifierToken);
        await Assert.That(tok.Text).IsEqualTo("_a1");
    }

    [Test]
    public async Task Main_Is_Identifier_Not_Keyword()
    {
        var tok = T("main")[0];
        await Assert.That(tok.Kind).IsEqualTo(SyntaxKind.IdentifierToken);
    }

    [Test]
    public async Task Print_Is_Identifier_Not_Keyword()
    {
        var tok = T("print")[0];
        await Assert.That(tok.Kind).IsEqualTo(SyntaxKind.IdentifierToken);
    }

    [Test]
    public async Task Whitespace_And_Comments_Between_Tokens_Are_Leading_Trivia()
    {
        var tokens = T("  // skip\n\tvoid");
        var v = tokens[0];
        await Assert.That(v.Kind).IsEqualTo(SyntaxKind.VoidKeyword);
        await Assert.That(v.LeadingTrivia.Count).IsGreaterThanOrEqualTo(2);
        await Assert.That(v.LeadingTrivia.Any(x => x.Kind == SyntaxKind.CommentTrivia)).IsTrue();
        await Assert.That(v.LeadingTrivia.Any(x => x.Kind == SyntaxKind.EndOfLineTrivia)).IsTrue();
    }

    [Test]
    public async Task Block_Comment_Style_Line_Is_Single_Line_Comment_Only()
    {
        var tokens = T("// not { block }\nx");
        await Assert.That(tokens[0].Kind).IsEqualTo(SyntaxKind.IdentifierToken);
        await Assert.That(tokens[0].Text).IsEqualTo("x");
    }

    [Test]
    public async Task String_Empty_Is_Valid()
    {
        var tok = T("\"\"")[0];
        await Assert.That(tok.Kind).IsEqualTo(SyntaxKind.StringLiteralToken);
        await Assert.That(tok.Value).IsEqualTo("");
    }

    [Test]
    public async Task String_Unterminated_Is_BadToken_To_End()
    {
        var tok = T("\"hello")[0];
        await Assert.That(tok.Kind).IsEqualTo(SyntaxKind.BadToken);
        await Assert.That(tok.Text).IsEqualTo("\"hello");
    }

    [Test]
    public async Task String_Supports_Quote_And_Backslash_Escapes()
    {
        var tok = T("\"line\\n\"")[0];
        await Assert.That(tok.Kind).IsEqualTo(SyntaxKind.StringLiteralToken);
        await Assert.That(tok.Value).IsEqualTo("line\\n");
    }

    [Test]
    public async Task Typical_Print_Call_Token_Sequence()
    {
        await AssertTokenKindsAsync(
            "print(42);",
            SyntaxKind.IdentifierToken,
            SyntaxKind.OpenParenToken,
            SyntaxKind.IntegerLiteralToken,
            SyntaxKind.CloseParenToken,
            SyntaxKind.SemicolonToken);
    }

    [Test]
    public async Task Typical_Void_Main_Header_Token_Sequence()
    {
        await AssertTokenKindsAsync(
            "void main()",
            SyntaxKind.VoidKeyword,
            SyntaxKind.IdentifierToken,
            SyntaxKind.OpenParenToken,
            SyntaxKind.CloseParenToken);
    }

    [Test]
    public async Task Dot_Not_Start_Of_Number_Is_BadToken()
    {
        var t = T(".5");
        await Assert.That(t[0].Kind).IsEqualTo(SyntaxKind.BadToken);
    }

    [Test]
    public async Task Only_Comment_And_Whitespace_Yields_EndOf_File()
    {
        var tokens = T("// only\n   \n");
        await Assert.That(tokens[^1].Kind).IsEqualTo(SyntaxKind.EndOfFileToken);
        await Assert.That(tokens.Count).IsEqualTo(1);
    }

    [Test]
    public async Task Semicolon_Is_Own_Token_After_Close_Brace()
    {
        var t = T("};");
        await Assert.That(t[0].Kind).IsEqualTo(SyntaxKind.CloseBraceToken);
        await Assert.That(t[1].Kind).IsEqualTo(SyntaxKind.SemicolonToken);
    }

    [Test]
    public async Task Comma_Separates_Identifiers_In_Parameter_List_Text()
    {
        var kinds = T("integer a, integer b").TakeWhile(x => x.Kind != SyntaxKind.EndOfFileToken).Select(x => x.Kind).ToArray();
        await Assert.That(kinds.Any(k => k == SyntaxKind.CommaToken)).IsTrue();
    }
}
