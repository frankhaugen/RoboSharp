using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using RoboSharp.Studio.Panels;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.ViewModels;
using RoboSharp.World;

namespace RoboSharp.Studio.Shell;

public sealed class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;
    private readonly IReadOnlyList<IStudioPanel> _panels;
    private TextBlock? _karelAscii;

    public MainWindow(MainWindowViewModel viewModel, IEnumerable<IStudioPanel> panels)
    {
        _viewModel = viewModel;
        _panels = panels.OrderBy(p => p.Order).ToList();

        Title = "RoboSharp Studio";
        Width = 1320;
        Height = 840;
        MinWidth = 1024;
        MinHeight = 560;
        Background = StudioVisual.BackgroundDeepBrush;
        FontFamily = StudioVisual.UiFontFamily;
        DataContext = _viewModel;

        Content = BuildRoot();

        _viewModel.PipelineUpdated += OnPipelineUpdated;
        _viewModel.KarelFrameUpdated += OnKarelFrameUpdated;
        _viewModel.Build();
    }

    private Control BuildRoot()
    {
        var grid = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,Auto,*"),
        };

        grid.Children.Add(BuildMenu());
        Grid.SetRow(grid.Children[^1], 0);

        grid.Children.Add(BuildToolbar());
        Grid.SetRow(grid.Children[^1], 1);

        grid.Children.Add(BuildMainWorkspace());
        Grid.SetRow(grid.Children[^1], 2);

        return grid;
    }

    private Menu BuildMenu()
    {
        var exit = new MenuItem { Header = "E_xit" };
        exit.Click += (_, _) => Close();

        var about = new MenuItem { Header = "_About…" };
        about.Click += (_, _) => ShowAbout();

        return new Menu
        {
            Background = StudioVisual.SurfaceBrush,
            Foreground = StudioVisual.TextPrimaryBrush,
            Items =
            {
                new MenuItem
                {
                    Header = "_File",
                    Items = { exit },
                },
                new MenuItem
                {
                    Header = "_Help",
                    Items = { about },
                },
            },
        };
    }

    private void ShowAbout()
    {
        var ok = new Button
        {
            Content = "OK",
            HorizontalAlignment = HorizontalAlignment.Right,
            Padding = new Thickness(20, 8),
            CornerRadius = StudioVisual.ButtonRadius,
            Background = StudioVisual.AccentBrush,
            Foreground = new SolidColorBrush(Color.Parse("#0B0F14")),
        };

        var w = new Window
        {
            Title = "About",
            Width = 420,
            Height = 220,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Background = StudioVisual.SurfaceBrush,
            Content = new Border
            {
                Padding = new Thickness(24),
                Child = new StackPanel
                {
                    Spacing = 12,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "RoboSharp Studio",
                            FontSize = 22,
                            FontWeight = FontWeight.Bold,
                            Foreground = StudioVisual.AccentBrush,
                        },
                        new TextBlock
                        {
                            Text = "Teaching IDE host — modular pipeline panels, code-first Avalonia UI.\nSee docs/studio/ for the full specification.",
                            TextWrapping = TextWrapping.Wrap,
                            Foreground = StudioVisual.TextMutedBrush,
                            LineHeight = 22,
                        },
                        ok,
                    },
                },
            },
        };

        ok.Click += (_, _) => w.Close();
        w.ShowDialog(this);
    }

    private Control BuildToolbar()
    {
        var build = new Button
        {
            Content = "Build",
            Padding = new Thickness(16, 10),
            CornerRadius = StudioVisual.ButtonRadius,
            Background = StudioVisual.SurfaceElevatedBrush,
            Foreground = StudioVisual.TextPrimaryBrush,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(1),
            FontWeight = FontWeight.SemiBold,
            Cursor = new Cursor(StandardCursorType.Hand),
        };
        build.Bind(Button.CommandProperty, new Binding(nameof(MainWindowViewModel.BuildCommand)));

        var run = new Button
        {
            Content = "▶  Run",
            Padding = new Thickness(20, 10),
            CornerRadius = StudioVisual.ButtonRadius,
            Background = StudioVisual.AccentBrush,
            Foreground = new SolidColorBrush(Color.Parse("#0B0F14")),
            FontWeight = FontWeight.SemiBold,
            Cursor = new Cursor(StandardCursorType.Hand),
        };
        run.Bind(Button.CommandProperty, new Binding(nameof(MainWindowViewModel.RunCommand)));

        var speedLabel = new TextBlock
        {
            Text = "Step speed",
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = StudioVisual.TextMutedBrush,
            FontSize = 13,
        };

        var speedBox = new ComboBox
        {
            MinWidth = 120,
            ItemsSource = Enum.GetValues<StudioRunSpeed>(),
            VerticalAlignment = VerticalAlignment.Center,
        };
        speedBox.Bind(ComboBox.SelectedItemProperty, new Binding(nameof(MainWindowViewModel.SelectedRunSpeed))
        {
            Mode = BindingMode.TwoWay,
        });

        var title = new TextBlock
        {
            Text = "RoboSharp Studio",
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 20,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.AccentBrush,
            Margin = new Thickness(0, 0, 24, 0),
        };

        var subtitle = new TextBlock
        {
            Text = "Karel grid (left) · Build = compile only · Run = compile then step interpreter (see speed)",
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = StudioVisual.TextMutedBrush,
            FontSize = 13,
        };

        return new Border
        {
            Background = StudioVisual.SurfaceBrush,
            BorderThickness = new Thickness(0, 0, 0, 1),
            BorderBrush = StudioVisual.BorderSubtleBrush,
            Padding = new Thickness(16, 10),
            Child = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 16,
                Children = { title, build, run, speedLabel, speedBox, subtitle },
            },
        };
    }

    private Control BuildMainWorkspace()
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("280,6,*,6,460"),
            Margin = new Thickness(12),
        };

        grid.Children.Add(BuildSidebar());
        Grid.SetColumn(grid.Children[^1], 0);

        var split1 = new GridSplitter
        {
            Width = 6,
            Background = StudioVisual.BorderSubtleBrush,
            ResizeDirection = GridResizeDirection.Columns,
        };
        grid.Children.Add(split1);
        Grid.SetColumn(grid.Children[^1], 1);

        grid.Children.Add(BuildEditorPane());
        Grid.SetColumn(grid.Children[^1], 2);

        var split2 = new GridSplitter
        {
            Width = 6,
            Background = StudioVisual.BorderSubtleBrush,
            ResizeDirection = GridResizeDirection.Columns,
        };
        grid.Children.Add(split2);
        Grid.SetColumn(grid.Children[^1], 3);

        grid.Children.Add(BuildInspectorColumn());
        Grid.SetColumn(grid.Children[^1], 4);

        return grid;
    }

    private Control BuildSidebar()
    {
        var karelTitle = new TextBlock
        {
            Text = "Karel world",
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.AccentBrush,
            Margin = new Thickness(0, 0, 0, 8),
        };

        _karelAscii = new TextBlock
        {
            Text = "",
            FontFamily = StudioVisual.CodeFontFamily,
            FontSize = 11,
            Foreground = StudioVisual.TextPrimaryBrush,
            TextWrapping = TextWrapping.NoWrap,
        };

        var hint = new TextBlock
        {
            Text =
                "Symbols: # wall  . floor  * goal  ^>v< robot facing N/E/S/W.\n\n" +
                "Build refreshes compile stages; Run compiles again then animates the robot. " +
                "Try Realtime for instant finish, Slow or Glacial to watch each IL step.",
            TextWrapping = TextWrapping.Wrap,
            Foreground = StudioVisual.TextMutedBrush,
            LineHeight = 20,
            FontSize = 12,
            Margin = new Thickness(0, 16, 0, 0),
        };

        var stack = new StackPanel
        {
            Spacing = 0,
            Children = { karelTitle, _karelAscii, hint },
        };

        return new Border
        {
            Background = StudioVisual.SurfaceBrush,
            CornerRadius = StudioVisual.PanelRadius,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(16),
            Child = stack,
        };
    }

    private Control BuildEditorPane()
    {
        var editor = new TextBox
        {
            AcceptsReturn = true,
            Watermark = "Source (.robo)",
            FontFamily = StudioVisual.CodeFontFamily,
            FontSize = 13,
            Foreground = StudioVisual.TextPrimaryBrush,
            Background = Brushes.Transparent,
            CaretBrush = StudioVisual.AccentBrush,
            SelectionBrush = new SolidColorBrush(StudioVisual.Accent) { Opacity = 0.35 },
        };
        editor.Bind(TextBox.TextProperty, new Binding(nameof(MainWindowViewModel.SourceDocument))
        {
            Mode = BindingMode.TwoWay,
        });

        return new Border
        {
            Background = StudioVisual.SurfaceElevatedBrush,
            CornerRadius = StudioVisual.PanelRadius,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(12),
            Child = editor,
        };
    }

    private Control BuildInspectorColumn()
    {
        var stack = new StackPanel
        {
            Spacing = 10,
        };

        foreach (var panel in _panels)
            stack.Children.Add(BuildPanelCard(panel));

        var scroll = new ScrollViewer
        {
            Content = stack,
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
        };

        return new Border
        {
            Background = StudioVisual.SurfaceBrush,
            CornerRadius = StudioVisual.PanelRadius,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(8),
            Child = scroll,
        };
    }

    private Control BuildPanelCard(IStudioPanel panel)
    {
        var header = new TextBlock
        {
            Text = panel.DisplayName,
            FontSize = 13,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.AccentBrush,
            Margin = new Thickness(0, 0, 0, 8),
        };

        var body = panel.CreateView();
        var bodyScroll = new ScrollViewer
        {
            Content = body,
            MaxHeight = 280,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
        };

        var inner = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*"),
        };
        inner.Children.Add(header);
        Grid.SetRow(header, 0);
        inner.Children.Add(bodyScroll);
        Grid.SetRow(bodyScroll, 1);

        return new Border
        {
            Background = StudioVisual.SurfaceElevatedBrush,
            CornerRadius = StudioVisual.PanelRadius,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(12),
            Child = inner,
        };
    }

    private void OnPipelineUpdated(PipelineSnapshot snapshot)
    {
        foreach (var panel in _panels)
            panel.OnSnapshotChanged(snapshot);

        if (snapshot.WorldVisualization is { } w)
            ApplyKarelSnapshot(w);
    }

    private void OnKarelFrameUpdated(RobotWorldSnapshot snapshot) =>
        ApplyKarelSnapshot(snapshot);

    private void ApplyKarelSnapshot(RobotWorldSnapshot snapshot)
    {
        if (_karelAscii is null)
            return;
        _karelAscii.Text = KarelWorldAsciiFormatter.Format(snapshot);
    }
}
