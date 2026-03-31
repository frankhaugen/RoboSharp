using System.Text;
using RoboSharp.Semantics;

namespace RoboSharp.Studio.Pipeline;

/// <summary>Indented bound tree text for the Studio bound-tree panel (teaching, not a full pretty-printer).</summary>
public static class BoundTreeTeachingFormatter
{
    public static string Format(BoundCompilationUnit unit)
    {
        var sb = new StringBuilder();
        AppendLine(sb, 0, $"BoundCompilationUnit  entry: {FormatEntry(unit.EntryPoint)}");
        foreach (var fn in unit.Functions)
            FormatFunction(fn, sb, 1);

        return sb.ToString();
    }

    private static string FormatEntry(FunctionSymbol? entry) =>
        entry is null
            ? "(none)"
            : IsTopLevelEntry(entry)
                ? "top-level statements (your file body runs here first)"
                : entry.Name;

    private static bool IsTopLevelEntry(FunctionSymbol symbol) =>
        symbol.Name == CompilationArtifacts.TopLevelStatementsFunctionName;

    private static void FormatFunction(BoundFunctionDeclaration fn, StringBuilder sb, int depth)
    {
        if (IsTopLevelEntry(fn.Symbol))
        {
            AppendLine(sb, depth, "Your top-level statements (grouped for the interpreter — you did not write a function with this name):");
            FormatBlock(fn.Body, sb, depth + 1);
            return;
        }

        var ps = string.Join(", ", fn.Symbol.Parameters.Select(p => $"{FormatType(p.Type)} {p.Name}"));
        AppendLine(sb, depth, $"Function {fn.Symbol.Name}({ps}): {FormatType(fn.Symbol.ReturnType)}");
        FormatBlock(fn.Body, sb, depth + 1);
    }

    private static void FormatBlock(BoundBlockStatement block, StringBuilder sb, int depth)
    {
        AppendLine(sb, depth, "Block");
        foreach (var s in block.Statements)
            FormatStatement(s, sb, depth + 1);
    }

    private static void FormatStatement(BoundStatement s, StringBuilder sb, int depth)
    {
        switch (s)
        {
            case BoundVariableDeclarationStatement v:
                AppendLine(
                    sb,
                    depth,
                    $"Var {FormatType(v.Symbol.Type)} {v.Symbol.Name} = {FormatExpression(v.Initializer)}");
                break;
            case BoundAssignmentStatement a:
                AppendLine(sb, depth, $"Assign {a.Symbol.Name} = {FormatExpression(a.Expression)}");
                break;
            case BoundExpressionStatement e:
                AppendLine(sb, depth, $"ExprStmt {FormatExpression(e.Expression)}");
                break;
            case BoundIfStatement i:
                AppendLine(sb, depth, $"If {FormatExpression(i.Condition)}");
                FormatStatement(i.ThenStatement, sb, depth + 1);
                if (i.ElseStatement is not null)
                {
                    AppendLine(sb, depth, "Else");
                    FormatStatement(i.ElseStatement, sb, depth + 1);
                }

                break;
            case BoundWhileStatement w:
                AppendLine(sb, depth, $"While {FormatExpression(w.Condition)}");
                FormatStatement(w.Body, sb, depth + 1);
                break;
            case BoundReturnStatement r:
                AppendLine(
                    sb,
                    depth,
                    r.Expression is null ? "Return" : $"Return {FormatExpression(r.Expression)}");
                break;
            case BoundBlockStatement b:
                FormatBlock(b, sb, depth);
                break;
            default:
                AppendLine(sb, depth, $"({s.GetType().Name})");
                break;
        }
    }

    private static string FormatExpression(BoundExpression e) =>
        e switch
        {
            BoundLiteralExpression lit => FormatLiteral(lit),
            BoundVariableExpression v => v.Symbol.Name,
            BoundBinaryExpression b =>
                $"({FormatExpression(b.Left)} {b.Syntax.OperatorToken.Text} {FormatExpression(b.Right)})",
            BoundUnaryExpression u => $"({u.Syntax.OperatorToken.Text}{FormatExpression(u.Operand)})",
            BoundCallExpression c =>
                $"{c.Function.Name}({string.Join(", ", c.Arguments.Select(FormatExpression))})",
            BoundBuiltinCallExpression b =>
                $"{b.Builtin}({string.Join(", ", b.Arguments.Select(FormatExpression))})",
            BoundArrayCreationExpression a =>
                $"[{string.Join(", ", a.Elements.Select(FormatExpression))}]",
            BoundIndexExpression i => $"{FormatExpression(i.Target)}[{FormatExpression(i.Index)}]",
            BoundConversionExpression c => $"({FormatType(c.TargetType)}){FormatExpression(c.Operand)}",
            _ => e.GetType().Name,
        };

    private static string FormatLiteral(BoundLiteralExpression lit)
    {
        if (lit.Value is string s)
            return $"\"{s}\"";
        return lit.Value?.ToString() ?? "null";
    }

    private static string FormatType(TypeSymbol t) =>
        t switch
        {
            PrimitiveTypeSymbol p => p.Kind.ToString().ToLowerInvariant(),
            ArrayTypeSymbol a => $"{FormatType(a.Element)}[]",
            _ => t.ToString() ?? "?",
        };

    private static void AppendLine(StringBuilder sb, int depth, string text)
    {
        sb.Append(' ', depth * 2);
        sb.AppendLine(text);
    }
}
