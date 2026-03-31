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
}
