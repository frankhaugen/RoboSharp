using RoboSharp.World;

namespace RoboSharp.Application.Teaching;

/// <summary>Live execution tick for world animation + IL step visualization.</summary>
public readonly record struct StudioRunProgress(
    RobotWorldSnapshot World,
    int InstructionsExecutedSoFar,
    string? InstructionDescription,
    int? IlHighlightFunctionIndex,
    int? IlHighlightInstructionIndex);
