namespace RoboSharp.Language;

/// <summary>Default <see cref="ILexer"/> using <see cref="Lexer.Tokenize"/>.</summary>
public sealed class RoboLexer : ILexer
{
    public IReadOnlyList<SyntaxToken> Lex(SourceText sourceText) => Lexer.Tokenize(sourceText);
}
