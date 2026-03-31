using System.Text;
using RoboSharp.Language.Syntax;

namespace RoboSharp.Language;

/// <summary>One syntax-tree inspector row paired with a source span for stepping highlights.</summary>
public readonly record struct SyntaxTeachingLine(string Text, TextSpan AssociatedSource);

/// <summary>Parallel to <see cref="SyntaxTreeSerializer"/> layout, with span metadata per line.</summary>
public static class SyntaxTreeTeachingListing
{
    public static IReadOnlyList<SyntaxTeachingLine> Build(CompilationUnitSyntax root)
    {
        ArgumentNullException.ThrowIfNull(root);
        var list = new List<SyntaxTeachingLine>();
        WriteCompilationUnit(list, root, 0);
        return list;
    }

    private static void Add(List<SyntaxTeachingLine> list, int depth, string text, TextSpan span)
    {
        var sb = new StringBuilder(depth * 2 + text.Length);
        sb.Append(' ', depth * 2);
        sb.Append(text);
        list.Add(new SyntaxTeachingLine(sb.ToString(), span));
    }

    private static void WriteCompilationUnit(List<SyntaxTeachingLine> list, CompilationUnitSyntax n, int depth)
    {
        Add(list, depth, nameof(CompilationUnitSyntax), TeachingSyntaxSpan.Of(n));
        foreach (var m in n.Members)
            WriteMember(list, m, depth + 1);

        Add(
            list,
            depth + 1,
            "EOF " + n.EndOfFileToken.Kind,
            n.EndOfFileToken.Span);
    }

    private static void WriteMember(List<SyntaxTeachingLine> list, MemberSyntax m, int depth)
    {
        switch (m)
        {
            case GlobalStatementSyntax g:
                Add(list, depth, nameof(GlobalStatementSyntax), TeachingSyntaxSpan.Of(g.Statement));
                WriteStatement(list, g.Statement, depth + 1);
                break;
            case FunctionDeclarationSyntax f:
                Add(
                    list,
                    depth,
                    $"{nameof(FunctionDeclarationSyntax)} {f.Identifier.Text}",
                    TeachingSyntaxSpan.Of(f));
                WriteTypeLine(list, f.ReturnType, depth + 1);
                WriteParameterList(list, f.Parameters, depth + 1);
                WriteBlock(list, f.Body, depth + 1);
                break;
            default:
                Add(list, depth, m.GetType().Name, TextSpan.Invalid);
                break;
        }
    }

    private static void WriteParameterList(List<SyntaxTeachingLine> list, ParameterListSyntax p, int depth)
    {
        Add(list, depth, nameof(ParameterListSyntax), TextSpan.Invalid);
        foreach (var param in p.Parameters)
        {
            var line = $"{param.Identifier.Text}: ";
            var sb = new StringBuilder(line);
            AppendTypeInline(sb, param.Type);
            Add(list, depth + 1, sb.ToString(), param.Identifier.Span);
        }
    }

    private static void WriteBlock(List<SyntaxTeachingLine> list, BlockStatementSyntax b, int depth)
    {
        Add(list, depth, nameof(BlockStatementSyntax), TeachingSyntaxSpan.Of(b));
        foreach (var s in b.Statements)
            WriteStatement(list, s, depth + 1);
    }

    private static void WriteStatement(List<SyntaxTeachingLine> list, StatementSyntax s, int depth)
    {
        switch (s)
        {
            case BlockStatementSyntax b:
                WriteBlock(list, b, depth);
                break;
            case VariableDeclarationStatementSyntax v:
                {
                    var sb = new StringBuilder();
                    sb.Append(nameof(VariableDeclarationStatementSyntax));
                    sb.Append(' ');
                    sb.Append(v.Identifier.Text);
                    sb.Append(" = ");
                    AppendExpressionInline(sb, v.Initializer);
                    Add(list, depth, sb.ToString(), TeachingSyntaxSpan.Of(v));
                }

                break;
            case AssignmentStatementSyntax a:
                {
                    var sb = new StringBuilder();
                    sb.Append(nameof(AssignmentStatementSyntax));
                    sb.Append(' ');
                    sb.Append(a.Identifier.Text);
                    sb.Append(" = ");
                    AppendExpressionInline(sb, a.Expression);
                    Add(list, depth, sb.ToString(), TeachingSyntaxSpan.Of(a));
                }

                break;
            case ExpressionStatementSyntax e:
                {
                    var sb = new StringBuilder();
                    sb.Append(nameof(ExpressionStatementSyntax));
                    sb.Append(' ');
                    AppendExpressionInline(sb, e.Expression);
                    Add(list, depth, sb.ToString(), TeachingSyntaxSpan.Of(e));
                }

                break;
            case IfStatementSyntax i:
                Add(list, depth, nameof(IfStatementSyntax), TeachingSyntaxSpan.Of(i));
                WriteStatement(list, i.ThenStatement, depth + 1);
                if (i.ElseClause is not null)
                {
                    var ec = i.ElseClause;
                    Add(
                        list,
                        depth + 1,
                        "else",
                        TeachingSyntaxSpan.Union(ec.ElseKeyword.Span, TeachingSyntaxSpan.Of(ec.Statement)));
                    WriteStatement(list, ec.Statement, depth + 2);
                }

                break;
            case WhileStatementSyntax w:
                Add(list, depth, nameof(WhileStatementSyntax), TeachingSyntaxSpan.Of(w));
                WriteStatement(list, w.Body, depth + 1);
                break;
            case ReturnStatementSyntax r:
                {
                    if (r.Expression is not null)
                    {
                        var sb = new StringBuilder();
                        sb.Append(nameof(ReturnStatementSyntax));
                        sb.Append(' ');
                        AppendExpressionInline(sb, r.Expression);
                        Add(list, depth, sb.ToString(), TeachingSyntaxSpan.Of(r));
                    }
                    else
                    {
                        Add(list, depth, nameof(ReturnStatementSyntax), TeachingSyntaxSpan.Of(r));
                    }
                }

                break;
            default:
                Add(list, depth, s.GetType().Name, TextSpan.Invalid);
                break;
        }
    }

    private static void WriteTypeLine(List<SyntaxTeachingLine> list, TypeSyntax t, int depth)
    {
        var sb = new StringBuilder();
        AppendTypeInline(sb, t);
        Add(list, depth, sb.ToString(), TextSpan.Invalid);
    }

    private static void AppendTypeInline(StringBuilder sb, TypeSyntax t)
    {
        switch (t)
        {
            case PrimitiveTypeSyntax p:
                sb.Append(p.Keyword.Text);
                break;
            case ArrayTypeSyntax a:
                AppendTypeInline(sb, a.ElementType);
                sb.Append("[]");
                break;
            default:
                sb.Append('?');
                break;
        }
    }

    private static void AppendExpressionInline(StringBuilder sb, ExpressionSyntax e)
    {
        switch (e)
        {
            case LiteralExpressionSyntax l:
                sb.Append(l.LiteralToken.Text);
                break;
            case NameExpressionSyntax n:
                sb.Append(n.IdentifierToken.Text);
                break;
            case UnaryExpressionSyntax u:
                sb.Append(u.OperatorToken.Text);
                AppendExpressionInline(sb, u.Operand);
                break;
            case BinaryExpressionSyntax b:
                sb.Append('(');
                AppendExpressionInline(sb, b.Left);
                sb.Append(' ');
                sb.Append(b.OperatorToken.Text);
                sb.Append(' ');
                AppendExpressionInline(sb, b.Right);
                sb.Append(')');
                break;
            case ParenthesizedExpressionSyntax p:
                sb.Append('(');
                AppendExpressionInline(sb, p.Expression);
                sb.Append(')');
                break;
            case CallExpressionSyntax c:
                AppendExpressionInline(sb, c.Callee);
                sb.Append('(');
                for (var i = 0; i < c.Arguments.Count; i++)
                {
                    if (i > 0)
                        sb.Append(", ");
                    AppendExpressionInline(sb, c.Arguments[i]);
                }

                sb.Append(')');
                break;
            case ArrayLiteralExpressionSyntax a:
                sb.Append('[');
                for (var i = 0; i < a.Elements.Count; i++)
                {
                    if (i > 0)
                        sb.Append(", ");
                    AppendExpressionInline(sb, a.Elements[i]);
                }

                sb.Append(']');
                break;
            case IndexExpressionSyntax ix:
                AppendExpressionInline(sb, ix.Target);
                sb.Append('[');
                AppendExpressionInline(sb, ix.Index);
                sb.Append(']');
                break;
            default:
                sb.Append('?');
                break;
        }
    }
}
