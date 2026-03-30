namespace RoboSharp.IL;

public sealed class CompiledFunction
{
    public required string Name { get; init; }
    public required int ParameterCount { get; init; }
    public required int LocalSlotCount { get; init; }
    public required bool ReturnsVoid { get; init; }
    public required IReadOnlyList<Instruction> Instructions { get; init; }
}
