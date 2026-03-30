using System.Text;
using RoboSharp.Language.Syntax;

namespace RoboSharp.Language;

/// <summary>Human-readable dump of a compilation unit for teaching / debugging.</summary>
public interface ISyntaxTreeSerializer
{
    string Serialize(CompilationUnitSyntax root);
}

public sealed class SyntaxTreeSerializer : ISyntaxTreeSerializer
{
    public string Serialize(CompilationUnitSyntax root)
    {
        ArgumentNullException.ThrowIfNull(root);
        var sb = new StringBuilder();
        WriteCompilationUnit(sb, root, 0);
        return sb.ToString();
    }

    private static void Indent(StringBuilder sb, int depth)
    {
        sb.Append(' ', depth * 2);
    }

    private static void WriteCompilationUnit(StringBuilder sb, CompilationUnitSyntax n, int depth)
    {
        Indent(sb, depth);
        sb.AppendLine(nameof(CompilationUnitSyntax));
        foreach (var m in n.Members)
            WriteMember(sb, m, depth + 1);

        Indent(sb, depth + 1);
        sb.Append("EOF ");
        sb.AppendLine(n.EndOfFileToken.Kind.ToString());
    }

    private static void WriteMember(StringBuilder sb, MemberSyntax m, int depth)
    {
        switch (m)
        {
            case GlobalStatementSyntax g:
                Indent(sb, depth);
                sb.AppendLine(nameof(GlobalStatementSyntax));
                WriteStatement(sb, g.Statement, depth + 1);
                break;
            case FunctionDeclarationSyntax f:
                Indent(sb, depth);
                sb.Append(nameof(FunctionDeclarationSyntax));
                sb.Append(' ');
                sb.AppendLine(f.Identifier.Text);
                WriteType(sb, f.ReturnType, depth + 1);
                WriteParameterList(sb, f.Parameters, depth + 1);
                WriteBlock(sb, f.Body, depth + 1);
                break;
            default:
                Indent(sb, depth);
                sb.AppendLine(m.GetType().Name);
                break;
        }
    }

    private static void WriteParameterList(StringBuilder sb, ParameterListSyntax p, int depth)
    {
        Indent(sb, depth);
        sb.AppendLine(nameof(ParameterListSyntax));
        foreach (var param in p.Parameters)
        {
            Indent(sb, depth + 1);
            sb.Append(param.Identifier.Text);
            sb.Append(": ");
            WriteTypeInline(sb, param.Type);
            sb.AppendLine();
        }
    }

    private static void WriteBlock(StringBuilder sb, BlockStatementSyntax b, int depth)
    {
        Indent(sb, depth);
        sb.AppendLine(nameof(BlockStatementSyntax));
        foreach (var s in b.Statements)
            WriteStatement(sb, s, depth + 1);
    }

    private static void WriteStatement(StringBuilder sb, StatementSyntax s, int depth)
    {
        switch (s)
        {
            case BlockStatementSyntax b:
                WriteBlock(sb, b, depth);
                break;
            case VariableDeclarationStatementSyntax v:
                Indent(sb, depth);
                sb.Append(nameof(VariableDeclarationStatementSyntax));
                sb.Append(' ');
                sb.Append(v.Identifier.Text);
                sb.Append(" = ");
                WriteExpressionInline(sb, v.Initializer);
                sb.AppendLine();
                break;
            case AssignmentStatementSyntax a:
                Indent(sb, depth);
                sb.Append(nameof(AssignmentStatementSyntax));
                sb.Append(' ');
                sb.Append(a.Identifier.Text);
                sb.Append(" = ");
                WriteExpressionInline(sb, a.Expression);
                sb.AppendLine();
                break;
            case ExpressionStatementSyntax e:
                Indent(sb, depth);
                sb.Append(nameof(ExpressionStatementSyntax));
                sb.Append(' ');
                WriteExpressionInline(sb, e.Expression);
                sb.AppendLine();
                break;
            case IfStatementSyntax i:
                Indent(sb, depth);
                sb.AppendLine(nameof(IfStatementSyntax));
                WriteStatement(sb, i.ThenStatement, depth + 1);
                if (i.ElseClause is not null)
                {
                    Indent(sb, depth + 1);
                    sb.AppendLine("else");
                    WriteStatement(sb, i.ElseClause.Statement, depth + 2);
                }
                break;
            case WhileStatementSyntax w:
                Indent(sb, depth);
                sb.AppendLine(nameof(WhileStatementSyntax));
                WriteStatement(sb, w.Body, depth + 1);
                break;
            case ReturnStatementSyntax r:
                Indent(sb, depth);
                sb.Append(nameof(ReturnStatementSyntax));
                if (r.Expression is not null)
                {
                    sb.Append(' ');
                    WriteExpressionInline(sb, r.Expression);
                }

                sb.AppendLine();
                break;
            default:
                Indent(sb, depth);
                sb.AppendLine(s.GetType().Name);
                break;
        }
    }

    private static void WriteType(StringBuilder sb, TypeSyntax t, int depth)
    {
        Indent(sb, depth);
        WriteTypeInline(sb, t);
        sb.AppendLine();
    }

    private static void WriteTypeInline(StringBuilder sb, TypeSyntax t)
    {
        switch (t)
        {
            case PrimitiveTypeSyntax p:
                sb.Append(p.Keyword.Text);
                break;
            case ArrayTypeSyntax a:
                WriteTypeInline(sb, a.ElementType);
                sb.Append("[]");
                break;
            default:
                sb.Append('?');
                break;
        }
    }

    private static void WriteExpressionInline(StringBuilder sb, ExpressionSyntax e)
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
                WriteExpressionInline(sb, u.Operand);
                break;
            case BinaryExpressionSyntax b:
                sb.Append('(');
                WriteExpressionInline(sb, b.Left);
                sb.Append(' ');
                sb.Append(b.OperatorToken.Text);
                sb.Append(' ');
                WriteExpressionInline(sb, b.Right);
                sb.Append(')');
                break;
            case ParenthesizedExpressionSyntax p:
                sb.Append('(');
                WriteExpressionInline(sb, p.Expression);
                sb.Append(')');
                break;
            case CallExpressionSyntax c:
                WriteExpressionInline(sb, c.Callee);
                sb.Append('(');
                for (var i = 0; i < c.Arguments.Count; i++)
                {
                    if (i > 0)
                        sb.Append(", ");
                    WriteExpressionInline(sb, c.Arguments[i]);
                }

                sb.Append(')');
                break;
            case ArrayLiteralExpressionSyntax a:
                sb.Append('[');
                for (var i = 0; i < a.Elements.Count; i++)
                {
                    if (i > 0)
                        sb.Append(", ");
                    WriteExpressionInline(sb, a.Elements[i]);
                }

                sb.Append(']');
                break;
            case IndexExpressionSyntax ix:
                WriteExpressionInline(sb, ix.Target);
                sb.Append('[');
                WriteExpressionInline(sb, ix.Index);
                sb.Append(']');
                break;
            default:
                sb.Append('?');
                break;
        }
    }
}
