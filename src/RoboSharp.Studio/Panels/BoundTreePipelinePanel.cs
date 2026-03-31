using Avalonia.Controls;
using RoboSharp.Locales;
using RoboSharp.Application.Teaching;
using RoboSharp.Studio.Shell;
using RoboSharp.Toolchain;

namespace RoboSharp.Studio.Panels;

public sealed class BoundTreePipelinePanel : IStudioPanel
{
    private readonly ITeachingLocale _locale;
    private TextBlock? _lead;
    private TextBlock? _guide;
    private TextBlock? _footer;
    private TextBox? _text;

    public BoundTreePipelinePanel(ITeachingLocale locale) =>
        _locale = locale;

    public string PanelId => StudioPanelIds.BoundTree;

    public int Order => 40;

    public string DisplayName => _locale.Panels.BoundTreeTitle;

    public string? InspectorSubtitle => _locale.Panels.BoundTreeSubtitle;

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
        _lead.Text = _locale.Panels.BoundTreeLead;
        _guide.Text = _locale.Panels.BoundTreeGuide;
        _footer.Text = _locale.Panels.BoundTreeFootnote;

        return root;
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        if (_text is null)
            return;

        if (snapshot.BoundTreeText is { Length: > 0 } body)
        {
            _text.Text = body;
            return;
        }

        var note = snapshot.CompileReachedPhase switch
        {
            CompilePhase.Parse => _locale.Panels.BoundTreeNeedParseFirst,
            CompilePhase.Semantics => _locale.Panels.BoundTreeSemanticsStopped,
            CompilePhase.Lowered => _locale.Panels.BoundTreeUnexpectedEmpty,
            _ => _locale.Panels.BoundTreeBuildPrompt,
        };

        _text.Text = note;
    }

    public void ApplyLocale(PipelineSnapshot? lastSnapshot)
    {
        if (_lead is not null)
            _lead.Text = _locale.Panels.BoundTreeLead;
        if (_guide is not null)
            _guide.Text = _locale.Panels.BoundTreeGuide;
        if (_footer is not null)
            _footer.Text = _locale.Panels.BoundTreeFootnote;
        if (lastSnapshot is not null)
            OnSnapshotChanged(lastSnapshot);
    }
}
