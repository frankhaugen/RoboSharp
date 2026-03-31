using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using RoboSharp.Language;

namespace RoboSharp.Studio.Shell;

/// <summary>Applies lexer-driven <see cref="Run"/> colors to a <see cref="TextBlock"/> (Build snapshot, not live typing).</summary>
public static class LexerColoredSourcePresenter
{
    public static void PopulateInlines(TextBlock textBlock, string source, IReadOnlyList<SyntaxToken> tokens)
    {
        textBlock.Inlines?.Clear();
        if (tokens.Count == 0)
            return;

        foreach (var token in tokens)
        {
            if (token.Kind == SyntaxKind.EndOfFileToken)
                break;

            foreach (var tr in token.LeadingTrivia)
                AddTrivia(textBlock, tr);

            AddToken(textBlock, source, token);

            foreach (var tr in token.TrailingTrivia)
                AddTrivia(textBlock, tr);
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

    public static IBrush BrushFor(SyntaxKind kind) =>
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
