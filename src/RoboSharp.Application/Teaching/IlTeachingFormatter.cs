using System.Text;
using RoboSharp.IL;
using RoboSharp.Semantics;

namespace RoboSharp.Application.Teaching;

public static class IlTeachingFormatter
{
    /// <summary>Structured rows matching <see cref="Format"/> text layout (for step highlighting).</summary>
    public static IReadOnlyList<IlListingLine> BuildListing(RoboProgram program)
    {
        var list = new List<IlListingLine>
        {
            new(IlListingLineKind.Meta, -1, -1, $"entryFunctionIndex = {program.EntryFunctionIndex}  (starts execution here)"),
            new(IlListingLineKind.Meta, -1, -1, $"stringTable ({program.StringTable.Count}): {string.Join(", ", program.StringTable.Select(Quote))}"),
            new(IlListingLineKind.Meta, -1, -1, $"numberTable ({program.NumberTable.Count}): {string.Join(", ", program.NumberTable)}"),
            new(IlListingLineKind.Meta, -1, -1, string.Empty),
        };

        for (var fi = 0; fi < program.Functions.Count; fi++)
        {
            var fn = program.Functions[fi];
            var mark = fi == program.EntryFunctionIndex ? "  ← entry" : string.Empty;
            var display = fn.Name == CompilationArtifacts.TopLevelStatementsFunctionName
                ? "top-level statements"
                : fn.Name;
            list.Add(new(IlListingLineKind.FunctionHeader, fi, -1, $"--- fn[{fi}] {display}{mark} ---"));
            list.Add(new(IlListingLineKind.Meta, -1, -1, $"    params={fn.ParameterCount} localSlots={fn.LocalSlotCount} returnsVoid={fn.ReturnsVoid}"));
            for (var ip = 0; ip < fn.Instructions.Count; ip++)
            {
                var i = fn.Instructions[ip];
                var line = $"    {ip,4}: {i.Op,-16} A={i.A,4} B={i.B,4} C={i.C,4}";
                list.Add(new(IlListingLineKind.Instruction, fi, ip, line));
            }

            list.Add(new(IlListingLineKind.Meta, -1, -1, string.Empty));
        }

        return list;
    }

    public static string Format(RoboProgram program)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"entryFunctionIndex = {program.EntryFunctionIndex}  (starts execution here)");
        sb.AppendLine($"stringTable ({program.StringTable.Count}): {string.Join(", ", program.StringTable.Select(Quote))}");
        sb.AppendLine($"numberTable ({program.NumberTable.Count}): {string.Join(", ", program.NumberTable)}");
        sb.AppendLine();

        for (var fi = 0; fi < program.Functions.Count; fi++)
        {
            var fn = program.Functions[fi];
            var mark = fi == program.EntryFunctionIndex ? "  ← entry" : string.Empty;
            var display = fn.Name == CompilationArtifacts.TopLevelStatementsFunctionName
                ? "top-level statements"
                : fn.Name;
            sb.AppendLine($"--- fn[{fi}] {display}{mark} ---");
            sb.AppendLine($"    params={fn.ParameterCount} localSlots={fn.LocalSlotCount} returnsVoid={fn.ReturnsVoid}");
            for (var ip = 0; ip < fn.Instructions.Count; ip++)
            {
                var i = fn.Instructions[ip];
                sb.AppendLine($"    {ip,4}: {i.Op,-16} A={i.A,4} B={i.B,4} C={i.C,4}");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string Quote(string s) => $"\"{s.Replace("\"", "\\\"", StringComparison.Ordinal)}\"";
}
