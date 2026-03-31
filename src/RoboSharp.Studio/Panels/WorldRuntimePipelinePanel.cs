using Avalonia.Controls;
using RoboSharp.Locales;
using RoboSharp.Application.Teaching;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

public sealed class WorldRuntimePipelinePanel : IStudioPanel
{
    private readonly ITeachingLocale _locale;
    private TextBlock? _lead;
    private TextBlock? _guide;
    private TextBlock? _footer;
    private TextBox? _text;

    public WorldRuntimePipelinePanel(ITeachingLocale locale) =>
        _locale = locale;

    public string PanelId => StudioPanelIds.WorldRuntime;

    public int Order => 70;

    public string DisplayName => _locale.Panels.WorldRuntimeTitle;

    public string? InspectorSubtitle => null;

    public PipelineInspectTier AbstractionTier => PipelineInspectTier.RuntimeSummary;

    public Control CreateView()
    {
        _text = StudioCopyableText.CreateReadOnlyOutput();
        var scroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            Content = _text,
            MinHeight = 160,
        };

        var (root, parts) = TeachingInspectPanelChrome.CreateWithTier(scroll, AbstractionTier, dataMinHeight: 0);
        _lead = parts.Lead;
        _guide = parts.Guide;
        _footer = parts.Footer;
        _lead.Text = _locale.Panels.WorldRuntimeLead;
        _guide.Text = _locale.Panels.WorldRuntimeSubtitle;
        _footer.Text = _locale.Panels.WorldRuntimeFooter;

        return root;
    }

    public void ApplyLocale(PipelineSnapshot? lastSnapshot)
    {
        if (_lead is not null)
            _lead.Text = _locale.Panels.WorldRuntimeLead;
        if (_guide is not null)
            _guide.Text = _locale.Panels.WorldRuntimeSubtitle;
        if (_footer is not null)
            _footer.Text = _locale.Panels.WorldRuntimeFooter;
        if (lastSnapshot is not null)
            OnSnapshotChanged(lastSnapshot);
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        if (_text is null)
            return;

        _text.Text = _locale.Panels.FormatWorldRuntimePanel(
            snapshot.RuntimeSucceeded,
            snapshot.IlDisassemblyText is { Length: > 0 },
            snapshot.LessonOutcomeSummary,
            snapshot.LessonScore,
            snapshot.WorldAfterRunSummary,
            snapshot.RuntimeFaultMessage,
            snapshot.RuntimeStdout,
            snapshot.RuntimeStderr);
    }
}
