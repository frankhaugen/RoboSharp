using RoboSharp.World;

namespace RoboSharp.Studio.Pipeline;

/// <summary>Live execution tick for world animation + status line (IL step visualization).</summary>
public readonly record struct StudioRunProgress(
    RobotWorldSnapshot World,
    int InstructionsExecutedSoFar,
    string? InstructionDescription,
    int? IlHighlightFunctionIndex,
    int? IlHighlightInstructionIndex);
