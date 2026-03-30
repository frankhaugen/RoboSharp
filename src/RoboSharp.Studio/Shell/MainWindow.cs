using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using RoboSharp.Studio.Panels;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.ViewModels;

namespace RoboSharp.Studio.Shell;

public sealed class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;
    private readonly IReadOnlyList<IStudioPanel> _panels;

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
        _viewModel.RunPipeline();
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
        var run = new Button
        {
            Content = "▶  Run pipeline",
            Padding = new Thickness(20, 10),
            CornerRadius = StudioVisual.ButtonRadius,
            Background = StudioVisual.AccentBrush,
            Foreground = new SolidColorBrush(Color.Parse("#0B0F14")),
            FontWeight = FontWeight.SemiBold,
            Cursor = new Cursor(StandardCursorType.Hand),
        };
        run.Bind(Button.CommandProperty, new Binding(nameof(MainWindowViewModel.RefreshCommand)));

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
            Text = "Lexer → Parser → (Semantics • IL • World — coming soon)",
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
                Children = { title, run, subtitle },
            },
        };
    }

    private Control BuildMainWorkspace()
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("280,6,*,6,400"),
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

        grid.Children.Add(BuildInspectorTabs());
        Grid.SetColumn(grid.Children[^1], 4);

        return grid;
    }

    private Control BuildSidebar()
    {
        var body = new TextBlock
        {
            Text =
                "Workspace\n──────────\nOpen project / explorer will plug into RoboSharp.Workspaces + IO abstractions.\n\n" +
                "Didactic layout\n──────────\nEach right-rail tab is an IStudioPanel. Registration order in DI is the pipeline story students see.\n\n" +
                "Try editing the source, then Run pipeline.",
            TextWrapping = TextWrapping.Wrap,
            Foreground = StudioVisual.TextMutedBrush,
            LineHeight = 22,
            FontSize = 13,
        };

        return new Border
        {
            Background = StudioVisual.SurfaceBrush,
            CornerRadius = StudioVisual.PanelRadius,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(16),
            Child = body,
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

    private Control BuildInspectorTabs()
    {
        var tabs = new TabControl
        {
            Background = StudioVisual.SurfaceBrush,
            Foreground = StudioVisual.TextPrimaryBrush,
            CornerRadius = StudioVisual.PanelRadius,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(4),
        };

        foreach (var panel in _panels)
        {
            var item = new TabItem
            {
                Header = panel.DisplayName,
                Content = panel.CreateView(),
                Foreground = StudioVisual.TextPrimaryBrush,
            };
            tabs.Items.Add(item);
        }

        return tabs;
    }

    private void OnPipelineUpdated(PipelineSnapshot snapshot)
    {
        foreach (var panel in _panels)
            panel.OnSnapshotChanged(snapshot);
    }
}
