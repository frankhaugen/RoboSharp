namespace RoboSharp.Language;

/// <summary>
/// Language facts for keywords, operator text, and precedence. Higher precedence values bind tighter.
/// </summary>
public static class SyntaxFacts
{
    /// <summary>
    /// Returns the fixed spelling for a keyword or punctuation kind, or <see langword="null"/> when the kind has no single fixed text
    /// (trivia, literal tokens, identifiers, nodes, <see cref="SyntaxKind.BadToken"/>, <see cref="SyntaxKind.EndOfFileToken"/>).
    /// </summary>
    public static string? GetText(SyntaxKind kind) =>
        kind switch
        {
            // Keywords (v1 spellings per docs/language/syntax.md)
            SyntaxKind.IfKeyword => "if",
            SyntaxKind.ElseKeyword => "else",
            SyntaxKind.WhileKeyword => "while",
            SyntaxKind.ReturnKeyword => "return",
            SyntaxKind.IntegerKeyword => "integer",
            SyntaxKind.NumberKeyword => "number",
            SyntaxKind.StringKeyword => "string",
            SyntaxKind.BoolKeyword => "bool",
            SyntaxKind.TrueKeyword => "true",
            SyntaxKind.FalseKeyword => "false",

            // Operators / punctuation
            SyntaxKind.PlusToken => "+",
            SyntaxKind.MinusToken => "-",
            SyntaxKind.StarToken => "*",
            SyntaxKind.SlashToken => "/",
            SyntaxKind.BangToken => "!",
            SyntaxKind.EqualsToken => "=",
            SyntaxKind.EqualsEqualsToken => "==",
            SyntaxKind.BangEqualsToken => "!=",
            SyntaxKind.LessToken => "<",
            SyntaxKind.LessOrEqualsToken => "<=",
            SyntaxKind.GreaterToken => ">",
            SyntaxKind.GreaterOrEqualsToken => ">=",
            SyntaxKind.AmpersandAmpersandToken => "&&",
            SyntaxKind.PipePipeToken => "||",
            SyntaxKind.OpenParenToken => "(",
            SyntaxKind.CloseParenToken => ")",
            SyntaxKind.OpenBraceToken => "{",
            SyntaxKind.CloseBraceToken => "}",
            SyntaxKind.OpenBracketToken => "[",
            SyntaxKind.CloseBracketToken => "]",
            SyntaxKind.CommaToken => ",",
            SyntaxKind.SemicolonToken => ";",

            _ => null,
        };

    /// <summary>
    /// Maps a keyword string to its <see cref="SyntaxKind"/> using ordinal (case-sensitive) comparison.
    /// Returns <see langword="null"/> when <paramref name="text"/> is not a keyword spelling.
    /// </summary>
    public static SyntaxKind? GetKeywordKind(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        return text switch
        {
            "if" => SyntaxKind.IfKeyword,
            "else" => SyntaxKind.ElseKeyword,
            "while" => SyntaxKind.WhileKeyword,
            "return" => SyntaxKind.ReturnKeyword,
            "integer" => SyntaxKind.IntegerKeyword,
            "number" => SyntaxKind.NumberKeyword,
            "string" => SyntaxKind.StringKeyword,
            "bool" => SyntaxKind.BoolKeyword,
            "true" => SyntaxKind.TrueKeyword,
            "false" => SyntaxKind.FalseKeyword,
            _ => null,
        };
    }

    /// <summary>
    /// Returns whether <paramref name="kind"/> is one of the keyword kinds (<see cref="SyntaxKind.IfKeyword"/> through <see cref="SyntaxKind.FalseKeyword"/>).
    /// </summary>
    public static bool IsKeywordKind(SyntaxKind kind) =>
        kind is >= SyntaxKind.IfKeyword and <= SyntaxKind.FalseKeyword;

    /// <summary>
    /// Unary operator precedence; larger values bind tighter. Returns 0 when <paramref name="kind"/> is not a unary operator token.
    /// </summary>
    public static int GetUnaryOperatorPrecedence(SyntaxKind kind) =>
        kind switch
        {
            SyntaxKind.PlusToken or SyntaxKind.MinusToken or SyntaxKind.BangToken => 14,
            _ => 0,
        };

    /// <summary>
    /// Binary operator precedence; larger values bind tighter. Returns 0 when <paramref name="kind"/> is not a binary operator token.
    /// </summary>
    public static int GetBinaryOperatorPrecedence(SyntaxKind kind) =>
        kind switch
        {
            SyntaxKind.StarToken or SyntaxKind.SlashToken => 12,
            SyntaxKind.PlusToken or SyntaxKind.MinusToken => 11,
            SyntaxKind.LessToken or SyntaxKind.LessOrEqualsToken or SyntaxKind.GreaterToken or SyntaxKind.GreaterOrEqualsToken => 10,
            SyntaxKind.EqualsEqualsToken or SyntaxKind.BangEqualsToken => 9,
            SyntaxKind.AmpersandAmpersandToken => 8,
            SyntaxKind.PipePipeToken => 7,
            _ => 0,
        };

    /// <summary>
    /// Returns whether <paramref name="kind"/> is a primitive type keyword (<c>integer</c>, <c>number</c>, <c>string</c>, <c>bool</c>).
    /// </summary>
    public static bool IsTypeKeyword(SyntaxKind kind) =>
        kind is SyntaxKind.IntegerKeyword or SyntaxKind.NumberKeyword or SyntaxKind.StringKeyword or SyntaxKind.BoolKeyword;
}
