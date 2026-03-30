using RoboSharp.Language.Syntax;

namespace RoboSharp.Language;

internal sealed class ParserCore
{
    private readonly IReadOnlyList<SyntaxToken> _tokens;
    private readonly SourceText _source;
    private readonly List<ParseDiagnostic> _diagnostics = [];
    private int _index;

    public ParserCore(IReadOnlyList<SyntaxToken> tokens, SourceText source)
    {
        _tokens = tokens;
        _source = source;
    }

    public SyntaxTree Parse()
    {
        var members = new List<MemberSyntax>();
        while (!IsAtEnd())
        {
            if (Current.Kind == SyntaxKind.EndOfFileToken)
                break;

            members.Add(ParseMember());
        }

        var eof = Match(SyntaxKind.EndOfFileToken);
        var root = new CompilationUnitSyntax(members, eof);
        return new SyntaxTree(_source, root, _diagnostics);
    }

    private SyntaxToken Current => _tokens[_index];

    private SyntaxToken Peek(int offset) => _tokens[Math.Min(_index + offset, _tokens.Count - 1)];

    private bool IsAtEnd() => Current.Kind == SyntaxKind.EndOfFileToken;

    private SyntaxToken Advance()
    {
        var t = Current;
        if (!IsAtEnd())
            _index++;
        return t;
    }

    private void Report(string message, TextSpan span) =>
        _diagnostics.Add(new ParseDiagnostic(span, message));

    private SyntaxToken Match(SyntaxKind kind)
    {
        if (Current.Kind == kind)
            return Advance();

        Report($"Expected '{kind}' but found '{Current.Kind}'.", Current.Span);
        return MissingToken(kind);
    }

    private SyntaxToken MissingToken(SyntaxKind kind) =>
        new(kind, new TextSpan(Current.Span.Start, 0), string.Empty, null, [], []);

    private MemberSyntax ParseMember()
    {
        if (SyntaxFacts.IsTypeKeyword(Current.Kind))
        {
            var type = ParseType();
            var id = Match(SyntaxKind.IdentifierToken);
            if (Current.Kind == SyntaxKind.OpenParenToken)
            {
                var parameters = ParseParameterList();
                var body = ParseBlock();
                return new FunctionDeclarationSyntax(type, id, parameters, body);
            }

            if (Current.Kind == SyntaxKind.EqualsToken)
            {
                var eq = Advance();
                var init = ParseExpression();
                var semi = Match(SyntaxKind.SemicolonToken);
                var stmt = new VariableDeclarationStatementSyntax(type, id, eq, init, semi);
                return new GlobalStatementSyntax(stmt);
            }

            Report("Expected '(' for function or '=' for variable declaration after identifier.", id.Span);
            SkipToMemberSync();
            return new GlobalStatementSyntax(SyntheticExpressionStatement());
        }

        var statement = ParseStatement();
        return new GlobalStatementSyntax(statement);
    }

    private void SkipToMemberSync()
    {
        while (!IsAtEnd())
        {
            if (Current.Kind is SyntaxKind.CloseBraceToken or SyntaxKind.SemicolonToken)
            {
                Advance();
                return;
            }

            if (SyntaxFacts.IsTypeKeyword(Current.Kind))
                return;

            if (Current.Kind is SyntaxKind.IfKeyword or SyntaxKind.WhileKeyword or SyntaxKind.ReturnKeyword)
                return;

            Advance();
        }
    }

    private StatementSyntax SyntheticExpressionStatement()
    {
        var tok = new SyntaxToken(
            SyntaxKind.IdentifierToken,
            new TextSpan(Current.Span.Start, 0),
            "",
            null,
            [],
            []);
        var expr = new NameExpressionSyntax(tok);
        return new ExpressionStatementSyntax(expr, MissingToken(SyntaxKind.SemicolonToken));
    }

    private ParameterListSyntax ParseParameterList()
    {
        var open = Match(SyntaxKind.OpenParenToken);
        var parameters = new List<ParameterSyntax>();
        var commas = new List<SyntaxToken>();

        if (Current.Kind != SyntaxKind.CloseParenToken)
        {
            while (true)
            {
                var pType = ParseType();
                var pId = Match(SyntaxKind.IdentifierToken);
                parameters.Add(new ParameterSyntax(pType, pId));

                if (Current.Kind == SyntaxKind.CommaToken)
                {
                    commas.Add(Advance());
                    continue;
                }

                break;
            }
        }

        var close = Match(SyntaxKind.CloseParenToken);
        return new ParameterListSyntax(open, parameters, commas, close);
    }

    private BlockStatementSyntax ParseBlock()
    {
        var open = Match(SyntaxKind.OpenBraceToken);
        var stmts = new List<StatementSyntax>();
        while (Current.Kind != SyntaxKind.CloseBraceToken && !IsAtEnd())
            stmts.Add(ParseStatement());

        var close = Match(SyntaxKind.CloseBraceToken);
        return new BlockStatementSyntax(open, stmts, close);
    }

    private StatementSyntax ParseStatement()
    {
        if (Current.Kind == SyntaxKind.OpenBraceToken)
            return ParseBlock();

        if (Current.Kind == SyntaxKind.IfKeyword)
            return ParseIfStatement();

        if (Current.Kind == SyntaxKind.WhileKeyword)
            return ParseWhileStatement();

        if (Current.Kind == SyntaxKind.ReturnKeyword)
            return ParseReturnStatement();

        if (SyntaxFacts.IsTypeKeyword(Current.Kind))
        {
            var type = ParseType();
            var id = Match(SyntaxKind.IdentifierToken);
            var eq = Match(SyntaxKind.EqualsToken);
            var init = ParseExpression();
            var semi = Match(SyntaxKind.SemicolonToken);
            return new VariableDeclarationStatementSyntax(type, id, eq, init, semi);
        }

        if (Current.Kind == SyntaxKind.IdentifierToken && Peek(1).Kind == SyntaxKind.EqualsToken)
        {
            var id = Advance();
            var eq = Advance();
            var expr = ParseExpression();
            var semi = Match(SyntaxKind.SemicolonToken);
            return new AssignmentStatementSyntax(id, eq, expr, semi);
        }

        var expression = ParseExpression();
        var semicolon = Match(SyntaxKind.SemicolonToken);
        return new ExpressionStatementSyntax(expression, semicolon);
    }

    private IfStatementSyntax ParseIfStatement()
    {
        var ifKw = Advance();
        var open = Match(SyntaxKind.OpenParenToken);
        var cond = ParseExpression();
        var close = Match(SyntaxKind.CloseParenToken);
        var then = ParseStatement();
        ElseClauseSyntax? elseClause = null;
        if (Current.Kind == SyntaxKind.ElseKeyword)
        {
            var elseKw = Advance();
            var elseStmt = ParseStatement();
            elseClause = new ElseClauseSyntax(elseKw, elseStmt);
        }

        return new IfStatementSyntax(ifKw, open, cond, close, then, elseClause);
    }

    private WhileStatementSyntax ParseWhileStatement()
    {
        var w = Advance();
        var open = Match(SyntaxKind.OpenParenToken);
        var cond = ParseExpression();
        var close = Match(SyntaxKind.CloseParenToken);
        var body = ParseStatement();
        return new WhileStatementSyntax(w, open, cond, close, body);
    }

    private ReturnStatementSyntax ParseReturnStatement()
    {
        var r = Advance();
        if (Current.Kind == SyntaxKind.SemicolonToken)
        {
            var semi = Advance();
            return new ReturnStatementSyntax(r, null, semi);
        }

        var expr = ParseExpression();
        var s = Match(SyntaxKind.SemicolonToken);
        return new ReturnStatementSyntax(r, expr, s);
    }

    private TypeSyntax ParseType()
    {
        TypeSyntax type = ParsePrimitiveType();
        while (Current.Kind == SyntaxKind.OpenBracketToken)
        {
            var ob = Advance();
            var cb = Match(SyntaxKind.CloseBracketToken);
            type = new ArrayTypeSyntax(type, ob, cb);
        }

        return type;
    }

    private PrimitiveTypeSyntax ParsePrimitiveType()
    {
        if (!SyntaxFacts.IsTypeKeyword(Current.Kind))
        {
            Report("Expected primitive type keyword.", Current.Span);
            return new PrimitiveTypeSyntax(MissingToken(SyntaxKind.IntegerKeyword));
        }

        var kw = Advance();
        return new PrimitiveTypeSyntax(kw);
    }

    private ExpressionSyntax ParseExpression() => ParseBinaryExpression(parentMinPrecedence: 0);

    private ExpressionSyntax ParseBinaryExpression(int parentMinPrecedence)
    {
        var left = ParseUnaryExpression();
        while (true)
        {
            var precedence = SyntaxFacts.GetBinaryOperatorPrecedence(Current.Kind);
            if (precedence == 0 || precedence < parentMinPrecedence)
                break;

            var op = Advance();
            var right = ParseBinaryExpression(precedence + 1);
            left = new BinaryExpressionSyntax(left, op, right);
        }

        return left;
    }

    private ExpressionSyntax ParseUnaryExpression()
    {
        var kind = Current.Kind;
        if (SyntaxFacts.GetUnaryOperatorPrecedence(kind) > 0)
        {
            var op = Advance();
            var operand = ParseUnaryExpression();
            return new UnaryExpressionSyntax(op, operand);
        }

        return ParsePostfixExpression();
    }

    private ExpressionSyntax ParsePostfixExpression()
    {
        var expr = ParsePrimaryExpression();
        while (true)
        {
            if (Current.Kind == SyntaxKind.OpenParenToken)
            {
                var open = Advance();
                var args = new List<ExpressionSyntax>();
                var commas = new List<SyntaxToken>();
                if (Current.Kind != SyntaxKind.CloseParenToken)
                {
                    while (true)
                    {
                        args.Add(ParseExpression());
                        if (Current.Kind == SyntaxKind.CommaToken)
                        {
                            commas.Add(Advance());
                            continue;
                        }

                        break;
                    }
                }

                var close = Match(SyntaxKind.CloseParenToken);
                expr = new CallExpressionSyntax(expr, open, args, commas, close);
                continue;
            }

            if (Current.Kind == SyntaxKind.OpenBracketToken)
            {
                var ob = Advance();
                var indexExpr = ParseExpression();
                var cb = Match(SyntaxKind.CloseBracketToken);
                expr = new IndexExpressionSyntax(expr, ob, indexExpr, cb);
                continue;
            }

            break;
        }

        return expr;
    }

    private ExpressionSyntax ParsePrimaryExpression()
    {
        switch (Current.Kind)
        {
            case SyntaxKind.IntegerLiteralToken:
            case SyntaxKind.NumberLiteralToken:
            case SyntaxKind.StringLiteralToken:
            case SyntaxKind.TrueKeyword:
            case SyntaxKind.FalseKeyword:
                return new LiteralExpressionSyntax(Advance());

            case SyntaxKind.IdentifierToken:
                return new NameExpressionSyntax(Advance());

            case SyntaxKind.OpenParenToken:
            {
                var open = Advance();
                var inner = ParseExpression();
                var close = Match(SyntaxKind.CloseParenToken);
                return new ParenthesizedExpressionSyntax(open, inner, close);
            }

            case SyntaxKind.OpenBracketToken:
                return ParseArrayLiteral();

            default:
                var errorSpan = Current.Span;
                Report($"Unexpected token '{Current.Kind}' in expression.", errorSpan);
                if (Current.Kind == SyntaxKind.BadToken)
                    Advance();
                else if (!IsAtEnd())
                    Advance();

                return new LiteralExpressionSyntax(
                    new SyntaxToken(
                        SyntaxKind.IntegerLiteralToken,
                        new TextSpan(errorSpan.Start, 0),
                        "0",
                        0,
                        [],
                        []));
        }
    }

    private ArrayLiteralExpressionSyntax ParseArrayLiteral()
    {
        var open = Advance();
        var elements = new List<ExpressionSyntax>();
        var commas = new List<SyntaxToken>();
        if (Current.Kind != SyntaxKind.CloseBracketToken)
        {
            while (true)
            {
                elements.Add(ParseExpression());
                if (Current.Kind == SyntaxKind.CommaToken)
                {
                    commas.Add(Advance());
                    continue;
                }

                break;
            }
        }

        var close = Match(SyntaxKind.CloseBracketToken);
        return new ArrayLiteralExpressionSyntax(open, elements, commas, close);
    }
}
