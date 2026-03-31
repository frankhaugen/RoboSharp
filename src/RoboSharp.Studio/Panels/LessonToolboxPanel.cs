using Avalonia.Controls;
using RoboSharp.Locales;
using RoboSharp.Application.Teaching;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

/// <summary>Shows which built-ins the active lesson profile allows — like a tiny cheat sheet for kids.</summary>
public sealed class LessonToolboxPanel : IStudioPanel
{
    private readonly ITeachingLocale _locale;
    private TextBlock? _lead;
    private TextBlock? _guide;
    private TextBlock? _footer;
    private TextBox? _text;

    public LessonToolboxPanel(ITeachingLocale locale) =>
        _locale = locale;

    public string PanelId => StudioPanelIds.LessonToolbox;

    public int Order => 7;

    public string DisplayName => _locale.Panels.LessonToolboxTitle;

    public string? InspectorSubtitle => _locale.Panels.LessonToolboxSubtitle;

    public Control CreateView()
    {
        _text = StudioCopyableText.CreateReadOnlyOutput();
        var scroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            Content = _text,
            MinHeight = 120,
        };

        var (root, parts) = TeachingInspectPanelChrome.Create(scroll, dataMinHeight: 0);
        _lead = parts.Lead;
        _guide = parts.Guide;
        _footer = parts.Footer;
        _lead.Text = _locale.Panels.LessonToolboxLead;
        _guide.Text = _locale.Panels.LessonToolboxGuide;
        _footer.Text = _locale.Panels.LessonToolboxFooter;

        return root;
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        if (_text is null)
            return;

        if (snapshot.LessonProfileHelpText is { Length: > 0 } help)
        {
            _text.Text = help;
            return;
        }

        _text.Text = _locale.Panels.LessonToolboxBuildPrompt;
    }

    public void ApplyLocale(PipelineSnapshot? lastSnapshot)
    {
        if (_lead is not null)
            _lead.Text = _locale.Panels.LessonToolboxLead;
        if (_guide is not null)
            _guide.Text = _locale.Panels.LessonToolboxGuide;
        if (_footer is not null)
            _footer.Text = _locale.Panels.LessonToolboxFooter;
        if (lastSnapshot is not null)
            OnSnapshotChanged(lastSnapshot);
    }
}
