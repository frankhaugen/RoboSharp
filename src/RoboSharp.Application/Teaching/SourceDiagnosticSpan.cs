namespace RoboSharp.Application.Teaching;

/// <summary>Character span in source for parse/semantic error highlighting.</summary>
public readonly record struct SourceDiagnosticSpan(int Start, int Length);
