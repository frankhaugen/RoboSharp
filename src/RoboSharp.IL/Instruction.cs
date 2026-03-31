namespace RoboSharp.IL;

/// <param name="SourceStart">Source text start offset for teaching stepping; <c>-1</c> if unknown.</param>
/// <param name="SourceLength">Length paired with <paramref name="SourceStart"/>.</param>
public readonly record struct Instruction(
    RoboOpcode Op,
    int A = 0,
    int B = 0,
    int C = 0,
    int SourceStart = -1,
    int SourceLength = 0);
