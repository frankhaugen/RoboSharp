using Avalonia;
using Avalonia.Controls;
using RoboSharp.Language;
using RoboSharp.Locales;
using RoboSharp.Application.Teaching;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

public sealed class SyntaxTreePipelinePanel : IStudioPanel
{
    private readonly ISyntaxTreeSerializer _serializer;
    private readonly ITeachingLocale _locale;
    private TextBox? _text;

    public SyntaxTreePipelinePanel(ISyntaxTreeSerializer serializer, ITeachingLocale locale)
    {
        _serializer = serializer;
        _locale = locale;
    }

    public string PanelId => StudioPanelIds.SyntaxTree;

    public int Order => 20;

    public string DisplayName => _locale.Panels.SyntaxTreeTitle;

    public string? InspectorSubtitle => _locale.Panels.SyntaxTreeSubtitle;

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

        _text.Text = _locale.Panels.SyntaxTreePreamble + _serializer.Serialize(snapshot.SyntaxTree.Root);
    }
}
