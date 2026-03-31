using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using RoboSharp.Language;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

/// <summary>Lexer-based syntax coloring (updates on Build) — kid-friendly without a full code editor package.</summary>
public sealed class ColoredSourcePreviewPanel : IStudioPanel
{
    private TextBlock? _text;

    public int Order => 6;

    public string DisplayName => "Syntax colors";

    public string? InspectorSubtitle =>
        "Keywords, strings, and comments tinted from the last Build. Edit in the main editor; this panel refreshes when you Build.";

    public Control CreateView()
    {
        _text = new TextBlock
        {
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            FontFamily = StudioVisual.CodeFontFamily,
            FontSize = 12,
            LineHeight = 18,
            Foreground = StudioVisual.TextPrimaryBrush,
        };

        return new Border
        {
            Padding = new Thickness(8),
            Child = new ScrollViewer
            {
                MaxHeight = 280,
                Content = _text,
            },
        };
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        if (_text is null)
            return;

        _text.Inlines?.Clear();
        const string preamble =
            "# Syntax colors (teaching preview)\r\n" +
            "This is the same text as your editor, colored by the lexer. It is not live while you type — press Build to refresh.\r\n\r\n";

        _text.Inlines!.Add(new Run(preamble) { Foreground = StudioVisual.TextMutedBrush });

        if (snapshot.Tokens.Count == 0)
        {
            _text.Inlines.Add(new Run("(Nothing to show.)"));
            return;
        }

        foreach (var token in snapshot.Tokens)
        {
            if (token.Kind == SyntaxKind.EndOfFileToken)
                break;

            foreach (var tr in token.LeadingTrivia)
                AddTrivia(_text, tr);

            AddToken(_text, snapshot.Source, token);

            foreach (var tr in token.TrailingTrivia)
                AddTrivia(_text, tr);
        }
    }

    private static void AddTrivia(TextBlock tb, SyntaxTrivia trivia)
    {
        IBrush brush = trivia.Kind == SyntaxKind.CommentTrivia
            ? (IBrush)StudioVisual.TextMutedBrush
            : (IBrush)Brushes.Gray;
        tb.Inlines!.Add(new Run(trivia.Text) { Foreground = brush });
    }

    private static void AddToken(TextBlock tb, string source, SyntaxToken token)
    {
        var len = Math.Clamp(token.Span.Length, 0, Math.Max(0, source.Length - token.Span.Start));
        var slice = len == 0 ? token.Text : source.AsSpan(token.Span.Start, len).ToString();
        tb.Inlines!.Add(new Run(slice) { Foreground = BrushFor(token.Kind) });
    }

    private static IBrush BrushFor(SyntaxKind kind) =>
        kind switch
        {
            >= SyntaxKind.IfKeyword and <= SyntaxKind.FalseKeyword => (IBrush)new SolidColorBrush(Color.Parse("#6EC9C0")),
            SyntaxKind.IdentifierToken => (IBrush)StudioVisual.TextPrimaryBrush,
            SyntaxKind.IntegerLiteralToken or SyntaxKind.NumberLiteralToken =>
                (IBrush)new SolidColorBrush(Color.Parse("#E8C468")),
            SyntaxKind.StringLiteralToken => (IBrush)new SolidColorBrush(Color.Parse("#F0A875")),
            SyntaxKind.BadToken => (IBrush)new SolidColorBrush(Color.Parse("#FF6B6B")),
            _ => (IBrush)new SolidColorBrush(Color.Parse("#B8C0CC")),
        };
}
