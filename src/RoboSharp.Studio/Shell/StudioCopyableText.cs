using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace RoboSharp.Studio.Shell;

/// <summary>Read-only multiline <see cref="TextBox"/> so learners can select, copy, and paste pipeline output.</summary>
internal static class StudioCopyableText
{
    public static TextBox CreateReadOnlyOutput(double fontSize = 12)
    {
        return new TextBox
        {
            IsReadOnly = true,
            AcceptsReturn = true,
            AcceptsTab = true,
            TextWrapping = TextWrapping.Wrap,
            FontFamily = StudioVisual.CodeFontFamily,
            FontSize = fontSize,
            Foreground = StudioVisual.TextPrimaryBrush,
            CaretBrush = StudioVisual.TextPrimaryBrush,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(0),
            MinHeight = 36,
        };
    }
}
