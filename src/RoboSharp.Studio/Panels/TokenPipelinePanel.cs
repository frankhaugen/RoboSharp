using Avalonia.Controls;
using Avalonia.Media;
using RoboSharp.Locales;
using RoboSharp.Application.Teaching;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

public sealed class TokenPipelinePanel : IStudioPanel
{
    private readonly ITeachingLocale _locale;
    private TextBlock? _lead;
    private TextBlock? _guide;
    private TextBlock? _footer;
    private TextBox? _text;

    public TokenPipelinePanel(ITeachingLocale locale) =>
        _locale = locale;

    public string PanelId => StudioPanelIds.Tokens;

    public int Order => 10;

    public string DisplayName => _locale.Panels.TokensTitle;

    public string? InspectorSubtitle => _locale.Panels.TokensSubtitle;

    public PipelineInspectTier AbstractionTier => PipelineInspectTier.Lexical;

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

        var (root, parts) = TeachingInspectPanelChrome.CreateWithTier(scroll, AbstractionTier, dataMinHeight: 0);
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
        if (_text is null)
            return;

        var rows = snapshot.Tokens.Select(t =>
            $"{t.Kind,-22}  @{t.Span.Start,4} len {t.Span.Length,3}  {VisualizeText(t.Text)}");

        _text.Text = _locale.Panels.TokensColumnHeader + Environment.NewLine + string.Join(Environment.NewLine, rows);
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
