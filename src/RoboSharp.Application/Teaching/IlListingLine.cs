namespace RoboSharp.Application.Teaching;

/// <summary>One row in the structured IL listing (teaching hosts).</summary>
public readonly record struct IlListingLine(
    IlListingLineKind Kind,
    int FunctionIndex,
    int InstructionIndex,
    string Text);
