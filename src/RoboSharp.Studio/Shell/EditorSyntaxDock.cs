using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using RoboSharp.Language;
using RoboSharp.Locales;
using RoboSharp.Application.Teaching;

namespace RoboSharp.Studio.Shell;

/// <summary>Bottom-of-editor lexer-colored reference strip (IDE-style), separate from inspector tabs.</summary>
public sealed class EditorSyntaxDock : Border
{
    private readonly ITeachingLocale _locale;
    private readonly TextBlock _title;
    private readonly TextBlock _subtitle;
    private readonly TextBlock _body;
    private readonly StackPanel _legend;

    public EditorSyntaxDock(ITeachingLocale locale)
    {
        _locale = locale;
        _title = new TextBlock
        {
            FontSize = 12,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.AccentBrush,
            Margin = new Thickness(0, 0, 0, 4),
        };
        _subtitle = new TextBlock
        {
            FontSize = 11,
            Foreground = StudioVisual.TextMutedBrush,
            TextWrapping = TextWrapping.Wrap,
            LineHeight = 16,
            Margin = new Thickness(0, 0, 0, 8),
        };
        _legend = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            Margin = new Thickness(0, 0, 0, 8),
        };
        _body = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            FontFamily = StudioVisual.CodeFontFamily,
            FontSize = 12,
            LineHeight = 18,
            Foreground = StudioVisual.TextPrimaryBrush,
        };

        var scroll = new ScrollViewer
        {
            MaxHeight = 200,
            Content = _body,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
        };

        var inner = new StackPanel
        {
            Spacing = 0,
            Children = { _title, _subtitle, _legend, scroll },
        };

        Background = StudioVisual.SurfaceElevatedBrush;
        BorderBrush = StudioVisual.BorderSubtleBrush;
        BorderThickness = new Thickness(1, 1, 0, 0);
        CornerRadius = new CornerRadius(0, 0, StudioVisual.PanelRadius.BottomLeft, StudioVisual.PanelRadius.BottomRight);
        Padding = new Thickness(10, 8, 10, 8);
        Child = inner;

        ApplyLocaleStrings();
        RebuildLegend();
    }

    public void ApplySnapshot(PipelineSnapshot? snapshot)
    {
        if (snapshot is null || snapshot.Tokens.Count == 0)
        {
            _body.Inlines?.Clear();
            _body.Text = _locale.Panels.EditorSyntaxDockEmpty;
            return;
        }

        _body.Text = null;
        LexerColoredSourcePresenter.PopulateInlines(_body, snapshot.Source, snapshot.Tokens);
    }

    public void ApplyLocale()
    {
        ApplyLocaleStrings();
        RebuildLegend();
    }

    private void ApplyLocaleStrings()
    {
        _title.Text = _locale.Panels.EditorSyntaxDockTitle;
        _subtitle.Text = _locale.Panels.EditorSyntaxDockSubtitle;
    }

    private void RebuildLegend()
    {
        _legend.Children.Clear();
        var p = _locale.Panels;
        AddLegendItem(p.EditorSyntaxLegendKeyword, LexerColoredSourcePresenter.BrushFor(SyntaxKind.IfKeyword));
        AddLegendItem(p.EditorSyntaxLegendIdentifier, LexerColoredSourcePresenter.BrushFor(SyntaxKind.IdentifierToken));
        AddLegendItem(p.EditorSyntaxLegendNumberLiteral, LexerColoredSourcePresenter.BrushFor(SyntaxKind.IntegerLiteralToken));
        AddLegendItem(p.EditorSyntaxLegendStringLiteral, LexerColoredSourcePresenter.BrushFor(SyntaxKind.StringLiteralToken));
        AddLegendItem(p.EditorSyntaxLegendComment, StudioVisual.TextMutedBrush);
        AddLegendItem(p.EditorSyntaxLegendPunctuation, LexerColoredSourcePresenter.BrushFor(SyntaxKind.SemicolonToken));
        AddLegendItem(p.EditorSyntaxLegendError, LexerColoredSourcePresenter.BrushFor(SyntaxKind.BadToken));
    }

    private void AddLegendItem(string label, IBrush swatch)
    {
        var sw = new Border
        {
            Width = 14,
            Height = 14,
            CornerRadius = new CornerRadius(2),
            Background = (IBrush?)swatch,
            VerticalAlignment = VerticalAlignment.Center,
        };
        var tx = new TextBlock
        {
            Text = label,
            FontSize = 10,
            Foreground = StudioVisual.TextMutedBrush,
            VerticalAlignment = VerticalAlignment.Center,
        };
        _legend.Children.Add(new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            Children = { sw, tx },
        });
    }
}
