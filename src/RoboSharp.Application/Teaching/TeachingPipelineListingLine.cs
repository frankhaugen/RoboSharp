namespace RoboSharp.Application.Teaching;

/// <summary>One inspectable row with optional source association for run-step highlighting.</summary>
public readonly record struct TeachingPipelineListingLine(
    string Text,
    int SourceStart = -1,
    int SourceLength = 0)
{
    public bool HasSource => SourceStart >= 0 && SourceLength > 0;

    public static bool SpansOverlap(int aStart, int aLen, int bStart, int bLen) =>
        aLen > 0 && bLen > 0 && aStart >= 0 && bStart >= 0 &&
        aStart < bStart + bLen && bStart < aStart + aLen;
}
