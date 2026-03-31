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
    private TextBlock? _lead;
    private TextBlock? _guide;
    private TextBlock? _footer;
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
        var scroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            Content = _text,
            MinHeight = 140,
        };

        var (root, parts) = TeachingInspectPanelChrome.Create(scroll, dataMinHeight: 0);
        _lead = parts.Lead;
        _guide = parts.Guide;
        _footer = parts.Footer;
        _lead.Text = _locale.Panels.SyntaxTreeLead;
        _guide.Text = _locale.Panels.SyntaxTreeGuide;
        _footer.Text = _locale.Panels.SyntaxTreeFootnote;

        return root;
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        if (_text is null)
            return;

        _text.Text = _serializer.Serialize(snapshot.SyntaxTree.Root);
    }

    public void ApplyLocale(PipelineSnapshot? lastSnapshot)
    {
        if (_lead is not null)
            _lead.Text = _locale.Panels.SyntaxTreeLead;
        if (_guide is not null)
            _guide.Text = _locale.Panels.SyntaxTreeGuide;
        if (_footer is not null)
            _footer.Text = _locale.Panels.SyntaxTreeFootnote;
        if (lastSnapshot is not null)
            OnSnapshotChanged(lastSnapshot);
    }
}
