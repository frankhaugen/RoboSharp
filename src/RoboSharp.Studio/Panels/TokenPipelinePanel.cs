using Avalonia;
using Avalonia.Controls;
using RoboSharp.Locales;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

public sealed class TokenPipelinePanel : IStudioPanel
{
    private readonly ITeachingLocale _locale;
    private TextBox? _text;

    public TokenPipelinePanel(ITeachingLocale locale) =>
        _locale = locale;

    public string PanelId => StudioPanelIds.Tokens;

    public int Order => 10;

    public string DisplayName => _locale.Panels.TokensTitle;

    public string? InspectorSubtitle => _locale.Panels.TokensSubtitle;

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

        var rows = snapshot.Tokens.Select(t =>
            $"{t.Kind,-22}  @{t.Span.Start,4} len {t.Span.Length,3}  {VisualizeText(t.Text)}");

        _text.Text = _locale.Panels.TokensPreamble + string.Join(Environment.NewLine, rows);
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
