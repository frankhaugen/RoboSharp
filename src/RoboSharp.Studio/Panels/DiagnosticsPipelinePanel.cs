using Avalonia.Controls;
using RoboSharp.Locales;
using RoboSharp.Application.Teaching;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

public sealed class DiagnosticsPipelinePanel : IStudioPanel
{
    private readonly ITeachingLocale _locale;
    private TextBlock? _lead;
    private TextBlock? _guide;
    private TextBlock? _footer;
    private TextBox? _text;

    public DiagnosticsPipelinePanel(ITeachingLocale locale) =>
        _locale = locale;

    public string PanelId => StudioPanelIds.Diagnostics;

    public int Order => 30;

    public string DisplayName => _locale.Panels.DiagnosticsTitle;

    public string? InspectorSubtitle => null;

    public PipelineInspectTier AbstractionTier => PipelineInspectTier.Diagnostics;

    public Control CreateView()
    {
        _text = StudioCopyableText.CreateReadOnlyOutput();

        var (root, parts) = TeachingInspectPanelChrome.CreateWithTier(_text, AbstractionTier, dataMinHeight: 0);
        _lead = parts.Lead;
        _guide = parts.Guide;
        _footer = parts.Footer;
        _lead.Text = _locale.Panels.DiagnosticsLead;
        _guide.Text = _locale.Panels.DiagnosticsGuide;
        _footer.Text = _locale.Panels.DiagnosticsFooter;

        return root;
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

    public void ApplyLocale(PipelineSnapshot? lastSnapshot)
    {
        if (_lead is not null)
            _lead.Text = _locale.Panels.DiagnosticsLead;
        if (_guide is not null)
            _guide.Text = _locale.Panels.DiagnosticsGuide;
        if (_footer is not null)
            _footer.Text = _locale.Panels.DiagnosticsFooter;
        if (lastSnapshot is not null)
            OnSnapshotChanged(lastSnapshot);
    }
}
