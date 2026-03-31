using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Editing;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Editor;

/// <summary>AvaloniaEdit-based buffer: line numbers, RoboSharp syntax coloring, diagnostic tinting.</summary>
public sealed class RoboSharpSourceEditor : Border
{
    private readonly TextEditor _editor;
    private StudioDiagnosticColorizer? _diagnosticColorizer;
    private bool _suspendTextEvents;

    public RoboSharpSourceEditor()
    {
        Background = StudioVisual.SurfaceElevatedBrush;
        BorderBrush = StudioVisual.BorderSubtleBrush;
        BorderThickness = new Thickness(1);
        CornerRadius = StudioVisual.PanelRadius;
        Padding = new Thickness(8);
        BoxShadow = StudioVisual.SubtleCardShadow;

        _editor = new TextEditor
        {
            ShowLineNumbers = true,
            FontFamily = StudioVisual.CodeFontFamily,
            FontSize = 13,
            Foreground = StudioVisual.TextPrimaryBrush,
            Background = Brushes.Transparent,
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            WordWrap = false,
        };

        _editor.TextArea.SelectionBrush = new SolidColorBrush(StudioVisual.Accent) { Opacity = 0.35 };
        _editor.TextArea.SelectionForeground = StudioVisual.TextPrimaryBrush;

        var hl = RoboSharpSyntaxHighlighting.Instance;
        if (hl is not null)
            _editor.SyntaxHighlighting = hl;

        _editor.Document.TextChanged += (_, _) =>
        {
            if (_suspendTextEvents)
                return;
            var t = NormalizeNewlines(_editor.Document.Text);
            if (t != _editor.Document.Text)
            {
                _suspendTextEvents = true;
                try
                {
                    var caret = _editor.CaretOffset;
                    _editor.Document.Text = t;
                    _editor.CaretOffset = Math.Min(caret, t.Length);
                }
                finally
                {
                    _suspendTextEvents = false;
                }
            }

            TextChanged?.Invoke(t);
        };

        Child = _editor;
        ApplyDiagnosticSpans([]);
    }

    /// <summary>Fired after buffer text changes (LF-normalized).</summary>
    public event Action<string>? TextChanged;

    public string Text
    {
        get => _editor.Document.Text;
        set => SetDocumentText(value);
    }

    public void SetDocumentText(string text, bool suspendEvents = false)
    {
        var normalized = NormalizeNewlines(text);
        if (_editor.Document.Text == normalized)
            return;

        if (suspendEvents)
            _suspendTextEvents = true;
        try
        {
            _editor.Document.Text = normalized;
        }
        finally
        {
            if (suspendEvents)
                _suspendTextEvents = false;
        }
    }

    public void FocusEditor() => _editor.Focus();

    public void ApplyDiagnosticSpans(IReadOnlyList<SourceDiagnosticSpan> spans)
    {
        var tv = _editor.TextArea.TextView;
        if (_diagnosticColorizer is not null)
        {
            tv.LineTransformers.Remove(_diagnosticColorizer);
            _diagnosticColorizer = null;
        }

        _diagnosticColorizer = new StudioDiagnosticColorizer(spans);
        tv.LineTransformers.Add(_diagnosticColorizer);
        tv.Redraw();
    }

    private static string NormalizeNewlines(string text) =>
        string.IsNullOrEmpty(text) ? text : text.ReplaceLineEndings("\n");
}
