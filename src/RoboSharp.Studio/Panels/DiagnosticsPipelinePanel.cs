using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

public sealed class DiagnosticsPipelinePanel : IStudioPanel
{
    private ListBox? _list;

    public int Order => 30;

    public string DisplayName => "Diagnostics";

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

        if (snapshot.Diagnostics.Count == 0)
        {
            _list.ItemsSource = new[] { "No parse diagnostics." };
            return;
        }

        _list.ItemsSource = snapshot.Diagnostics
            .Select(d => $"@{d.Span.Start}:{d.Span.Length}  {d.Message}")
            .ToList();
    }
}
