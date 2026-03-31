namespace RoboSharp.Studio.Pipeline;

/// <summary>One row in the structured IL listing (Studio inspector).</summary>
public readonly record struct IlListingLine(
    IlListingLineKind Kind,
    int FunctionIndex,
    int InstructionIndex,
    string Text);