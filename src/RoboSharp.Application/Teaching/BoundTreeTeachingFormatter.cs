using System.Text;
using RoboSharp.Language;
using RoboSharp.Semantics;

namespace RoboSharp.Application.Teaching;

/// <summary>Indented bound tree text for teaching panels (not a full pretty-printer).</summary>
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

    /// <summary>Structured rows aligned with <see cref="Format"/> for run-step highlighting.</summary>
    public static IReadOnlyList<TeachingPipelineListingLine> BuildListing(BoundCompilationUnit unit)
    {
        var list = new List<TeachingPipelineListingLine>();
        AddLine(list, 0, $"BoundCompilationUnit  entry: {FormatEntry(unit.EntryPoint)}", TextSpan.Invalid);
        foreach (var fn in unit.Functions)
            AppendFunctionListing(fn, list, 1);

        return list;
    }

    private static void AddLine(List<TeachingPipelineListingLine> list, int depth, string text, TextSpan span)
    {
        var sb = new StringBuilder(depth * 2 + text.Length);
        sb.Append(' ', depth * 2);
        sb.Append(text);
        list.Add(new TeachingPipelineListingLine(
            sb.ToString(),
            span.IsValid ? span.Start : -1,
            span.IsValid ? span.Length : 0));
    }

    private static void AppendFunctionListing(BoundFunctionDeclaration fn, List<TeachingPipelineListingLine> list, int depth)
    {
        if (IsTopLevelEntry(fn.Symbol))
        {
            AddLine(
                list,
                depth,
                "Your top-level statements (grouped for the interpreter — you did not write a function with this name):",
                TextSpan.Invalid);
            AppendBlockListing(fn.Body, list, depth + 1);
            return;
        }

        var ps = string.Join(", ", fn.Symbol.Parameters.Select(p => $"{FormatType(p.Type)} {p.Name}"));
        AddLine(
            list,
            depth,
            $"Function {fn.Symbol.Name}({ps}): {FormatType(fn.Symbol.ReturnType)}",
            TextSpan.Invalid);
        AppendBlockListing(fn.Body, list, depth + 1);
    }

    private static void AppendBlockListing(BoundBlockStatement block, List<TeachingPipelineListingLine> list, int depth)
    {
        AddLine(list, depth, "Block", BoundTeachingSyntaxSpan.ForStatement(block));
        foreach (var s in block.Statements)
            AppendStatementListing(s, list, depth + 1);
    }

    private static void AppendStatementListing(BoundStatement s, List<TeachingPipelineListingLine> list, int depth)
    {
        var span = BoundTeachingSyntaxSpan.ForStatement(s);
        switch (s)
        {
            case BoundVariableDeclarationStatement v:
                AddLine(
                    list,
                    depth,
                    $"Var {FormatType(v.Symbol.Type)} {v.Symbol.Name} = {FormatExpression(v.Initializer)}",
                    span);
                break;
            case BoundAssignmentStatement a:
                AddLine(list, depth, $"Assign {a.Symbol.Name} = {FormatExpression(a.Expression)}", span);
                break;
            case BoundExpressionStatement e:
                AddLine(list, depth, $"ExprStmt {FormatExpression(e.Expression)}", span);
                break;
            case BoundIfStatement i:
                AddLine(list, depth, $"If {FormatExpression(i.Condition)}", span);
                AppendStatementListing(i.ThenStatement, list, depth + 1);
                if (i.ElseStatement is not null)
                {
                    AddLine(list, depth, "Else", BoundTeachingSyntaxSpan.ForStatement(i.ElseStatement));
                    AppendStatementListing(i.ElseStatement, list, depth + 1);
                }

                break;
            case BoundWhileStatement w:
                AddLine(list, depth, $"While {FormatExpression(w.Condition)}", span);
                AppendStatementListing(w.Body, list, depth + 1);
                break;
            case BoundReturnStatement r:
                AddLine(
                    list,
                    depth,
                    r.Expression is null ? "Return" : $"Return {FormatExpression(r.Expression)}",
                    span);
                break;
            case BoundBlockStatement b:
                AppendBlockListing(b, list, depth);
                break;
            default:
                AddLine(list, depth, $"({s.GetType().Name})", TextSpan.Invalid);
                break;
        }
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
