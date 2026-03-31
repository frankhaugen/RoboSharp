using Avalonia;
using Avalonia.Controls;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

public sealed class DiagnosticsPipelinePanel : IStudioPanel
{
    private TextBox? _text;

    public int Order => 30;

    public string DisplayName => "Diagnostics";

    public string? InspectorSubtitle =>
        "Parse, semantic (binder), and runtime messages from the last Build or Run. Lines are prefixed by phase.";

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

        const string preamble =
            "# Diagnostics (compiler & interpreter)\r\n" +
            "parse    — lexer/parser could not build a syntax tree (see span: start:length in source).\r\n" +
            "semantic — binder/type rules failed after a successful parse.\r\n" +
            "runtime  — interpreter reported a fault while executing lowered IL (after Run).\r\n" +
            "\r\n";

        var lines = new List<string>();

        foreach (var d in snapshot.ParseDiagnostics)
        {
            var loc = SourceLocationFormatter.FormatLine(snapshot.Source, d.Span);
            lines.Add($"parse     @{d.Span.Start}:{d.Span.Length}  ({loc})  {d.Message}");
        }

        foreach (var s in snapshot.SemanticDiagnosticLines)
            lines.Add($"semantic  {s}");

        if (snapshot.RuntimeFaultMessage is not null)
            lines.Add($"runtime   {snapshot.RuntimeFaultMessage}");

        if (lines.Count == 0)
            lines.Add("(No diagnostics — last Build/Run did not report errors in these phases.)");

        _text.Text = preamble + string.Join(Environment.NewLine, lines);
    }
}
