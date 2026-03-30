using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

public sealed class TokenPipelinePanel : IStudioPanel
{
    private ListBox? _list;

    public int Order => 10;

    public string DisplayName => "Tokens";

    public Control CreateView()
    {
        _list = new ListBox
        {
            FontFamily = StudioVisual.CodeFontFamily,
            FontSize = 12,
            Background = Brushes.Transparent,
            Foreground = StudioVisual.TextPrimaryBrush,
        };

        return new Border
        {
            Padding = new Thickness(8),
            Child = _list,
        };
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        if (_list is null)
            return;

        var rows = snapshot.Tokens.Select(t =>
            $"{t.Kind,-22}  @{t.Span.Start,4} len {t.Span.Length,3}  {VisualizeText(t.Text)}");

        _list.ItemsSource = rows.ToList();
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
