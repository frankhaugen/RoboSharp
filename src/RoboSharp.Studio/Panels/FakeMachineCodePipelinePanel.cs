using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using RoboSharp.Application.Teaching;
using RoboSharp.Locales;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

public sealed class FakeMachineCodePipelinePanel : IStudioPanel
{
    private static readonly SolidColorBrush StepHighlightBrush =
        new SolidColorBrush(StudioVisual.Accent) { Opacity = 0.22 };

    private readonly ITeachingLocale _locale;
    private TextBox? _fallbackText;
    private Grid? _structuredRoot;
    private TextBlock? _leadBlock;
    private TextBlock? _guideBlock;
    private TextBlock? _footerBlock;
    private StackPanel? _listingHost;
    private ScrollViewer? _listingScroll;
    private readonly List<(Border box, int fi, int ip)> _stepRows = new();
    private int? _lastHighlightFi;
    private int? _lastHighlightIp;

    public FakeMachineCodePipelinePanel(ITeachingLocale locale) =>
        _locale = locale;

    public string PanelId => StudioPanelIds.FakeMachineCode;

    public int Order => 60;

    public string DisplayName => _locale.Panels.FakeMachineCodeTitle;

    public string? InspectorSubtitle => _locale.Panels.FakeMachineCodeSubtitle;

    public PipelineInspectTier AbstractionTier => PipelineInspectTier.MachineEncoding;

    public Control CreateView()
    {
        _fallbackText = StudioCopyableText.CreateReadOnlyOutput(fontSize: 11);

        _leadBlock = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.TierBrush(AbstractionTier),
            Margin = new Thickness(0, 0, 0, 6),
        };

        _guideBlock = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            FontSize = 12,
            LineHeight = 20,
            Foreground = StudioVisual.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 10),
        };

        _listingHost = new StackPanel { Spacing = 0 };

        _listingScroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = _listingHost,
            MinHeight = 120,
        };

        var accent = StudioVisual.TierBrush(AbstractionTier);
        var listingCard = new Border
        {
            Background = StudioVisual.SurfaceElevatedBrush,
            BorderBrush = new SolidColorBrush(accent.Color) { Opacity = 0.42 },
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(10, 8),
            Child = _listingScroll,
        };

        _footerBlock = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            FontSize = 11,
            Foreground = StudioVisual.TextMutedBrush,
            LineHeight = 17,
            Margin = new Thickness(0, 12, 0, 0),
        };

        _structuredRoot = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,Auto,*,Auto"),
            IsVisible = false,
            Children =
            {
                _leadBlock,
                _guideBlock,
                listingCard,
                _footerBlock,
            },
        };
        Grid.SetRow(_leadBlock, 0);
        Grid.SetRow(_guideBlock, 1);
        Grid.SetRow(listingCard, 2);
        Grid.SetRow(_footerBlock, 3);

        var layered = new Grid();
        layered.Children.Add(_fallbackText);
        layered.Children.Add(_structuredRoot);

        return new Border
        {
            BorderBrush = accent,
            BorderThickness = new Thickness(4, 0, 0, 0),
            Padding = new Thickness(10, 2, 4, 8),
            Child = layered,
        };
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        ClearStepHighlight();
        if (_fallbackText is null || _structuredRoot is null || _leadBlock is null || _guideBlock is null ||
            _listingHost is null || _footerBlock is null)
            return;

        _leadBlock.Text = _locale.Panels.FakeMachineCodeLead;
        _guideBlock.Text = _locale.Panels.FakeMachineCodeGuide;
        _footerBlock.Text = _locale.Panels.FakeMachineCodeFooter;

        if (snapshot.IlProgram is not null && snapshot.FakeMachineCodeText is { Length: > 0 })
        {
            _fallbackText.IsVisible = false;
            _structuredRoot.IsVisible = true;
            RebuildListing(snapshot.IlProgram);
            return;
        }

        _structuredRoot.IsVisible = false;
        _fallbackText.IsVisible = true;
        _fallbackText.Text = snapshot.FakeMachineCodeText is { Length: > 0 } body
            ? body
            : _locale.Panels.FakeMachineCodeWaitingForProgram;
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
            box.Background = on ? StepHighlightBrush : null;
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

        foreach (var line in MacroLayerTeachingFormatters.BuildFakeMachineListing(program))
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
                        ? StudioVisual.TierBrush(AbstractionTier)
                        : StudioVisual.TextPrimaryBrush,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, line.Kind == IlListingLineKind.FunctionHeader ? 6 : 0, 0, 0),
                };
                _listingHost.Children.Add(tb);
            }
        }
    }

    public void ApplyLocale(PipelineSnapshot? lastSnapshot)
    {
        if (_leadBlock is not null)
            _leadBlock.Text = _locale.Panels.FakeMachineCodeLead;
        if (_guideBlock is not null)
            _guideBlock.Text = _locale.Panels.FakeMachineCodeGuide;
        if (_footerBlock is not null)
            _footerBlock.Text = _locale.Panels.FakeMachineCodeFooter;
        if (lastSnapshot is not null)
            OnSnapshotChanged(lastSnapshot);
    }
}
