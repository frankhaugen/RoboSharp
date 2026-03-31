using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Editor;

/// <summary>Accent tint on the current interpreter step span (AvaloniaEdit line transformer).</summary>
public sealed class StudioStepSpanColorizer : DocumentColorizingTransformer
{
    private static readonly IBrush StepTint = new SolidColorBrush(StudioVisual.Accent) { Opacity = 0.18 };

    private readonly int _start;
    private readonly int _length;

    public StudioStepSpanColorizer(int start, int length)
    {
        _start = start;
        _length = length;
    }

    protected override void ColorizeLine(DocumentLine line)
    {
        if (_length <= 0 || _start < 0)
            return;

        var lo = line.Offset;
        var hi = line.EndOffset;
        var end = _start + _length;
        if (end <= lo || _start >= hi)
            return;

        var a = Math.Max(lo, _start);
        var b = Math.Min(hi, end);
        if (a < b)
            ChangeLinePart(a, b, e => e.BackgroundBrush = StepTint);
    }
}
