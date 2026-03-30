using RoboSharp.Language;
using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed class Binder
{
    private readonly IBuiltinProfileProvider _profile;
    private readonly List<SemanticDiagnostic> _diagnostics = new();
    private readonly Dictionary<string, FunctionSymbol> _functions = new(StringComparer.Ordinal);
    private readonly Dictionary<string, FunctionDeclarationSyntax> _canonicalFunctionDeclaration = new(StringComparer.Ordinal);
    private readonly Stack<Dictionary<string, LocalSymbol>> _scopes = new();
    private FunctionSymbol? _currentFunction;
    private int _nextSlot;

    public Binder(IBuiltinProfileProvider profile) => _profile = profile;

    public SemanticModel Bind(CompilationUnitSyntax syntax)
    {
        _functions.Clear();
        _canonicalFunctionDeclaration.Clear();
        _diagnostics.Clear();

        var functionSyntaxes = new List<FunctionDeclarationSyntax>();
        foreach (var member in syntax.Members)
        {
            switch (member)
            {
                case GlobalStatementSyntax g:
                    Report(GetStatementSpan(g.Statement), "Top-level statements are not supported; define void main() { ... } instead.");
                    break;
                case FunctionDeclarationSyntax f:
                    functionSyntaxes.Add(f);
                    break;
            }
        }

        foreach (var f in functionSyntaxes)
            TryDeclareFunction(f);

        var boundFunctions = new List<BoundFunctionDeclaration>();
        foreach (var f in functionSyntaxes)
        {
            var name = f.Identifier.Text;
            if (!_functions.ContainsKey(name))
                continue;

            if (_canonicalFunctionDeclaration.TryGetValue(name, out var canon) && !ReferenceEquals(canon, f))
                continue;

            boundFunctions.Add(BindFunctionBody(f));
        }

        FunctionSymbol? entry = null;
        foreach (var bf in boundFunctions)
        {
            if (bf.Symbol.Name != "main")
                continue;

            if (bf.Symbol.ReturnType is not PrimitiveTypeSymbol { Kind: PrimitiveTypeKind.Void })
            {
                Report(bf.Syntax.ReturnType is PrimitiveTypeSyntax pt ? pt.Keyword.Span : bf.Syntax.Identifier.Span, "main must have return type void.");
                continue;
            }

            if (bf.Symbol.Parameters.Count != 0)
            {
                Report(bf.Syntax.Parameters.OpenParenToken.Span, "main must take no parameters.");
                continue;
            }

            entry = bf.Symbol;
        }

        if (entry is null)
            Report(syntax.EndOfFileToken.Span, "Program must define void main() with no parameters.");

        var root = new BoundCompilationUnit(boundFunctions, entry);
        return new SemanticModel(syntax, root, _diagnostics.ToArray());
    }

    private void TryDeclareFunction(FunctionDeclarationSyntax syntax)
    {
        var name = syntax.Identifier.Text;
        if (_functions.ContainsKey(name))
        {
            Report(syntax.Identifier.Span, $"Duplicate function '{name}'.");
            return;
        }

        _canonicalFunctionDeclaration[name] = syntax;

        var seen = new HashSet<string>(StringComparer.Ordinal);
        var paramList = new List<ParameterSymbol>();
        var slot = 0;
        foreach (var p in syntax.Parameters.Parameters)
        {
            var pname = p.Identifier.Text;
            if (!seen.Add(pname))
            {
                Report(p.Identifier.Span, $"Duplicate parameter '{pname}'.");
                continue;
            }

            var ptype = MapType(p.Type);
            paramList.Add(new ParameterSymbol(pname, ptype, slot++));
        }

        var ret = MapType(syntax.ReturnType);
        _functions[name] = new FunctionSymbol(name, ret, paramList);
    }

    private BoundFunctionDeclaration BindFunctionBody(FunctionDeclarationSyntax syntax)
    {
        var symbol = _functions[syntax.Identifier.Text];
        _currentFunction = symbol;
        _nextSlot = symbol.Parameters.Count;

        _scopes.Clear();
        PushScope();
        foreach (var p in symbol.Parameters)
            Declare(new LocalSymbol(p.Name, p.Type, p.SlotIndex));

        var body = BindBlock(syntax.Body);
        PopScope();
        _currentFunction = null;

        return new BoundFunctionDeclaration(syntax, symbol, body);
    }

    private void PushScope() => _scopes.Push(new Dictionary<string, LocalSymbol>(StringComparer.Ordinal));

    private void PopScope() => _scopes.Pop();

    private void Declare(LocalSymbol symbol)
    {
        var dict = _scopes.Peek();
        if (dict.ContainsKey(symbol.Name))
        {
            ReportMissing($"Duplicate local '{symbol.Name}'.");
            return;
        }

        dict[symbol.Name] = symbol;
    }

    private bool TryLookup(string name, out LocalSymbol symbol)
    {
        foreach (var scope in _scopes)
        {
            if (scope.TryGetValue(name, out symbol!))
                return true;
        }

        symbol = null!;
        return false;
    }

    private BoundBlockStatement BindBlock(BlockStatementSyntax syntax)
    {
        PushScope();
        var list = new List<BoundStatement>();
        foreach (var s in syntax.Statements)
            list.Add(BindStatement(s));

        PopScope();
        return new BoundBlockStatement(syntax, list);
    }

    private BoundStatement BindStatement(StatementSyntax syntax) =>
        syntax switch
        {
            BlockStatementSyntax b => BindBlock(b),
            VariableDeclarationStatementSyntax v => BindVariableDeclaration(v),
            AssignmentStatementSyntax a => BindAssignment(a),
            ExpressionStatementSyntax e => BindExpressionStatement(e),
            IfStatementSyntax i => BindIf(i),
            WhileStatementSyntax w => BindWhile(w),
            ReturnStatementSyntax r => BindReturn(r),
            _ => throw new InvalidOperationException(),
        };

    private BoundStatement BindVariableDeclaration(VariableDeclarationStatementSyntax syntax)
    {
        var type = MapType(syntax.Type);
        var init = BindExpression(syntax.Initializer);
        init = CoerceAssignment(type, init, syntax.EqualsToken.Span);
        var name = syntax.Identifier.Text;
        if (_scopes.Peek().ContainsKey(name))
        {
            Report(syntax.Identifier.Span, $"Duplicate local '{name}'.");
            return new BoundExpressionStatement(
                new ExpressionStatementSyntax(
                    new LiteralExpressionSyntax(
                        new SyntaxToken(SyntaxKind.IntegerLiteralToken, syntax.Identifier.Span, "0", 0, [], [])),
                    syntax.SemicolonToken),
                new BoundLiteralExpression(
                    new LiteralExpressionSyntax(
                        new SyntaxToken(SyntaxKind.IntegerLiteralToken, syntax.Identifier.Span, "0", 0, [], [])),
                    PrimitiveTypeSymbol.Int,
                    0));
        }

        var sym = new LocalSymbol(name, type, _nextSlot++);
        Declare(sym);
        return new BoundVariableDeclarationStatement(syntax, sym, init);
    }

    private BoundStatement BindAssignment(AssignmentStatementSyntax syntax)
    {
        var name = syntax.Identifier.Text;
        if (!TryLookup(name, out var sym))
        {
            Report(syntax.Identifier.Span, $"Unknown variable '{name}'.");
            return SyntheticExprStmt();
        }

        var expr = BindExpression(syntax.Expression);
        expr = CoerceAssignment(sym.Type, expr, syntax.EqualsToken.Span);
        return new BoundAssignmentStatement(syntax, sym, expr);
    }

    private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
    {
        var e = BindExpression(syntax.Expression);
        return new BoundExpressionStatement(syntax, e);
    }

    private BoundStatement BindIf(IfStatementSyntax syntax)
    {
        var cond = BindExpression(syntax.Condition);
        cond = CoerceToBool(cond, syntax.OpenParenToken.Span);
        var then = BindStatement(syntax.ThenStatement);
        BoundStatement? els = null;
        if (syntax.ElseClause is not null)
            els = BindStatement(syntax.ElseClause.Statement);

        return new BoundIfStatement(syntax, cond, then, els);
    }

    private BoundStatement BindWhile(WhileStatementSyntax syntax)
    {
        var cond = BindExpression(syntax.Condition);
        cond = CoerceToBool(cond, syntax.OpenParenToken.Span);
        var body = BindStatement(syntax.Body);
        return new BoundWhileStatement(syntax, cond, body);
    }

    private BoundStatement BindReturn(ReturnStatementSyntax syntax)
    {
        var fn = _currentFunction!;
        if (syntax.Expression is null)
        {
            if (fn.ReturnType is not PrimitiveTypeSymbol { Kind: PrimitiveTypeKind.Void })
                Report(syntax.ReturnKeyword.Span, "Return value required.");

            return new BoundReturnStatement(syntax, null, fn.ReturnType);
        }

        var expr = BindExpression(syntax.Expression);
        expr = CoerceAssignment(fn.ReturnType, expr, GetExprSpan(syntax.Expression!));
        return new BoundReturnStatement(syntax, expr, fn.ReturnType);
    }

    private BoundExpression BindExpression(ExpressionSyntax syntax) =>
        syntax switch
        {
            LiteralExpressionSyntax l => BindLiteral(l),
            NameExpressionSyntax n => BindName(n),
            UnaryExpressionSyntax u => BindUnary(u),
            BinaryExpressionSyntax b => BindBinary(b),
            ParenthesizedExpressionSyntax p => BindExpression(p.Expression),
            CallExpressionSyntax c => BindCall(c),
            ArrayLiteralExpressionSyntax a => BindArrayLiteral(a),
            IndexExpressionSyntax i => BindIndex(i),
            _ => throw new InvalidOperationException(),
        };

    private BoundExpression BindLiteral(LiteralExpressionSyntax syntax)
    {
        var t = syntax.LiteralToken.Kind switch
        {
            SyntaxKind.IntegerLiteralToken => (TypeSymbol)PrimitiveTypeSymbol.Int,
            SyntaxKind.NumberLiteralToken => PrimitiveTypeSymbol.Number,
            SyntaxKind.StringLiteralToken => PrimitiveTypeSymbol.String,
            SyntaxKind.TrueKeyword or SyntaxKind.FalseKeyword => PrimitiveTypeSymbol.Bool,
            _ => PrimitiveTypeSymbol.Int,
        };

        object val = t switch
        {
            PrimitiveTypeSymbol { Kind: PrimitiveTypeKind.Int } => syntax.LiteralToken.Value ?? 0,
            PrimitiveTypeSymbol { Kind: PrimitiveTypeKind.Number } => syntax.LiteralToken.Value ?? 0.0,
            PrimitiveTypeSymbol { Kind: PrimitiveTypeKind.String } => syntax.LiteralToken.Value ?? "",
            PrimitiveTypeSymbol { Kind: PrimitiveTypeKind.Bool } => syntax.LiteralToken.Kind == SyntaxKind.TrueKeyword,
            _ => 0,
        };

        return new BoundLiteralExpression(syntax, t, val);
    }

    private BoundExpression BindName(NameExpressionSyntax syntax)
    {
        if (!TryLookup(syntax.IdentifierToken.Text, out var sym))
        {
            Report(syntax.IdentifierToken.Span, $"Unknown name '{syntax.IdentifierToken.Text}'.");
            return ErrorExpr(syntax);
        }

        return new BoundVariableExpression(syntax, sym);
    }

    private BoundExpression BindUnary(UnaryExpressionSyntax syntax)
    {
        var op = syntax.OperatorToken.Kind;
        var operand = BindExpression(syntax.Operand);
        if (op == SyntaxKind.MinusToken)
        {
            operand = PromoteNumberToInt(operand, GetExprSpan(syntax.Operand));
            if (!TypeEquals(operand.Type, PrimitiveTypeSymbol.Int))
            {
                Report(syntax.OperatorToken.Span, "Unary '-' requires integer.");
                return ErrorExpr(syntax);
            }

            return new BoundUnaryExpression(syntax, PrimitiveTypeSymbol.Int, operand);
        }

        if (op == SyntaxKind.BangToken)
        {
            operand = CoerceToBool(operand, GetExprSpan(syntax.Operand));
            return new BoundUnaryExpression(syntax, PrimitiveTypeSymbol.Bool, operand);
        }

        if (op == SyntaxKind.PlusToken)
        {
            operand = PromoteNumberToInt(operand, GetExprSpan(syntax.Operand));
            if (!TypeEquals(operand.Type, PrimitiveTypeSymbol.Int))
            {
                Report(syntax.OperatorToken.Span, "Unary '+' requires integer.");
                return ErrorExpr(syntax);
            }

            return new BoundUnaryExpression(syntax, PrimitiveTypeSymbol.Int, operand);
        }

        Report(syntax.OperatorToken.Span, "Unsupported unary operator.");
        return ErrorExpr(syntax);
    }

    private BoundExpression BindBinary(BinaryExpressionSyntax syntax)
    {
        var left = BindExpression(syntax.Left);
        var right = BindExpression(syntax.Right);
        var op = syntax.OperatorToken.Kind;

        if (op is SyntaxKind.AmpersandAmpersandToken or SyntaxKind.PipePipeToken)
        {
            left = CoerceToBool(left, GetExprSpan(syntax.Left));
            right = CoerceToBool(right, GetExprSpan(syntax.Right));
            return new BoundBinaryExpression(syntax, PrimitiveTypeSymbol.Bool, left, right);
        }

        left = PromoteNumberToInt(left, GetExprSpan(syntax.Left));
        right = PromoteNumberToInt(right, GetExprSpan(syntax.Right));
        if (!TypeEquals(left.Type, PrimitiveTypeSymbol.Int) || !TypeEquals(right.Type, PrimitiveTypeSymbol.Int))
        {
            Report(syntax.OperatorToken.Span, "Arithmetic and comparisons require integer operands.");
            return ErrorExpr(syntax);
        }

        var resultType = op is SyntaxKind.EqualsEqualsToken or SyntaxKind.BangEqualsToken
            or SyntaxKind.LessToken or SyntaxKind.LessOrEqualsToken
            or SyntaxKind.GreaterToken or SyntaxKind.GreaterOrEqualsToken
            ? (TypeSymbol)PrimitiveTypeSymbol.Bool
            : PrimitiveTypeSymbol.Int;

        return new BoundBinaryExpression(syntax, resultType, left, right);
    }

    private BoundExpression BindCall(CallExpressionSyntax syntax)
    {
        if (syntax.Callee is not NameExpressionSyntax nameExpr)
        {
            Report(GetCalleeSpan(syntax), "Only direct calls to named functions or built-ins are supported.");
            return ErrorExpr(syntax);
        }

        var name = nameExpr.IdentifierToken.Text;
        if (_functions.TryGetValue(name, out var fn))
        {
            var args = new List<BoundExpression>();
            if (syntax.Arguments.Count != fn.Parameters.Count)
            {
                Report(syntax.OpenParenToken.Span, $"Function '{name}' expects {fn.Parameters.Count} argument(s).");
            }
            else
            {
                for (var i = 0; i < syntax.Arguments.Count; i++)
                {
            var a = BindExpression(syntax.Arguments[i]);
            a = CoerceAssignment(fn.Parameters[i].Type, a, GetExprSpan(syntax.Arguments[i]));
                    args.Add(a);
                }
            }

            while (args.Count < fn.Parameters.Count)
                args.Add(ErrorLiteral());

            return new BoundCallExpression(syntax, fn.ReturnType, fn, args);
        }

        if (!BuiltinCatalog.TryGet(name, out var sig))
        {
            Report(nameExpr.IdentifierToken.Span, $"Unknown function or built-in '{name}'.");
            return ErrorExpr(syntax);
        }

        if (!_profile.IsAvailable(sig.Id))
        {
            Report(nameExpr.IdentifierToken.Span, $"Built-in '{name}' is not available in the active profile.");
            return ErrorExpr(syntax);
        }

        if (sig.Id == BuiltinId.Print)
        {
            if (syntax.Arguments.Count != 1)
            {
                Report(syntax.OpenParenToken.Span, "print expects 1 argument.");
                return ErrorExpr(syntax);
            }

            var arg = BindExpression(syntax.Arguments[0]);
            if (arg.Type is not PrimitiveTypeSymbol and not ArrayTypeSymbol)
            {
                Report(GetExprSpan(syntax.Arguments[0]), "print argument must be a primitive or array.");
                return ErrorExpr(syntax);
            }

            return new BoundBuiltinCallExpression(syntax, PrimitiveTypeSymbol.Void, BuiltinId.Print, [arg]);
        }

        if (syntax.Arguments.Count != sig.ParameterTypes.Count)
        {
            Report(syntax.OpenParenToken.Span, $"Built-in '{name}' expects {sig.ParameterTypes.Count} argument(s).");
            return ErrorExpr(syntax);
        }

        var boundArgs = new List<BoundExpression>();
        for (var i = 0; i < syntax.Arguments.Count; i++)
        {
            var a = BindExpression(syntax.Arguments[i]);
            a = CoerceAssignment(sig.ParameterTypes[i], a, GetExprSpan(syntax.Arguments[i]));
            boundArgs.Add(a);
        }

        return new BoundBuiltinCallExpression(syntax, sig.ReturnType, sig.Id, boundArgs);
    }

    private BoundExpression BindArrayLiteral(ArrayLiteralExpressionSyntax syntax)
    {
        if (syntax.Elements.Count == 0)
        {
            Report(syntax.OpenBracketToken.Span, "Array literal must have at least one element.");
            return ErrorExpr(syntax);
        }

        var elems = new List<BoundExpression>();
        BoundExpression? first = null;
        foreach (var el in syntax.Elements)
        {
            var e = BindExpression(el);
            e = PromoteNumberToInt(e, GetExprSpan(el));
            if (first is null)
                first = e;
            else if (!TypeEquals(first.Type, e.Type))
            {
                Report(GetExprSpan(el), "Array literal elements must have the same type.");
                e = first;
            }

            elems.Add(e);
        }

        if (first is null || !TypeEquals(first.Type, PrimitiveTypeSymbol.Int))
        {
            Report(syntax.OpenBracketToken.Span, "Only integer[] array literals are supported.");
            return ErrorExpr(syntax);
        }

        var arrType = new ArrayTypeSymbol(PrimitiveTypeSymbol.Int);
        return new BoundArrayCreationExpression(syntax, arrType, elems);
    }

    private BoundExpression BindIndex(IndexExpressionSyntax syntax)
    {
        var target = BindExpression(syntax.Target);
        var index = BindExpression(syntax.Index);
        index = PromoteNumberToInt(index, GetExprSpan(syntax.Index));
        if (target.Type is not ArrayTypeSymbol arr)
        {
            Report(GetExprSpan(syntax.Target), "Indexing requires an array.");
            return ErrorExpr(syntax);
        }

        if (!TypeEquals(index.Type, PrimitiveTypeSymbol.Int))
        {
            Report(GetExprSpan(syntax.Index), "Array index must be integer.");
            return ErrorExpr(syntax);
        }

        return new BoundIndexExpression(syntax, arr.Element, target, index);
    }

    private static TextSpan GetCalleeSpan(CallExpressionSyntax syntax) =>
        syntax.Callee switch
        {
            NameExpressionSyntax n => n.IdentifierToken.Span,
            _ => syntax.OpenParenToken.Span,
        };

    private static TypeSymbol MapType(TypeSyntax syntax) =>
        syntax switch
        {
            PrimitiveTypeSyntax p => p.Keyword.Kind switch
            {
                SyntaxKind.IntegerKeyword => PrimitiveTypeSymbol.Int,
                SyntaxKind.BoolKeyword => PrimitiveTypeSymbol.Bool,
                SyntaxKind.StringKeyword => PrimitiveTypeSymbol.String,
                SyntaxKind.NumberKeyword => PrimitiveTypeSymbol.Number,
                SyntaxKind.VoidKeyword => PrimitiveTypeSymbol.Void,
                _ => PrimitiveTypeSymbol.Int,
            },
            ArrayTypeSyntax a => new ArrayTypeSymbol(MapType(a.ElementType)),
            _ => PrimitiveTypeSymbol.Int,
        };

    private BoundExpression CoerceAssignment(TypeSymbol target, BoundExpression expr, TextSpan errorSpan)
    {
        if (TypeEquals(target, expr.Type))
            return expr;

        if (target is PrimitiveTypeSymbol { Kind: PrimitiveTypeKind.Int } &&
            expr.Type is PrimitiveTypeSymbol { Kind: PrimitiveTypeKind.Number })
            return new BoundConversionExpression(expr, PrimitiveTypeSymbol.Int);

        Report(errorSpan, $"Cannot convert {Describe(expr.Type)} to {Describe(target)}.");
        return expr;
    }

    private BoundExpression CoerceToBool(BoundExpression expr, TextSpan span)
    {
        if (TypeEquals(expr.Type, PrimitiveTypeSymbol.Bool))
            return expr;

        Report(span, "Expected bool.");
        return expr;
    }

    private BoundExpression PromoteNumberToInt(BoundExpression expr, TextSpan span)
    {
        if (expr.Type is PrimitiveTypeSymbol { Kind: PrimitiveTypeKind.Number })
            return new BoundConversionExpression(expr, PrimitiveTypeSymbol.Int);

        return expr;
    }

    private static bool TypeEquals(TypeSymbol a, TypeSymbol b) =>
        (a, b) switch
        {
            (PrimitiveTypeSymbol pa, PrimitiveTypeSymbol pb) => pa.Kind == pb.Kind,
            (ArrayTypeSymbol aa, ArrayTypeSymbol ab) => TypeEquals(aa.Element, ab.Element),
            _ => false,
        };

    private static string Describe(TypeSymbol t) =>
        t switch
        {
            PrimitiveTypeSymbol p => p.Kind.ToString(),
            ArrayTypeSymbol a => Describe(a.Element) + "[]",
            _ => "?",
        };

    private void Report(TextSpan span, string message) => _diagnostics.Add(new SemanticDiagnostic(span, message));

    private void ReportMissing(string message) => _diagnostics.Add(new SemanticDiagnostic(default, message));

    private BoundExpression ErrorExpr(ExpressionSyntax syntax) =>
        new BoundLiteralExpression(
            new LiteralExpressionSyntax(
                new SyntaxToken(SyntaxKind.IntegerLiteralToken, GetExprSpan(syntax), "0", 0, [], [])),
            PrimitiveTypeSymbol.Int,
            0);

    private BoundLiteralExpression ErrorLiteral() =>
        new BoundLiteralExpression(
            new LiteralExpressionSyntax(
                new SyntaxToken(SyntaxKind.IntegerLiteralToken, default, "0", 0, [], [])),
            PrimitiveTypeSymbol.Int,
            0);

    private BoundStatement SyntheticExprStmt() =>
        new BoundExpressionStatement(
            new ExpressionStatementSyntax(
                new LiteralExpressionSyntax(
                    new SyntaxToken(SyntaxKind.IntegerLiteralToken, default, "0", 0, [], [])),
                new SyntaxToken(SyntaxKind.SemicolonToken, default, ";", null, [], [])),
            ErrorLiteral());

    private static TextSpan GetExprSpan(ExpressionSyntax syntax) =>
        syntax switch
        {
            NameExpressionSyntax n => n.IdentifierToken.Span,
            LiteralExpressionSyntax l => l.LiteralToken.Span,
            CallExpressionSyntax c => c.CloseParenToken.Span,
            BinaryExpressionSyntax b => b.OperatorToken.Span,
            UnaryExpressionSyntax u => u.OperatorToken.Span,
            ParenthesizedExpressionSyntax p => p.OpenParenToken.Span,
            IndexExpressionSyntax i => i.OpenBracketToken.Span,
            ArrayLiteralExpressionSyntax a => a.OpenBracketToken.Span,
            _ => default,
        };

    private static TextSpan GetStatementSpan(StatementSyntax s) =>
        s switch
        {
            BlockStatementSyntax b => b.OpenBraceToken.Span,
            VariableDeclarationStatementSyntax v => v.Type is PrimitiveTypeSyntax pt ? pt.Keyword.Span : default,
            AssignmentStatementSyntax a => a.Identifier.Span,
            ExpressionStatementSyntax e => GetExprSpan(e.Expression),
            IfStatementSyntax i => i.IfKeyword.Span,
            WhileStatementSyntax w => w.WhileKeyword.Span,
            ReturnStatementSyntax r => r.ReturnKeyword.Span,
            _ => default,
        };
}
