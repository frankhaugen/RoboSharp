using RoboSharp.Language;

namespace RoboSharp.Studio.Pipeline;

public static class SourceLocationFormatter
{
    public static string FormatLine(string source, TextSpan span)
    {
        if (string.IsNullOrEmpty(source) || span.Start < 0 || span.Start > source.Length)
            return "line ?";

        var line = 1;
        var col = 1;
        for (var i = 0; i < span.Start; i++)
        {
            if (source[i] == '\n')
            {
                line++;
                col = 1;
            }
            else
                col++;
        }

        return $"line {line}, col {col}";
    }
}
