using RoboSharp.Language.Syntax;

namespace RoboSharp.Language;

/// <summary>Maps concrete syntax nodes to a contiguous source span for teaching execution stepping.</summary>
public static class TeachingSyntaxSpan
{
    public static TextSpan Of(ExpressionSyntax e) =>
        e switch
        {
            LiteralExpressionSyntax l => l.LiteralToken.Span,
            NameExpressionSyntax n => n.IdentifierToken.Span,
            ParenthesizedExpressionSyntax p => Union(p.OpenParenToken.Span, p.CloseParenToken.Span),
            UnaryExpressionSyntax u => Union(u.OperatorToken.Span, Of(u.Operand)),
            BinaryExpressionSyntax b => Union(Union(Of(b.Left), b.OperatorToken.Span), Of(b.Right)),
            CallExpressionSyntax c => Union(Of(c.Callee), c.CloseParenToken.Span),
            ArrayLiteralExpressionSyntax a => Union(a.OpenBracketToken.Span, a.CloseBracketToken.Span),
            IndexExpressionSyntax i => Union(Of(i.Target), i.CloseBracketToken.Span),
            _ => TextSpan.Invalid,
        };

    public static TextSpan Of(StatementSyntax s) =>
        s switch
        {
            BlockStatementSyntax b => Union(b.OpenBraceToken.Span, b.CloseBraceToken.Span),
            ExpressionStatementSyntax e => Union(Of(e.Expression), e.SemicolonToken.Span),
            VariableDeclarationStatementSyntax v =>
                Union(Union(TypeTokenSpan(v.Type), v.Identifier.Span), v.SemicolonToken.Span),
            AssignmentStatementSyntax a =>
                Union(Union(a.Identifier.Span, a.EqualsToken.Span), a.SemicolonToken.Span),
            IfStatementSyntax i =>
                Union(Union(i.IfKeyword.Span, i.CloseParenToken.Span), Of(i.ThenStatement)),
            WhileStatementSyntax w =>
                Union(Union(w.WhileKeyword.Span, w.CloseParenToken.Span), Of(w.Body)),
            ReturnStatementSyntax r => r.Expression is { } ex
                ? Union(Union(r.ReturnKeyword.Span, Of(ex)), r.SemicolonToken.Span)
                : Union(r.ReturnKeyword.Span, r.SemicolonToken.Span),
            _ => TextSpan.Invalid,
        };

    public static TextSpan Of(CompilationUnitSyntax root) =>
        root.Members.Count > 0
            ? OfMember(root.Members[0])
            : root.EndOfFileToken.Span;

    public static TextSpan Of(FunctionDeclarationSyntax f) =>
        Union(f.Identifier.Span, f.Body.CloseBraceToken.Span);

    private static TextSpan OfMember(MemberSyntax m) =>
        m switch
        {
            GlobalStatementSyntax g => Of(g.Statement),
            FunctionDeclarationSyntax f => Of(f),
            _ => TextSpan.Invalid,
        };

    private static TextSpan TypeTokenSpan(TypeSyntax t) =>
        t switch
        {
            PrimitiveTypeSyntax p => p.Keyword.Span,
            ArrayTypeSyntax a => Union(TypeTokenSpan(a.ElementType), Union(a.OpenBracketToken.Span, a.CloseBracketToken.Span)),
            _ => TextSpan.Invalid,
        };

    public static TextSpan Union(TextSpan a, TextSpan b)
    {
        if (!a.IsValid)
            return b.IsValid ? b : TextSpan.Invalid;
        if (!b.IsValid)
            return a;
        var start = Math.Min(a.Start, b.Start);
        var end = Math.Max(a.End, b.End);
        return new TextSpan(start, end - start);
    }
}
