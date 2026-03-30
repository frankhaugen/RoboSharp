namespace RoboSharp.Language.Tests;

public class SyntaxFactsTests
{
    [Test]
    [Arguments(SyntaxKind.IfKeyword, "if")]
    [Arguments(SyntaxKind.ElseKeyword, "else")]
    [Arguments(SyntaxKind.WhileKeyword, "while")]
    [Arguments(SyntaxKind.ReturnKeyword, "return")]
    [Arguments(SyntaxKind.IntegerKeyword, "integer")]
    [Arguments(SyntaxKind.NumberKeyword, "number")]
    [Arguments(SyntaxKind.StringKeyword, "string")]
    [Arguments(SyntaxKind.BoolKeyword, "bool")]
    [Arguments(SyntaxKind.VoidKeyword, "void")]
    [Arguments(SyntaxKind.TrueKeyword, "true")]
    [Arguments(SyntaxKind.FalseKeyword, "false")]
    [Arguments(SyntaxKind.PlusToken, "+")]
    [Arguments(SyntaxKind.EqualsEqualsToken, "==")]
    [Arguments(SyntaxKind.BangEqualsToken, "!=")]
    [Arguments(SyntaxKind.LessOrEqualsToken, "<=")]
    [Arguments(SyntaxKind.GreaterOrEqualsToken, ">=")]
    [Arguments(SyntaxKind.AmpersandAmpersandToken, "&&")]
    [Arguments(SyntaxKind.PipePipeToken, "||")]
    public async Task GetText_Returns_Spelling_For_Keyword_And_Punctuation(SyntaxKind kind, string expected)
    {
        await Assert.That(SyntaxFacts.GetText(kind)).IsEqualTo(expected);
    }

    [Test]
    public async Task GetText_Returns_Null_For_Trivia_And_Literals()
    {
        await Assert.That(SyntaxFacts.GetText(SyntaxKind.WhitespaceTrivia)).IsNull();
        await Assert.That(SyntaxFacts.GetText(SyntaxKind.IdentifierToken)).IsNull();
        await Assert.That(SyntaxFacts.GetText(SyntaxKind.IntegerLiteralToken)).IsNull();
        await Assert.That(SyntaxFacts.GetText(SyntaxKind.EndOfFileToken)).IsNull();
    }

    [Test]
    [Arguments("if", SyntaxKind.IfKeyword)]
    [Arguments("void", SyntaxKind.VoidKeyword)]
    [Arguments("integer", SyntaxKind.IntegerKeyword)]
    public async Task GetKeywordKind_Is_Case_Sensitive_And_Exact(string text, SyntaxKind expected)
    {
        await Assert.That(SyntaxFacts.GetKeywordKind(text)).IsEqualTo(expected);
    }

    [Test]
    [Arguments("IF")]
    [Arguments("Void")]
    [Arguments("main")]
    public async Task GetKeywordKind_Returns_Null_For_Non_Keyword_Spellings(string text)
    {
        await Assert.That(SyntaxFacts.GetKeywordKind(text)).IsNull();
    }

    [Test]
    public async Task IsKeywordKind_And_IsTypeKeyword_Classify_Kinds()
    {
        await Assert.That(SyntaxFacts.IsKeywordKind(SyntaxKind.IfKeyword)).IsTrue();
        await Assert.That(SyntaxFacts.IsKeywordKind(SyntaxKind.IdentifierToken)).IsFalse();

        await Assert.That(SyntaxFacts.IsTypeKeyword(SyntaxKind.VoidKeyword)).IsTrue();
        await Assert.That(SyntaxFacts.IsTypeKeyword(SyntaxKind.IntegerKeyword)).IsTrue();
        await Assert.That(SyntaxFacts.IsTypeKeyword(SyntaxKind.IfKeyword)).IsFalse();
    }

    [Test]
    public async Task Binary_Precedence_Multiplication_Stricter_Than_Addition()
    {
        await Assert.That(SyntaxFacts.GetBinaryOperatorPrecedence(SyntaxKind.StarToken))
            .IsGreaterThan(SyntaxFacts.GetBinaryOperatorPrecedence(SyntaxKind.PlusToken));
    }

    [Test]
    public async Task Binary_Precedence_Comparison_Stricter_Than_Logical_And()
    {
        await Assert.That(SyntaxFacts.GetBinaryOperatorPrecedence(SyntaxKind.EqualsEqualsToken))
            .IsGreaterThan(SyntaxFacts.GetBinaryOperatorPrecedence(SyntaxKind.AmpersandAmpersandToken));
    }

    [Test]
    public async Task Binary_Precedence_Logical_And_Stricter_Than_Logical_Or()
    {
        await Assert.That(SyntaxFacts.GetBinaryOperatorPrecedence(SyntaxKind.AmpersandAmpersandToken))
            .IsGreaterThan(SyntaxFacts.GetBinaryOperatorPrecedence(SyntaxKind.PipePipeToken));
    }

    [Test]
    public async Task Unary_Precedence_Is_Positive_For_Prefix_Operators()
    {
        await Assert.That(SyntaxFacts.GetUnaryOperatorPrecedence(SyntaxKind.MinusToken)).IsGreaterThan(0);
        await Assert.That(SyntaxFacts.GetUnaryOperatorPrecedence(SyntaxKind.BangToken)).IsGreaterThan(0);
        await Assert.That(SyntaxFacts.GetUnaryOperatorPrecedence(SyntaxKind.PlusToken)).IsGreaterThan(0);
    }

    [Test]
    public async Task Unary_Precedence_Is_Zero_For_Non_Unary_Tokens()
    {
        await Assert.That(SyntaxFacts.GetUnaryOperatorPrecedence(SyntaxKind.StarToken)).IsEqualTo(0);
    }
}
