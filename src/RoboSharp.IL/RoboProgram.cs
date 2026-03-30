namespace RoboSharp.IL;

public sealed class RoboProgram
{
    public required IReadOnlyList<string> StringTable { get; init; }
    public required IReadOnlyList<double> NumberTable { get; init; }
    public required IReadOnlyList<CompiledFunction> Functions { get; init; }
    public required int EntryFunctionIndex { get; init; }
}
