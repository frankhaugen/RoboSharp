using Avalonia.Controls;
using RoboSharp.Locales;
using RoboSharp.Application.Teaching;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

public sealed class FakeMachineCodePipelinePanel : IStudioPanel
{
    private readonly ITeachingLocale _locale;
    private TextBlock? _lead;
    private TextBlock? _guide;
    private TextBlock? _footer;
    private TextBox? _text;

    public FakeMachineCodePipelinePanel(ITeachingLocale locale) =>
        _locale = locale;

    public string PanelId => StudioPanelIds.FakeMachineCode;

    public int Order => 60;

    public string DisplayName => _locale.Panels.FakeMachineCodeTitle;

    public string? InspectorSubtitle => _locale.Panels.FakeMachineCodeSubtitle;

    public PipelineInspectTier AbstractionTier => PipelineInspectTier.MachineEncoding;

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

        var (root, parts) = TeachingInspectPanelChrome.CreateWithTier(scroll, AbstractionTier, dataMinHeight: 0);
        _lead = parts.Lead;
        _guide = parts.Guide;
        _footer = parts.Footer;
        _lead.Text = _locale.Panels.FakeMachineCodeLead;
        _guide.Text = _locale.Panels.FakeMachineCodeGuide;
        _footer.Text = _locale.Panels.FakeMachineCodeFooter;

        return root;
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        if (_text is null)
            return;

        _text.Text = snapshot.FakeMachineCodeText is { Length: > 0 } body
            ? body
            : _locale.Panels.FakeMachineCodeWaitingForProgram;
    }

    public void ApplyLocale(PipelineSnapshot? lastSnapshot)
    {
        if (_lead is not null)
            _lead.Text = _locale.Panels.FakeMachineCodeLead;
        if (_guide is not null)
            _guide.Text = _locale.Panels.FakeMachineCodeGuide;
        if (_footer is not null)
            _footer.Text = _locale.Panels.FakeMachineCodeFooter;
        if (lastSnapshot is not null)
            OnSnapshotChanged(lastSnapshot);
    }
}
