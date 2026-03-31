using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using RoboSharp.Application.Teaching;
using RoboSharp.Locales;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

public sealed class TokenPipelinePanel : IStudioPanel
{
    private static readonly SolidColorBrush StepHighlightBrush =
        new SolidColorBrush(StudioVisual.Accent) { Opacity = 0.22 };

    private readonly ITeachingLocale _locale;
    private TextBlock? _lead;
    private TextBlock? _guide;
    private TextBlock? _footer;
    private TextBox? _fallbackText;
    private Grid? _structuredRoot;
    private StackPanel? _listingHost;
    private readonly List<(Border box, int start, int len)> _stepRows = new();

    public TokenPipelinePanel(ITeachingLocale locale) =>
        _locale = locale;

    public string PanelId => StudioPanelIds.Tokens;

    public int Order => 10;

    public string DisplayName => _locale.Panels.TokensTitle;

    public string? InspectorSubtitle => _locale.Panels.TokensSubtitle;

    public PipelineInspectTier AbstractionTier => PipelineInspectTier.Lexical;

    public Control CreateView()
    {
        _fallbackText = StudioCopyableText.CreateReadOnlyOutput();

        _listingHost = new StackPanel { Spacing = 0 };

        _structuredRoot = new Grid();
        _structuredRoot.Children.Add(_listingHost);

        var layered = new Grid();
        layered.Children.Add(_fallbackText);
        layered.Children.Add(_structuredRoot);

        var (root, parts) = TeachingInspectPanelChrome.CreateWithTier(layered, AbstractionTier, dataMinHeight: 0);
        _lead = parts.Lead;
        _guide = parts.Guide;
        _footer = parts.Footer;
        _lead.Text = _locale.Panels.TokensLead;
        _guide.Text = _locale.Panels.TokensGuide;
        _footer.Text = _locale.Panels.TokensFootnote;

        return root;
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        ClearStepHighlight();
        if (_fallbackText is null || _structuredRoot is null || _listingHost is null)
            return;

        _fallbackText.IsVisible = false;
        _structuredRoot.IsVisible = true;
        _listingHost.Children.Clear();
        _stepRows.Clear();

        _listingHost.Children.Add(BuildHeaderRow());

        foreach (var t in snapshot.Tokens)
        {
            var line =
                $"{t.Kind,-22}  @{t.Span.Start,4} len {t.Span.Length,3}  {VisualizeText(t.Text)}";
            var inner = new TextBlock
            {
                Text = line,
                FontFamily = StudioVisual.CodeFontFamily,
                FontSize = 11,
                Foreground = StudioVisual.TextPrimaryBrush,
                Padding = new Thickness(4, 1, 4, 1),
            };
            var box = new Border { Child = inner };
            _listingHost.Children.Add(box);
            _stepRows.Add((box, t.Span.Start, t.Span.Length));
        }
    }

    private TextBlock BuildHeaderRow() =>
        new()
        {
            Text = _locale.Panels.TokensColumnHeader,
            FontFamily = StudioVisual.CodeFontFamily,
            FontSize = 11,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.TextMutedBrush,
            Margin = new Thickness(0, 0, 0, 4),
        };

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
            var on = TeachingPipelineListingLine.SpansOverlap(ts, tl, s0, ln);
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
            _lead.Text = _locale.Panels.TokensLead;
        if (_guide is not null)
            _guide.Text = _locale.Panels.TokensGuide;
        if (_footer is not null)
            _footer.Text = _locale.Panels.TokensFootnote;
        if (lastSnapshot is not null)
            OnSnapshotChanged(lastSnapshot);
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
