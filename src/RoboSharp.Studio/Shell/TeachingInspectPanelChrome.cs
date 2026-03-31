using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace RoboSharp.Studio.Shell;

/// <summary>Shared layout for pipeline inspector: hook, teaching copy, tier accent, card-wrapped data, footnote.</summary>
internal static class TeachingInspectPanelChrome
{
    public sealed record Parts(TextBlock Lead, TextBlock Guide, TextBlock Footer);

    /// <summary>Creates lead + guide + card containing <paramref name="dataContent"/> + footer (default teaching accent).</summary>
    public static (Border Root, Parts Parts) Create(
        Control dataContent,
        double dataMinHeight = 120) =>
        CreateWithTier(dataContent, PipelineInspectTier.VirtualIl, dataMinHeight);

    /// <summary>Same as <see cref="Create"/> but colors match the pipeline abstraction tier.</summary>
    public static (Border Root, Parts Parts) CreateWithTier(
        Control dataContent,
        PipelineInspectTier tier,
        double dataMinHeight = 120)
    {
        var tierBrush = StudioVisual.TierBrush(tier);
        var dimBorder = new SolidColorBrush(tierBrush.Color) { Opacity = 0.42 };

        var lead = new TextBlock
        {
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            Foreground = tierBrush,
            TextWrapping = TextWrapping.Wrap,
            LineHeight = 20,
            Margin = new Thickness(0, 0, 0, 6),
        };

        var guide = new TextBlock
        {
            FontSize = 12,
            Foreground = StudioVisual.TextPrimaryBrush,
            TextWrapping = TextWrapping.Wrap,
            LineHeight = 20,
            Margin = new Thickness(0, 0, 0, 10),
        };

        if (dataMinHeight > 0)
            dataContent.MinHeight = dataMinHeight;

        var dataCard = new Border
        {
            Background = StudioVisual.SurfaceElevatedBrush,
            BorderBrush = dimBorder,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(10, 8),
            Child = dataContent,
        };

        var footer = new TextBlock
        {
            FontSize = 11,
            Foreground = StudioVisual.TextMutedBrush,
            TextWrapping = TextWrapping.Wrap,
            LineHeight = 17,
            Margin = new Thickness(0, 12, 0, 0),
        };

        var stack = new StackPanel
        {
            Spacing = 0,
            Children = { lead, guide, dataCard, footer },
        };

        var root = new Border
        {
            BorderBrush = tierBrush,
            BorderThickness = new Thickness(4, 0, 0, 0),
            Padding = new Thickness(10, 2, 4, 8),
            Child = stack,
        };

        return (root, new Parts(lead, guide, footer));
    }
}
