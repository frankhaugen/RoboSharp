using Avalonia;
using Avalonia.Controls;
using RoboSharp.Locales;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

public sealed class DiagnosticsPipelinePanel : IStudioPanel
{
    private readonly ITeachingLocale _locale;
    private TextBox? _text;

    public DiagnosticsPipelinePanel(ITeachingLocale locale) =>
        _locale = locale;

    public string PanelId => StudioPanelIds.Diagnostics;

    public int Order => 30;

    public string DisplayName => _locale.Panels.DiagnosticsTitle;

    public string? InspectorSubtitle => _locale.Panels.DiagnosticsSubtitle;

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

        var lines = new List<string>();

        foreach (var d in snapshot.ParseDiagnostics)
        {
            var loc = SourceLocationFormatter.FormatLine(snapshot.Source, d.Span);
            lines.Add(_locale.Pipeline.FormatParseDiagnosticLine(d.Span.Start, d.Span.Length, loc, d.Message));
        }

        foreach (var s in snapshot.SemanticDiagnosticLines)
            lines.Add(s);

        if (snapshot.RuntimeFaultMessage is not null)
            lines.Add(_locale.Panels.DiagnosticsRuntimePrefix + snapshot.RuntimeFaultMessage);

        if (lines.Count == 0)
            lines.Add(_locale.Panels.DiagnosticsNone);

        _text.Text = _locale.Panels.DiagnosticsPreamble + string.Join(Environment.NewLine, lines);
    }
}
