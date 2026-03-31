using RoboSharp.IL;
using RoboSharp.Toolchain;

namespace RoboSharp.Toolchain.Tests;

public class RoboExecutableJsonTests
{
    [Test]
    public async Task RoundTrip_Preserves_Entry_And_Instructions()
    {
        var program = new RoboProgram
        {
            StringTable = ["hi"],
            NumberTable = [],
            EntryFunctionIndex = 0,
            Functions =
            [
                new CompiledFunction
                {
                    Name = "TopLevel",
                    ParameterCount = 0,
                    LocalSlotCount = 0,
                    ReturnsVoid = true,
                    Instructions =
                    [
                        new Instruction(RoboOpcode.PushInt, 7),
                        new Instruction(RoboOpcode.Return),
                    ],
                },
            ],
        };

        var ex = RoboExecutable.FromProgram(program);
        var json = RoboExecutableJsonSerializer.Serialize(ex);
        var back = RoboExecutableJsonSerializer.Deserialize(json);

        await Assert.That(back.FormatVersion).IsEqualTo(RoboExecutable.CurrentFormatVersion);
        await Assert.That(back.Program.EntryFunctionIndex).IsEqualTo(0);
        await Assert.That(back.Program.Functions[0].Instructions).HasCount(2);
        await Assert.That(back.Program.Functions[0].Instructions[0].Op).IsEqualTo(RoboOpcode.PushInt);
        await Assert.That(back.Program.Functions[0].Instructions[0].A).IsEqualTo(7);
    }

    [Test]
    public async Task RoundTrip_Preserves_Optional_Instruction_Source_Spans()
    {
        var program = new RoboProgram
        {
            StringTable = [],
            NumberTable = [],
            EntryFunctionIndex = 0,
            Functions =
            [
                new CompiledFunction
                {
                    Name = "TopLevel",
                    ParameterCount = 0,
                    LocalSlotCount = 0,
                    ReturnsVoid = true,
                    Instructions =
                    [
                        new Instruction(RoboOpcode.PushInt, 1, 0, 0, SourceStart: 10, SourceLength: 3),
                        new Instruction(RoboOpcode.Return),
                    ],
                },
            ],
        };

        var ex = RoboExecutable.FromProgram(program);
        var json = RoboExecutableJsonSerializer.Serialize(ex);
        var back = RoboExecutableJsonSerializer.Deserialize(json);

        var i0 = back.Program.Functions[0].Instructions[0];
        await Assert.That(i0.SourceStart).IsEqualTo(10);
        await Assert.That(i0.SourceLength).IsEqualTo(3);
        var i1 = back.Program.Functions[0].Instructions[1];
        await Assert.That(i1.SourceStart).IsEqualTo(-1);
        await Assert.That(i1.SourceLength).IsEqualTo(0);
    }
}
