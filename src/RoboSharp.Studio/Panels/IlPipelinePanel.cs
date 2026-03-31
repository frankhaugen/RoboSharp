using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

using RoboSharp.Locales;
using RoboSharp.Application.Teaching;
using RoboSharp.Studio.Shell;
using RoboSharp.Toolchain;

namespace RoboSharp.Studio.Panels;

public sealed class IlPipelinePanel : IStudioPanel
{
    private static readonly SolidColorBrush IlStepHighlightBrush =
        new SolidColorBrush(StudioVisual.Accent) { Opacity = 0.22 };

    private readonly ITeachingLocale _locale;
    private TextBox? _fallbackText;
    private Grid? _structuredRoot;
    private TextBlock? _preambleBlock;
    private StackPanel? _listingHost;
    private TextBlock? _footnoteBlock;
    private Button? _copyButton;
    private ScrollViewer? _listingScroll;
    private string? _copyPayload;
    private readonly List<(Border box, int fi, int ip)> _stepRows = new();
    private int? _lastHighlightFi;
    private int? _lastHighlightIp;

    public IlPipelinePanel(ITeachingLocale locale) =>
        _locale = locale;

    public string PanelId => StudioPanelIds.Il;

    public int Order => 50;

    public string DisplayName => _locale.Panels.IlTitle;

    public string? InspectorSubtitle => _locale.Panels.IlSubtitle;

    public Control CreateView()
    {
        _fallbackText = StudioCopyableText.CreateReadOnlyOutput(fontSize: 11);

        _preambleBlock = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            FontSize = 12,
            Foreground = StudioVisual.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 8),
        };

        _copyButton = new Button
        {
            Content = _locale.Panels.IlCopyDisassembly,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 0, 8),
        };
        _copyButton.Click += OnCopyClick;

        _listingHost = new StackPanel { Spacing = 0 };

        _listingScroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = _listingHost,
            MinHeight = 120,
        };

        _footnoteBlock = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            FontSize = 11,
            Foreground = StudioVisual.TextMutedBrush,
            Margin = new Thickness(0, 12, 0, 0),
            IsVisible = false,
        };

        _structuredRoot = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,Auto,*,Auto"),
            IsVisible = false,
            Children =
            {
                _preambleBlock,
                _copyButton,
                _listingScroll,
                _footnoteBlock,
            },
        };
        Grid.SetRow(_preambleBlock, 0);
        Grid.SetRow(_copyButton, 1);
        Grid.SetRow(_listingScroll, 2);
        Grid.SetRow(_footnoteBlock, 3);

        var layered = new Grid();
        layered.Children.Add(_fallbackText);
        layered.Children.Add(_structuredRoot);

        return new Border
        {
            Padding = new Thickness(4),
            Child = layered,
        };
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        ClearStepHighlight();

        if (_fallbackText is null || _structuredRoot is null || _preambleBlock is null ||
            _listingHost is null || _footnoteBlock is null)
            return;

        if (_copyButton is not null)
            _copyButton.Content = _locale.Panels.IlCopyDisassembly;

        if (snapshot.IlProgram is not null && snapshot.IlDisassemblyText is { Length: > 0 } ilText)
        {
            _fallbackText.IsVisible = false;
            _structuredRoot.IsVisible = true;

            _preambleBlock.Text = _locale.Panels.IlPreamble.TrimEnd();

            var body = ilText;
            if (snapshot.IlExecutionFootnote is { Length: > 0 } foot)
                body += "\r\n\r\n" + foot;
            _copyPayload = _locale.Panels.IlPreamble + body;

            RebuildListing(snapshot.IlProgram);

            if (snapshot.IlExecutionFootnote is { Length: > 0 } foot2)
            {
                _footnoteBlock.Text = foot2.TrimEnd();
                _footnoteBlock.IsVisible = true;
            }
            else
            {
                _footnoteBlock.Text = string.Empty;
                _footnoteBlock.IsVisible = false;
            }

            return;
        }

        _structuredRoot.IsVisible = false;
        _fallbackText.IsVisible = true;
        _copyPayload = null;

        if (snapshot.IlDisassemblyText is { Length: > 0 } il)
        {
            var body = il;
            if (snapshot.IlExecutionFootnote is { Length: > 0 } foot)
                body += "\r\n\r\n" + foot;
            _fallbackText.Text = _locale.Panels.IlPreamble + body;
            return;
        }

        _fallbackText.Text = _locale.Panels.IlPreamble + (snapshot.CompileReachedPhase < CompilePhase.Lowered
            ? _locale.Panels.IlWaitingForLowering
            : _locale.Panels.IlNoTextUnexpected);
    }

    public void OnRunProgress(StudioRunProgress progress)
    {
        if (_listingHost is null || _structuredRoot is not { IsVisible: true })
            return;

        var fi = progress.IlHighlightFunctionIndex;
        var ip = progress.IlHighlightInstructionIndex;

        Border? scrollTo = null;
        foreach (var (box, rowFi, rowIp) in _stepRows)
        {
            var on = fi == rowFi && ip == rowIp;
            box.Background = on ? IlStepHighlightBrush : null;
            if (on)
                scrollTo = box;
        }

        if (scrollTo is not null && (fi != _lastHighlightFi || ip != _lastHighlightIp))
            scrollTo.BringIntoView();

        _lastHighlightFi = fi;
        _lastHighlightIp = ip;
    }

    private void ClearStepHighlight()
    {
        foreach (var (box, _, _) in _stepRows)
            box.Background = null;
        _stepRows.Clear();
        _lastHighlightFi = null;
        _lastHighlightIp = null;
    }

    private void RebuildListing(RoboSharp.IL.RoboProgram program)
    {
        if (_listingHost is null)
            return;

        _listingHost.Children.Clear();
        _stepRows.Clear();

        foreach (var line in IlTeachingFormatter.BuildListing(program))
        {
            if (line.Kind == IlListingLineKind.Instruction)
            {
                var inner = new TextBlock
                {
                    Text = line.Text,
                    FontFamily = StudioVisual.CodeFontFamily,
                    FontSize = 11,
                    Foreground = StudioVisual.TextPrimaryBrush,
                    Padding = new Thickness(4, 1, 4, 1),
                };
                var box = new Border { Child = inner };
                _listingHost.Children.Add(box);
                _stepRows.Add((box, line.FunctionIndex, line.InstructionIndex));
            }
            else
            {
                var tb = new TextBlock
                {
                    Text = line.Text,
                    FontFamily = line.Kind == IlListingLineKind.FunctionHeader
                        ? StudioVisual.UiFontFamily
                        : StudioVisual.CodeFontFamily,
                    FontSize = line.Kind == IlListingLineKind.FunctionHeader ? 12 : 11,
                    FontWeight = line.Kind == IlListingLineKind.FunctionHeader ? FontWeight.SemiBold : FontWeight.Normal,
                    Foreground = line.Kind == IlListingLineKind.FunctionHeader
                        ? StudioVisual.AccentBrush
                        : StudioVisual.TextPrimaryBrush,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, line.Kind == IlListingLineKind.FunctionHeader ? 6 : 0, 0, 0),
                };
                _listingHost.Children.Add(tb);
            }
        }
    }

    private async void OnCopyClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_copyPayload))
            return;
        if (sender is not Control c)
            return;
        var top = TopLevel.GetTopLevel(c);
        if (top?.Clipboard is { } clip)
            await clip.SetTextAsync(_copyPayload).ConfigureAwait(true);
    }
}