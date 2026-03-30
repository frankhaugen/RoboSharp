using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using RoboSharp.Language;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

public sealed class SyntaxTreePipelinePanel : IStudioPanel
{
    private readonly ISyntaxTreeSerializer _serializer;
    private ScrollViewer? _scroll;
    private TextBlock? _text;

    public SyntaxTreePipelinePanel(ISyntaxTreeSerializer serializer) =>
        _serializer = serializer;

    public int Order => 20;

    public string DisplayName => "Syntax tree";

    public Control CreateView()
    {
        _text = new TextBlock
        {
            FontFamily = StudioVisual.CodeFontFamily,
            FontSize = 12,
            Foreground = StudioVisual.TextPrimaryBrush,
            TextWrapping = TextWrapping.Wrap,
        };

        _scroll = new ScrollViewer
        {
            Content = _text,
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
        };

        return new Border
        {
            Padding = new Thickness(8),
            Child = _scroll,
        };
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        if (_text is null)
            return;

        _text.Text = _serializer.Serialize(snapshot.SyntaxTree.Root);
    }
}
