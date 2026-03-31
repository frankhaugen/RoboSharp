using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using RoboSharp.Application.Teaching;
using RoboSharp.Locales;
using RoboSharp.Studio.Shell;
using RoboSharp.Toolchain;

namespace RoboSharp.Studio.Panels;

public sealed class BoundTreePipelinePanel : IStudioPanel
{
    private static readonly SolidColorBrush StepHighlightBrush =
        new SolidColorBrush(StudioVisual.Accent) { Opacity = 0.22 };

    private readonly ITeachingLocale _locale;
    private TextBlock? _lead;
    private TextBlock? _guide;
    private TextBlock? _footer;
    private TextBox? _fallbackText;
    private Grid? _structuredRoot;
    private ScrollViewer? _listingScroll;
    private StackPanel? _listingHost;
    private readonly List<(Border box, int start, int len)> _stepRows = new();

    public BoundTreePipelinePanel(ITeachingLocale locale) =>
        _locale = locale;

    public string PanelId => StudioPanelIds.BoundTree;

    public int Order => 40;

    public string DisplayName => _locale.Panels.BoundTreeTitle;

    public string? InspectorSubtitle => _locale.Panels.BoundTreeSubtitle;

    public PipelineInspectTier AbstractionTier => PipelineInspectTier.Semantic;

    public Control CreateView()
    {
        _fallbackText = StudioCopyableText.CreateReadOnlyOutput();

        _listingHost = new StackPanel { Spacing = 0 };
        _listingScroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = _listingHost,
            MinHeight = 140,
        };

        _structuredRoot = new Grid();
        _structuredRoot.Children.Add(_listingScroll);

        var layered = new Grid();
        layered.Children.Add(_fallbackText);
        layered.Children.Add(_structuredRoot);

        var (root, parts) = TeachingInspectPanelChrome.CreateWithTier(layered, AbstractionTier, dataMinHeight: 0);
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
        ClearStepHighlight();
        if (_fallbackText is null || _structuredRoot is null || _listingHost is null)
            return;

        var lines = snapshot.BoundTeachingLines;
        if (lines is { Count: > 0 })
        {
            _fallbackText.IsVisible = false;
            _structuredRoot.IsVisible = true;
            _listingHost.Children.Clear();
            _stepRows.Clear();

            foreach (var line in lines)
            {
                var inner = new TextBlock
                {
                    Text = line.Text,
                    FontFamily = StudioVisual.CodeFontFamily,
                    FontSize = 11,
                    Foreground = StudioVisual.TextPrimaryBrush,
                    TextWrapping = TextWrapping.Wrap,
                    Padding = new Thickness(4, 1, 4, 1),
                };
                var box = new Border { Child = inner };
                _listingHost.Children.Add(box);
                if (line.HasSource)
                    _stepRows.Add((box, line.SourceStart, line.SourceLength));
                else
                    _stepRows.Add((box, -1, 0));
            }

            return;
        }

        _structuredRoot.IsVisible = false;
        _fallbackText.IsVisible = true;

        if (snapshot.BoundTreeText is { Length: > 0 } body)
        {
            _fallbackText.Text = body;
            return;
        }

        var note = snapshot.CompileReachedPhase switch
        {
            CompilePhase.Parse => _locale.Panels.BoundTreeNeedParseFirst,
            CompilePhase.Semantics => _locale.Panels.BoundTreeSemanticsStopped,
            CompilePhase.Lowered => _locale.Panels.BoundTreeUnexpectedEmpty,
            _ => _locale.Panels.BoundTreeBuildPrompt,
        };

        _fallbackText.Text = note;
    }

    public void OnRunProgress(StudioRunProgress progress)
    {
        if (_listingHost is null || _structuredRoot is not { IsVisible: true })
            return;

        if (progress.SourceStepStart is not { } s0 || progress.SourceStepLength is not { } ln || ln <= 0)
        {
            foreach (var (box, _, _) in _stepRows)
                box.Background = null;
            return;
        }

        Border? scrollTo = null;
        foreach (var (box, ts, tl) in _stepRows)
        {
            var on = tl > 0 && TeachingPipelineListingLine.SpansOverlap(ts, tl, s0, ln);
            box.Background = on ? StepHighlightBrush : null;
            if (on)
                scrollTo = box;
        }

        scrollTo?.BringIntoView();
    }

    private void ClearStepHighlight()
    {
        foreach (var (box, _, _) in _stepRows)
            box.Background = null;
        _stepRows.Clear();
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
