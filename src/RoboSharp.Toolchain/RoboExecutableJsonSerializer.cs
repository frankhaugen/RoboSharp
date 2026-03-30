using System.Text.Json;
using System.Text.Json.Serialization;
using RoboSharp.IL;

namespace RoboSharp.Toolchain;

/// <summary>JSON v1 encoding for <see cref="RoboExecutable"/> (teaching-friendly <c>.roboexe</c> interchange).</summary>
public static class RoboExecutableJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public static string Serialize(RoboExecutable executable)
    {
        ArgumentNullException.ThrowIfNull(executable);
        var dto = ToDto(executable);
        return JsonSerializer.Serialize(dto, Options);
    }

    public static RoboExecutable Deserialize(string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        var dto = JsonSerializer.Deserialize<RoboExecutableDto>(json, Options)
                  ?? throw new JsonException("Empty document.");
        return FromDto(dto);
    }

    private static RoboExecutableDto ToDto(RoboExecutable ex)
    {
        var p = ex.Program;
        return new RoboExecutableDto
        {
            FormatVersion = ex.FormatVersion,
            EntryFunctionIndex = p.EntryFunctionIndex,
            StringTable = p.StringTable.ToList(),
            NumberTable = p.NumberTable.ToList(),
            Functions = p.Functions.Select(f => new CompiledFunctionDto
            {
                Name = f.Name,
                ParameterCount = f.ParameterCount,
                LocalSlotCount = f.LocalSlotCount,
                ReturnsVoid = f.ReturnsVoid,
                Instructions = f.Instructions.Select(i => new InstructionDto
                {
                    Op = i.Op.ToString(),
                    A = i.A,
                    B = i.B,
                    C = i.C,
                }).ToList(),
            }).ToList(),
        };
    }

    private static RoboExecutable FromDto(RoboExecutableDto dto)
    {
        var functions = dto.Functions.Select(f => new CompiledFunction
        {
            Name = f.Name,
            ParameterCount = f.ParameterCount,
            LocalSlotCount = f.LocalSlotCount,
            ReturnsVoid = f.ReturnsVoid,
            Instructions = f.Instructions.Select(i => new Instruction(
                Enum.Parse<RoboOpcode>(i.Op, ignoreCase: true),
                i.A,
                i.B,
                i.C)).ToList(),
        }).ToList();

        var program = new RoboProgram
        {
            StringTable = dto.StringTable,
            NumberTable = dto.NumberTable,
            Functions = functions,
            EntryFunctionIndex = dto.EntryFunctionIndex,
        };

        return new RoboExecutable
        {
            FormatVersion = dto.FormatVersion,
            Program = program,
        };
    }

    private sealed class RoboExecutableDto
    {
        public int FormatVersion { get; set; }
        public int EntryFunctionIndex { get; set; }
        public List<string> StringTable { get; set; } = [];
        public List<double> NumberTable { get; set; } = [];
        public List<CompiledFunctionDto> Functions { get; set; } = [];
    }

    private sealed class CompiledFunctionDto
    {
        public string Name { get; set; } = "";
        public int ParameterCount { get; set; }
        public int LocalSlotCount { get; set; }
        public bool ReturnsVoid { get; set; }
        public List<InstructionDto> Instructions { get; set; } = [];
    }

    private sealed class InstructionDto
    {
        public string Op { get; set; } = "";
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
    }
}
