namespace RoboSharp.Language;

public readonly record struct LinePositionSpan(
    LinePosition Start,
    LinePosition End);
