using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace RoboSharp.Studio.Shell;

/// <summary>Shared layout for pipeline inspector tabs: hook, teaching copy, card-wrapped data, footnote.</summary>
internal static class TeachingInspectPanelChrome
{
    public sealed record Parts(TextBlock Lead, TextBlock Guide, TextBlock Footer);

    /// <summary>Creates lead + guide + card containing <paramref name="dataContent"/> + footer. Caller sets texts and updates data.</summary>
    public static (Border Root, Parts Parts) Create(
        Control dataContent,
        double dataMinHeight = 120)
    {
        var lead = new TextBlock
        {
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.AccentBrush,
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

        dataContent.MinHeight = dataMinHeight;

        var dataCard = new Border
        {
            Background = StudioVisual.SurfaceElevatedBrush,
            BorderBrush = StudioVisual.BorderSubtleBrush,
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
            Padding = new Thickness(4, 2, 4, 8),
            Child = stack,
        };

        return (root, new Parts(lead, guide, footer));
    }
}
