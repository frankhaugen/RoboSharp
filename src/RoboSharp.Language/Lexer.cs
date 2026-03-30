using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RoboSharp.Language;

/// <summary>
/// Scans <see cref="SourceText"/> into a flat list of <see cref="SyntaxToken"/> with leading trivia.
/// </summary>
public static class Lexer
{
    /// <summary>
    /// Tokenizes <paramref name="source"/> from start to end, ending with <see cref="SyntaxKind.EndOfFileToken"/>
    /// at <see cref="SourceText.Text"/>.Length with zero width.
    /// </summary>
    public static IReadOnlyList<SyntaxToken> Tokenize(SourceText source)
    {
        ArgumentNullException.ThrowIfNull(source);
        var impl = new LexerCore(source.Text);
        return impl.TokenizeAll();
    }

    private sealed class LexerCore
    {
        private readonly string _text;
        private readonly int _length;
        private int _position;

        public LexerCore(string text)
        {
            _text = text;
            _length = text.Length;
        }

        public IReadOnlyList<SyntaxToken> TokenizeAll()
        {
            var tokens = new List<SyntaxToken>();

            while (true)
            {
                var leading = new List<SyntaxTrivia>();
                CollectLeadingTrivia(leading);

                if (_position >= _length)
                {
                    tokens.Add(new SyntaxToken(
                        SyntaxKind.EndOfFileToken,
                        new TextSpan(_length, 0),
                        string.Empty,
                        null,
                        leading,
                        Array.Empty<SyntaxTrivia>()));
                    break;
                }

                var token = ReadToken(leading);
                tokens.Add(token);
            }

            return tokens;
        }

        private void CollectLeadingTrivia(List<SyntaxTrivia> list)
        {
            while (_position < _length)
            {
                char c = _text[_position];
                if (c is ' ' or '\t')
                {
                    int start = _position;
                    while (_position < _length && (_text[_position] is ' ' or '\t'))
                        _position++;
                    int len = _position - start;
                    list.Add(new SyntaxTrivia(SyntaxKind.WhitespaceTrivia, new TextSpan(start, len), _text.Substring(start, len)));
                }
                else if (c is '\r' or '\n')
                {
                    int start = _position;
                    if (c == '\r' && _position + 1 < _length && _text[_position + 1] == '\n')
                        _position += 2;
                    else
                        _position += 1;
                    int len = _position - start;
                    list.Add(new SyntaxTrivia(SyntaxKind.EndOfLineTrivia, new TextSpan(start, len), _text.Substring(start, len)));
                }
                else if (c == '/' && _position + 1 < _length && _text[_position + 1] == '/')
                {
                    int start = _position;
                    _position += 2;
                    while (_position < _length && _text[_position] is not ('\r' or '\n'))
                        _position++;
                    int len = _position - start;
                    list.Add(new SyntaxTrivia(SyntaxKind.CommentTrivia, new TextSpan(start, len), _text.Substring(start, len)));
                }
                else
                    break;
            }
        }

        private SyntaxToken ReadToken(IReadOnlyList<SyntaxTrivia> leading)
        {
            int start = _position;
            char c = _text[_position];

            if (IsIdentifierStart(c))
                return ReadIdentifierOrKeyword(start, leading);

            if (char.IsDigit(c))
                return ReadNumericLiteral(start, leading);

            if (c == '"')
                return ReadStringLiteral(start, leading);

            SyntaxToken? twoChar = TryReadTwoCharOperator(start, leading);
            if (twoChar is not null)
                return twoChar;

            SyntaxToken? oneChar = TryReadOneCharToken(start, leading);
            if (oneChar is not null)
                return oneChar;

            return ReadBadToken(start, leading);
        }

        private static bool IsIdentifierStart(char c) =>
            char.IsLetter(c) || c == '_';

        private static bool IsIdentifierPart(char c) =>
            char.IsLetterOrDigit(c) || c == '_';

        private SyntaxToken ReadIdentifierOrKeyword(int start, IReadOnlyList<SyntaxTrivia> leading)
        {
            _position++;
            while (_position < _length && IsIdentifierPart(_text[_position]))
                _position++;

            int len = _position - start;
            string text = _text.Substring(start, len);
            SyntaxKind? kw = SyntaxFacts.GetKeywordKind(text);
            SyntaxKind kind = kw ?? SyntaxKind.IdentifierToken;
            return new SyntaxToken(kind, new TextSpan(start, len), text, null, leading, Array.Empty<SyntaxTrivia>());
        }

        private SyntaxToken ReadNumericLiteral(int start, IReadOnlyList<SyntaxTrivia> leading)
        {
            _position++;
            while (_position < _length && char.IsDigit(_text[_position]))
                _position++;

            if (_position < _length && _text[_position] == '.')
            {
                _position++;
                while (_position < _length && char.IsDigit(_text[_position]))
                    _position++;

                int len = _position - start;
                string text = _text.Substring(start, len);
                if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
                    return new SyntaxToken(SyntaxKind.BadToken, new TextSpan(start, len), text, null, leading, Array.Empty<SyntaxTrivia>());

                return new SyntaxToken(SyntaxKind.NumberLiteralToken, new TextSpan(start, len), text, d, leading, Array.Empty<SyntaxTrivia>());
            }

            int intLen = _position - start;
            string intText = _text.Substring(start, intLen);
            if (!int.TryParse(intText, NumberStyles.None, CultureInfo.InvariantCulture, out int value))
                return new SyntaxToken(SyntaxKind.BadToken, new TextSpan(start, intLen), intText, null, leading, Array.Empty<SyntaxTrivia>());

            return new SyntaxToken(SyntaxKind.IntegerLiteralToken, new TextSpan(start, intLen), intText, value, leading, Array.Empty<SyntaxTrivia>());
        }

        private SyntaxToken ReadStringLiteral(int start, IReadOnlyList<SyntaxTrivia> leading)
        {
            _position++; // opening "
            var sb = new StringBuilder();
            while (_position < _length)
            {
                char ch = _text[_position];
                if (ch == '"')
                {
                    _position++;
                    int len = _position - start;
                    string text = _text.Substring(start, len);
                    return new SyntaxToken(SyntaxKind.StringLiteralToken, new TextSpan(start, len), text, sb.ToString(), leading, Array.Empty<SyntaxTrivia>());
                }

                if (ch == '\\' && _position + 1 < _length)
                {
                    char n = _text[_position + 1];
                    if (n == '"')
                    {
                        sb.Append('"');
                        _position += 2;
                        continue;
                    }

                    if (n == '\\')
                    {
                        sb.Append('\\');
                        _position += 2;
                        continue;
                    }
                }

                sb.Append(ch);
                _position++;
            }

            // Unclosed string: bad run from opening quote to EOF
            int badLen = _position - start;
            string badText = _text.Substring(start, badLen);
            return new SyntaxToken(SyntaxKind.BadToken, new TextSpan(start, badLen), badText, null, leading, Array.Empty<SyntaxTrivia>());
        }

        private SyntaxToken? TryReadTwoCharOperator(int start, IReadOnlyList<SyntaxTrivia> leading)
        {
            if (_position + 1 >= _length)
                return null;

            char a = _text[_position];
            char b = _text[_position + 1];
            SyntaxKind? kind = (a, b) switch
            {
                ('=', '=') => SyntaxKind.EqualsEqualsToken,
                ('!', '=') => SyntaxKind.BangEqualsToken,
                ('<', '=') => SyntaxKind.LessOrEqualsToken,
                ('>', '=') => SyntaxKind.GreaterOrEqualsToken,
                ('&', '&') => SyntaxKind.AmpersandAmpersandToken,
                ('|', '|') => SyntaxKind.PipePipeToken,
                _ => null,
            };

            if (kind is null)
                return null;

            _position += 2;
            string text = _text.Substring(start, 2);
            return new SyntaxToken(kind.Value, new TextSpan(start, 2), text, null, leading, Array.Empty<SyntaxTrivia>());
        }

        private SyntaxToken? TryReadOneCharToken(int start, IReadOnlyList<SyntaxTrivia> leading)
        {
            char c = _text[_position];
            SyntaxKind? kind = c switch
            {
                '+' => SyntaxKind.PlusToken,
                '-' => SyntaxKind.MinusToken,
                '*' => SyntaxKind.StarToken,
                '/' => SyntaxKind.SlashToken,
                '!' => SyntaxKind.BangToken,
                '=' => SyntaxKind.EqualsToken,
                '<' => SyntaxKind.LessToken,
                '>' => SyntaxKind.GreaterToken,
                '(' => SyntaxKind.OpenParenToken,
                ')' => SyntaxKind.CloseParenToken,
                '{' => SyntaxKind.OpenBraceToken,
                '}' => SyntaxKind.CloseBraceToken,
                '[' => SyntaxKind.OpenBracketToken,
                ']' => SyntaxKind.CloseBracketToken,
                ',' => SyntaxKind.CommaToken,
                ';' => SyntaxKind.SemicolonToken,
                _ => null,
            };

            if (kind is null)
                return null;

            _position++;
            return new SyntaxToken(kind.Value, new TextSpan(start, 1), c.ToString(), null, leading, Array.Empty<SyntaxTrivia>());
        }

        private SyntaxToken ReadBadToken(int start, IReadOnlyList<SyntaxTrivia> leading)
        {
            _position++;
            return new SyntaxToken(
                SyntaxKind.BadToken,
                new TextSpan(start, 1),
                _text.Substring(start, 1),
                null,
                leading,
                Array.Empty<SyntaxTrivia>());
        }
    }
}
