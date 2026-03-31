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

    public static Color KarelWall { get; } = Color.Parse("#06090D");

    public static Color KarelFloor { get; } = Color.Parse("#1A2638");

    public static Color KarelGoal { get; } = Color.Parse("#153D36");

    public static Color KarelWorldChrome { get; } = Color.Parse("#0E141C");

    public static Color KarelRobotGlyph { get; } = Color.Parse("#051016");

    public static SolidColorBrush BackgroundDeepBrush { get; } = new(BackgroundDeep);

    public static SolidColorBrush SurfaceBrush { get; } = new(Surface);

    public static SolidColorBrush SurfaceElevatedBrush { get; } = new(SurfaceElevated);

    public static SolidColorBrush AccentBrush { get; } = new(Accent);

    public static SolidColorBrush TextPrimaryBrush { get; } = new(TextPrimary);

    public static SolidColorBrush TextMutedBrush { get; } = new(TextMuted);

    public static SolidColorBrush BorderSubtleBrush { get; } = new(BorderSubtle);

    public static SolidColorBrush KarelWallBrush { get; } = new(KarelWall);

    public static SolidColorBrush KarelFloorBrush { get; } = new(KarelFloor);

    public static SolidColorBrush KarelGoalBrush { get; } = new(KarelGoal);

    public static SolidColorBrush KarelWorldChromeBrush { get; } = new(KarelWorldChrome);

    /// <summary>Robot tile fill — accent-tinted so the actor reads at a glance.</summary>
    public static SolidColorBrush KarelRobotCellBrush { get; } = new SolidColorBrush(Accent) { Opacity = 0.38 };

    public static SolidColorBrush KarelRobotGlyphBrush { get; } = new(KarelRobotGlyph);

    public static SolidColorBrush KarelCellEdgeBrush { get; } = new(Color.Parse("#141C28"));

    /// <summary>Soft elevation under chrome panels (world, inspector cards).</summary>
    public static BoxShadows SoftPanelShadow { get; } = new(
        new BoxShadow
        {
            OffsetX = 0,
            OffsetY = 4,
            Blur = 14,
            Spread = 0,
            Color = Color.FromArgb(0x5A, 0, 0, 0),
        });

    public static BoxShadows SubtleCardShadow { get; } = new(
        new BoxShadow
        {
            OffsetX = 0,
            OffsetY = 2,
            Blur = 10,
            Spread = 0,
            Color = Color.FromArgb(0x40, 0, 0, 0),
        });

    public static BoxShadows ToolbarShadow { get; } = new(
        new BoxShadow
        {
            OffsetX = 0,
            OffsetY = 3,
            Blur = 8,
            Spread = 0,
            Color = Color.FromArgb(0x45, 0, 0, 0),
        });

    public static FontFamily CodeFontFamily { get; } = new("JetBrains Mono, Cascadia Mono, Consolas, Courier New, monospace");

    public static FontFamily UiFontFamily { get; } = new("Inter, Segoe UI, system-ui, sans-serif");

    public static CornerRadius PanelRadius { get; } = new(10);

    public static CornerRadius ButtonRadius { get; } = new(8);
}
