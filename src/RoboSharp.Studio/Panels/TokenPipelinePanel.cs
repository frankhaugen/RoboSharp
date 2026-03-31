using Avalonia;
using Avalonia.Controls;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

public sealed class TokenPipelinePanel : IStudioPanel
{
    private TextBox? _text;

    public int Order => 10;

    public string DisplayName => "Tokens";

    public string? InspectorSubtitle =>
        "Lexer output: one line per token (kind, source index, length, escaped text). Click inside, then Ctrl+A / Ctrl+C to copy.";

    public Control CreateView()
    {
        _text = StudioCopyableText.CreateReadOnlyOutput();

        return new Border
        {
            Padding = new Thickness(8),
            Child = _text,
        };
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        if (_text is null)
            return;

        const string preamble =
            "# Lexer tokens (output of lexical analysis)\r\n" +
            "Each line is one token from your source before parsing: kind, @start index, length in characters, and token text (escape sequences shown as \\r, \\n, \\t).\r\n" +
            "\r\n";

        var rows = snapshot.Tokens.Select(t =>
            $"{t.Kind,-22}  @{t.Span.Start,4} len {t.Span.Length,3}  {VisualizeText(t.Text)}");

        _text.Text = preamble + string.Join(Environment.NewLine, rows);
    }

    private static string VisualizeText(string text)
    {
        if (text.Length == 0)
            return "∅";
        return text
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal)
            .Replace("\t", "\\t", StringComparison.Ordinal);
    }
}
