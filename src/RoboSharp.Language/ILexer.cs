namespace RoboSharp.Language;

/// <summary>Lexical analysis seam (see <see cref="Lexer"/>).</summary>
public interface ILexer
{
    IReadOnlyList<SyntaxToken> Lex(SourceText sourceText);
}
