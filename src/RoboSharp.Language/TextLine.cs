namespace RoboSharp.Language;

public readonly record struct TextLine(
    int Start,
    int Length,
    int LineNumber);
