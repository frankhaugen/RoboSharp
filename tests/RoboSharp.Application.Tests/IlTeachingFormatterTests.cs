using RoboSharp.Application.Teaching;
using RoboSharp.IL;

namespace RoboSharp.Application.Tests;

public sealed class IlTeachingFormatterTests
{
    [Test]
    public async Task Format_WhileShapedBytecode_AnnotatesLoopHeadAndJumps()
    {
        // Mirrors IlLowerer output for: while (cond) { move(); } then more stmts
        var program = new RoboProgram
        {
            EntryFunctionIndex = 0,
            StringTable = [],
            NumberTable = [],
            Functions =
            [
                new CompiledFunction
                {
                    Name = "<top-level statements>",
                    ParameterCount = 0,
                    LocalSlotCount = 0,
                    ReturnsVoid = true,
                    Instructions =
                    [
                        new Instruction(RoboOpcode.CallBuiltin, 5, 0, 0),
                        new Instruction(RoboOpcode.JumpIfFalse, 4, 0, 0),
                        new Instruction(RoboOpcode.CallBuiltin, 0, 0, 0),
                        new Instruction(RoboOpcode.Jump, 0, 0, 0),
                        new Instruction(RoboOpcode.CallBuiltin, 1, 0, 0),
                        new Instruction(RoboOpcode.CallBuiltin, 0, 0, 0),
                        new Instruction(RoboOpcode.Return, 0, 0, 0),
                    ],
                },
            ],
        };

        var text = IlTeachingFormatter.Format(program);

        await Assert.That(text).Contains("← loop head (while condition checked here)");
        await Assert.That(text).Contains("exit while");
        await Assert.That(text).Contains("loop again (back to condition)");
        await Assert.That(text).Contains("target instruction index");
    }
}
