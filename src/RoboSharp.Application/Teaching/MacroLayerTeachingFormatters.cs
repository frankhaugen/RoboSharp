using System.Text;
using RoboSharp.IL;
using RoboSharp.Semantics;

namespace RoboSharp.Application.Teaching;

/// <summary>Teaching-only views below IL: mnemonic assembly and fake machine words (not CLR, not a real ISA).</summary>
public static class MacroLayerTeachingFormatters
{
    /// <summary>Structured SharpAssembly rows; instruction lines share fi/ip with the IL panel for stepping.</summary>
    public static IReadOnlyList<IlListingLine> BuildSharpAssemblyListing(RoboProgram program)
    {
        var list = new List<IlListingLine>
        {
            new(IlListingLineKind.Meta, -1, -1,
                "; SharpAssembly — teaching mnemonics derived from RoboSharp IL (not .NET IL, not a real CPU)."),
            new(IlListingLineKind.Meta, -1, -1,
                "; Same program as the IL panel, reshaped to look like assembly language."),
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
            list.Add(new(IlListingLineKind.Meta, -1, -1,
                $"    params={fn.ParameterCount} locals={fn.LocalSlotCount} void={fn.ReturnsVoid}"));
            for (var ip = 0; ip < fn.Instructions.Count; ip++)
            {
                var insn = fn.Instructions[ip];
                var line = $"    {ip,4}:  {FormatSharpLine(insn)}";
                list.Add(new(IlListingLineKind.Instruction, fi, ip, line, insn.SourceStart, insn.SourceLength));
            }

            list.Add(new(IlListingLineKind.Meta, -1, -1, string.Empty));
        }

        return list;
    }

    /// <summary>Structured fake machine rows; instruction lines share fi/ip with the IL panel.</summary>
    public static IReadOnlyList<IlListingLine> BuildFakeMachineListing(RoboProgram program)
    {
        var list = new List<IlListingLine>
        {
            new(IlListingLineKind.Meta, -1, -1,
                "; Machine words — synthetic 32-bit encodings of opcode + operands (not x86, ARM, RISC-V, or CLR)."),
            new(IlListingLineKind.Meta, -1, -1,
                "; Same instructions as IL; hex is only for “what machine code might feel like” in the classroom."),
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
            for (var ip = 0; ip < fn.Instructions.Count; ip++)
            {
                var insn = fn.Instructions[ip];
                var w = EncodeTeachingWord(insn, ip, fi);
                var line =
                    $"    {ip,4}:  {w:X8}    {insn.Op,-16}  A={insn.A,4} B={insn.B,4} C={insn.C,4}";
                list.Add(new(IlListingLineKind.Instruction, fi, ip, line, insn.SourceStart, insn.SourceLength));
            }

            list.Add(new(IlListingLineKind.Meta, -1, -1, string.Empty));
        }

        return list;
    }

    /// <summary>SharpAssembly: one readable line per IL instruction (mnemonics + operands).</summary>
    public static string FormatSharpAssembly(RoboProgram program)
    {
        var sb = new StringBuilder();
        sb.AppendLine(
            "; SharpAssembly — teaching mnemonics derived from RoboSharp IL (not .NET IL, not a real CPU).");
        sb.AppendLine("; Same program as the IL panel, reshaped to look like assembly language.");
        sb.AppendLine();

        for (var fi = 0; fi < program.Functions.Count; fi++)
        {
            var fn = program.Functions[fi];
            var mark = fi == program.EntryFunctionIndex ? "  ← entry" : string.Empty;
            var display = fn.Name == CompilationArtifacts.TopLevelStatementsFunctionName
                ? "top-level statements"
                : fn.Name;
            sb.AppendLine($"--- fn[{fi}] {display}{mark} ---");
            sb.AppendLine($"    params={fn.ParameterCount} locals={fn.LocalSlotCount} void={fn.ReturnsVoid}");
            for (var ip = 0; ip < fn.Instructions.Count; ip++)
                sb.AppendLine($"    {ip,4}:  {FormatSharpLine(fn.Instructions[ip])}");

            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>Fake 32-bit words + disassembly hint (deterministic encoding for teaching only).</summary>
    public static string FormatFakeMachineCode(RoboProgram program)
    {
        var sb = new StringBuilder();
        sb.AppendLine(
            "; Machine words — synthetic 32-bit encodings of opcode + operands (not x86, ARM, RISC-V, or CLR).");
        sb.AppendLine("; Same instructions as IL; hex is only for “what machine code might feel like” in the classroom.");
        sb.AppendLine();

        for (var fi = 0; fi < program.Functions.Count; fi++)
        {
            var fn = program.Functions[fi];
            var mark = fi == program.EntryFunctionIndex ? "  ← entry" : string.Empty;
            var display = fn.Name == CompilationArtifacts.TopLevelStatementsFunctionName
                ? "top-level statements"
                : fn.Name;
            sb.AppendLine($"--- fn[{fi}] {display}{mark} ---");
            for (var ip = 0; ip < fn.Instructions.Count; ip++)
            {
                var insn = fn.Instructions[ip];
                var w = EncodeTeachingWord(insn, ip, fi);
                sb.AppendLine(
                    $"    {ip,4}:  {w:X8}    {insn.Op,-16}  A={insn.A,4} B={insn.B,4} C={insn.C,4}");
            }

            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }

    private static uint EncodeTeachingWord(Instruction i, int ip, int fnIndex)
    {
        unchecked
        {
            var op = (uint)(byte)i.Op;
            var a = (uint)(i.A & 0xFFFF);
            var b = (uint)(i.B & 0xFF);
            var c = (uint)(i.C & 0xFF);
            var mix = (uint)(ip * 0x9E37_79B9) ^ (uint)(fnIndex * 0x85EB_CA6B);
            return (op << 24) | ((a & 0xFFF) << 12) | (b << 8) | c ^ mix;
        }
    }

    private static string FormatSharpLine(Instruction i)
    {
        return i.Op switch
        {
            RoboOpcode.Nop => "nop",
            RoboOpcode.Pop => "pop",
            RoboOpcode.Dup => "dup",
            RoboOpcode.PushInt => $"push.int  {i.A}",
            RoboOpcode.PushBool => $"push.bool  {i.A}",
            RoboOpcode.PushNumber => $"push.num  #{i.A}",
            RoboOpcode.PushString => $"push.str  #{i.A}",
            RoboOpcode.LoadLocal => $"ldloc  {i.A}",
            RoboOpcode.StoreLocal => $"stloc  {i.A}",
            RoboOpcode.Add => "add",
            RoboOpcode.Sub => "sub",
            RoboOpcode.Mul => "mul",
            RoboOpcode.Div => "div",
            RoboOpcode.Neg => "neg",
            RoboOpcode.EqInt => "ceq.int",
            RoboOpcode.NeInt => "cne.int",
            RoboOpcode.Lt => "clt",
            RoboOpcode.Le => "cle",
            RoboOpcode.Gt => "cgt",
            RoboOpcode.Ge => "cge",
            RoboOpcode.And => "and",
            RoboOpcode.Or => "or",
            RoboOpcode.Not => "not",
            RoboOpcode.Jump => $"jmp  →{i.A}",
            RoboOpcode.JumpIfFalse => $"brfalse  →{i.A}",
            RoboOpcode.JumpIfTrue => $"brtrue  →{i.A}",
            RoboOpcode.Call => $"call  fn#{i.A}  args={i.B}",
            RoboOpcode.CallBuiltin => $"sys.call  builtin#{i.A}  mode={i.B}",
            RoboOpcode.Return => "ret",
            RoboOpcode.NewIntArray => "newarr.int",
            RoboOpcode.ArrayLen => "len",
            RoboOpcode.ArrayGet => "ldelem",
            RoboOpcode.ArraySet => "stelem",
            RoboOpcode.ConvNumberToInt => "conv.i4",
            _ => $"{i.Op}  A={i.A} B={i.B} C={i.C}",
        };
    }
}
