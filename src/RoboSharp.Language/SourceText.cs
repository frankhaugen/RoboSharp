using System.Collections.Generic;

namespace RoboSharp.Language;

public sealed class SourceText
{
    private readonly TextLine[] _lines;

    private SourceText(string text, TextLine[] lines)
    {
        Text = text;
        _lines = lines;
    }

    public string Text { get; }

    public IReadOnlyList<TextLine> Lines => _lines;

    public static SourceText From(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return new SourceText(text, BuildLines(text));
    }

    private static TextLine[] BuildLines(string text)
    {
        var lines = new List<TextLine>();
        int lineStart = 0;
        int lineNumber = 0;
        int i = 0;
        int n = text.Length;

        while (i < n)
        {
            char c = text[i];
            if (c == '\r' && i + 1 < n && text[i + 1] == '\n')
            {
                lines.Add(new TextLine(lineStart, i + 2 - lineStart, lineNumber));
                lineNumber++;
                i += 2;
                lineStart = i;
            }
            else if (c == '\n')
            {
                lines.Add(new TextLine(lineStart, i + 1 - lineStart, lineNumber));
                lineNumber++;
                i++;
                lineStart = i;
            }
            else if (c == '\r')
            {
                lines.Add(new TextLine(lineStart, i + 1 - lineStart, lineNumber));
                lineNumber++;
                i++;
                lineStart = i;
            }
            else
            {
                i++;
            }
        }

        if (lineStart < n)
        {
            lines.Add(new TextLine(lineStart, n - lineStart, lineNumber));
        }
        else if (lines.Count > 0)
        {
            lines.Add(new TextLine(lineStart, 0, lineNumber));
        }
        else
        {
            lines.Add(new TextLine(0, 0, 0));
        }

        return lines.ToArray();
    }
}
