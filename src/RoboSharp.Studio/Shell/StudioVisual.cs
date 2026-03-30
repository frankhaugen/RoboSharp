using Avalonia;
using Avalonia.Media;

namespace RoboSharp.Studio.Shell;

/// <summary>
/// Central palette + typography for a cohesive, modern dark Studio chrome (all C# — no XAML theme files).
/// </summary>
public static class StudioVisual
{
    public static Color BackgroundDeep { get; } = Color.Parse("#0B0F14");

    public static Color Surface { get; } = Color.Parse("#121820");

    public static Color SurfaceElevated { get; } = Color.Parse("#1A2332");

    public static Color Accent { get; } = Color.Parse("#2EE6C8");

    public static Color AccentDim { get; } = Color.Parse("#1B9A87");

    public static Color Coral { get; } = Color.Parse("#FF7A6E");

    public static Color TextPrimary { get; } = Color.Parse("#E8F0F7");

    public static Color TextMuted { get; } = Color.Parse("#8FA3B8");

    public static Color BorderSubtle { get; } = Color.Parse("#2A3544");

    public static SolidColorBrush BackgroundDeepBrush { get; } = new(BackgroundDeep);

    public static SolidColorBrush SurfaceBrush { get; } = new(Surface);

    public static SolidColorBrush SurfaceElevatedBrush { get; } = new(SurfaceElevated);

    public static SolidColorBrush AccentBrush { get; } = new(Accent);

    public static SolidColorBrush TextPrimaryBrush { get; } = new(TextPrimary);

    public static SolidColorBrush TextMutedBrush { get; } = new(TextMuted);

    public static SolidColorBrush BorderSubtleBrush { get; } = new(BorderSubtle);

    public static FontFamily CodeFontFamily { get; } = new("JetBrains Mono, Cascadia Mono, Consolas, Courier New, monospace");

    public static FontFamily UiFontFamily { get; } = new("Inter, Segoe UI, system-ui, sans-serif");

    public static CornerRadius PanelRadius { get; } = new(10);

    public static CornerRadius ButtonRadius { get; } = new(8);
}
