using System.Text;
using RoboSharp.IL;
using RoboSharp.Semantics;

namespace RoboSharp.Studio.Pipeline;

public static class IlTeachingFormatter
{
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
