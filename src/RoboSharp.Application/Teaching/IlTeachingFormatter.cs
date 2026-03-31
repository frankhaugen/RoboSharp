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
            list.Add(new(IlListingLineKind.Meta, -1, -1,
                "    A on Jump / JumpIfFalse / JumpIfTrue = target instruction index in this function (left column)."));
            var loopHeads = ComputeLoopHeadIndices(fn.Instructions);
            for (var ip = 0; ip < fn.Instructions.Count; ip++)
            {
                var i = fn.Instructions[ip];
                var line = FormatInstructionLine(ip, i, loopHeads);
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
            sb.AppendLine(
                "    A on Jump / JumpIfFalse / JumpIfTrue = target instruction index in this function (left column).");
            var loopHeads = ComputeLoopHeadIndices(fn.Instructions);
            for (var ip = 0; ip < fn.Instructions.Count; ip++)
            {
                var i = fn.Instructions[ip];
                sb.AppendLine(FormatInstructionLine(ip, i, loopHeads));
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    /// <summary>IPs jumped to by a backward <see cref="RoboOpcode.Jump"/> — typically the while-condition head.</summary>
    private static HashSet<int> ComputeLoopHeadIndices(IReadOnlyList<Instruction> code)
    {
        var heads = new HashSet<int>();
        for (var j = 0; j < code.Count; j++)
        {
            var ins = code[j];
            if (ins.Op == RoboOpcode.Jump && ins.A >= 0 && ins.A < j)
                heads.Add(ins.A);
        }

        return heads;
    }

    private static string FormatInstructionLine(int ip, Instruction i, HashSet<int> loopHeads)
    {
        var baseLine = $"    {ip,4}: {i.Op,-16} A={i.A,4} B={i.B,4} C={i.C,4}";
        var suffix = new StringBuilder();
        if (loopHeads.Contains(ip))
            suffix.Append("  ← loop head (while condition checked here)");
        suffix.Append(FormatJumpTeachingNote(i.Op, i.A, ip));
        return baseLine + suffix;
    }

    /// <summary>Explains jump operands; lowering is correct but raw A= looks opaque without this.</summary>
    private static string FormatJumpTeachingNote(RoboOpcode op, int targetIp, int currentIp)
    {
        return op switch
        {
            RoboOpcode.Jump when targetIp < currentIp =>
                $"  ; goto @{targetIp} — loop again (back to condition)",
            RoboOpcode.Jump =>
                $"  ; goto @{targetIp}",
            RoboOpcode.JumpIfFalse =>
                $"  ; if popped bool is false → @{targetIp} (exit while); if true → next line (run body)",
            RoboOpcode.JumpIfTrue =>
                $"  ; if popped bool is true → @{targetIp}; else → next line",
            _ => string.Empty,
        };
    }

    private static string Quote(string s) => $"\"{s.Replace("\"", "\\\"", StringComparison.Ordinal)}\"";
}
