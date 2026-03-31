using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using RoboSharp.Application.Teaching;

namespace RoboSharp.Studio.Editor;

/// <summary>Soft red background on parse/semantic diagnostic spans (AvaloniaEdit line transformer).</summary>
public sealed class StudioDiagnosticColorizer : DocumentColorizingTransformer
{
    private static readonly IBrush ErrorTint = new SolidColorBrush(Color.FromArgb(0x55, 220, 72, 72));

    private readonly IReadOnlyList<SourceDiagnosticSpan> _spans;

    public StudioDiagnosticColorizer(IReadOnlyList<SourceDiagnosticSpan> spans) =>
        _spans = spans;

    protected override void ColorizeLine(DocumentLine line)
    {
        if (_spans.Count == 0)
            return;

        var lo = line.Offset;
        var hi = line.EndOffset;

        foreach (var s in _spans)
        {
            var end = s.Start + s.Length;
            if (end <= lo || s.Start >= hi)
                continue;

            var a = Math.Max(lo, s.Start);
            var b = Math.Min(hi, end);
            if (a < b)
                ChangeLinePart(a, b, e => e.BackgroundBrush = ErrorTint);
        }
    }
}
