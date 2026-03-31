using System.IO;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using RoboSharp.Semantics;
using RoboSharp.Studio.Panels;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.ViewModels;
using RoboSharp.World;

namespace RoboSharp.Studio.Shell;

public sealed class MainWindow : Window
{
    private static readonly FilePickerFileType RoboSourceFileType = new("RoboSharp source (.robo)")
    {
        Patterns = ["*.robo"],
    };

    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    private readonly MainWindowViewModel _viewModel;
    private readonly IReadOnlyList<IStudioPanel> _panels;
    private KarelWorldGridView? _karelWorld;
    private bool _closeBypassDirtyCheck;

    public MainWindow(MainWindowViewModel viewModel, IEnumerable<IStudioPanel> panels)
    {
        _viewModel = viewModel;
        _panels = panels.OrderBy(p => p.Order).ToList();

        Width = 1320;
        Height = 840;
        MinWidth = 1024;
        MinHeight = 560;
        Background = StudioVisual.BackgroundDeepBrush;
        FontFamily = StudioVisual.UiFontFamily;
        DataContext = _viewModel;

        Bind(
            TitleProperty,
            new Binding(nameof(MainWindowViewModel.WindowTitle)) { Mode = BindingMode.OneWay });

        Content = BuildRoot();

        AddHandler(KeyDownEvent, OnWindowPreviewKeyDown, RoutingStrategies.Tunnel);

        Closing += OnWindowClosing;

        _viewModel.PipelineUpdated += OnPipelineUpdated;
        _viewModel.RunProgressUpdated += OnRunProgress;
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
        var fileNew = new MenuItem
        {
            Header = "_New",
            HotKey = new KeyGesture(Key.N, KeyModifiers.Control),
        };
        fileNew.Click += (_, _) => _ = NewDocumentWithPromptAsync();

        var fileOpen = new MenuItem
        {
            Header = "_Open…",
            HotKey = new KeyGesture(Key.O, KeyModifiers.Control),
        };
        fileOpen.Click += (_, _) => _ = OpenDocumentWithPromptAsync();

        var fileSave = new MenuItem
        {
            Header = "_Save",
            HotKey = new KeyGesture(Key.S, KeyModifiers.Control),
        };
        fileSave.Click += (_, _) => _ = TrySaveDocumentAsync();

        var fileSaveAs = new MenuItem
        {
            Header = "Save _As…",
            HotKey = new KeyGesture(Key.S, KeyModifiers.Control | KeyModifiers.Shift),
        };
        fileSaveAs.Click += (_, _) => _ = TrySaveAsAsync();

        var fileExit = new MenuItem { Header = "E_xit" };
        fileExit.Click += (_, _) => Close();

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
                    Items =
                    {
                        fileNew,
                        fileOpen,
                        new MenuItem { Header = "-" },
                        fileSave,
                        fileSaveAs,
                        new MenuItem { Header = "-" },
                        fileExit,
                    },
                },
                new MenuItem
                {
                    Header = "_Help",
                    Items = { about },
                },
            },
        };
    }

    private void OnWindowPreviewKeyDown(object? sender, KeyEventArgs e)
    {
        var ctrl = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        if (!ctrl)
            return;

        var shift = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
        if (e.Key == Key.N)
        {
            e.Handled = true;
            _ = NewDocumentWithPromptAsync();
            return;
        }

        if (e.Key == Key.O)
        {
            e.Handled = true;
            _ = OpenDocumentWithPromptAsync();
            return;
        }

        if (e.Key == Key.S)
        {
            e.Handled = true;
            _ = shift ? TrySaveAsAsync() : TrySaveDocumentAsync();
        }
    }

    private async void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        if (_closeBypassDirtyCheck || !_viewModel.IsDirty)
            return;

        e.Cancel = true;
        var choice = await ShowUnsavedChangesDialogAsync();
        switch (choice)
        {
            case UnsavedChoice.Save:
                if (!await TrySaveDocumentAsync())
                    return;
                _closeBypassDirtyCheck = true;
                Close();
                break;
            case UnsavedChoice.Discard:
                _closeBypassDirtyCheck = true;
                Close();
                break;
            case UnsavedChoice.Cancel:
                break;
        }
    }

    private async Task NewDocumentWithPromptAsync()
    {
        if (!await EnsureBufferSavedOrDiscardAsync())
            return;
        _viewModel.NewUntitledDocument();
        _viewModel.Build();
    }

    private async Task OpenDocumentWithPromptAsync()
    {
        if (!await EnsureBufferSavedOrDiscardAsync())
            return;

        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open RoboSharp source",
            AllowMultiple = false,
            FileTypeFilter = [RoboSourceFileType],
        });

        if (files.Count == 0)
            return;

        var file = files[0];
        var path = file.TryGetLocalPath();
        string text;
        try
        {
            if (path is not null)
                text = await File.ReadAllTextAsync(path, Utf8NoBom);
            else
            {
                await using var stream = await file.OpenReadAsync();
                using var reader = new StreamReader(stream, Utf8NoBom, detectEncodingFromByteOrderMarks: true);
                text = await reader.ReadToEndAsync();
            }
        }
        catch (Exception ex)
        {
            await ShowMessageDialogAsync("Open failed", ex.Message);
            return;
        }

        _viewModel.LoadDocument(path, text);
        _viewModel.Build();
    }

    private async Task<bool> EnsureBufferSavedOrDiscardAsync()
    {
        if (!_viewModel.IsDirty)
            return true;

        var choice = await ShowUnsavedChangesDialogAsync();
        return choice switch
        {
            UnsavedChoice.Cancel => false,
            UnsavedChoice.Discard => true,
            UnsavedChoice.Save => await TrySaveDocumentAsync(),
            _ => false,
        };
    }

    private async Task<bool> TrySaveDocumentAsync()
    {
        if (_viewModel.DocumentPath is { } existing)
        {
            try
            {
                await File.WriteAllTextAsync(existing, _viewModel.SourceDocument, Utf8NoBom);
                _viewModel.MarkSavedInPlace();
                return true;
            }
            catch (Exception ex)
            {
                await ShowMessageDialogAsync("Save failed", ex.Message);
                return false;
            }
        }

        return await TrySaveAsAsync();
    }

    private async Task<bool> TrySaveAsAsync()
    {
        var suggested = _viewModel.DocumentPath is { } p
            ? Path.GetFileName(p)
            : "Untitled.robo";

        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save RoboSharp source",
            DefaultExtension = "robo",
            ShowOverwritePrompt = true,
            SuggestedFileName = suggested,
            FileTypeChoices = [RoboSourceFileType],
        });

        if (file is null)
            return false;

        var path = file.TryGetLocalPath();
        if (path is null)
        {
            await ShowMessageDialogAsync(
                "Save failed",
                "Could not resolve a local file path. Try saving to a folder on this computer.");
            return false;
        }

        try
        {
            await File.WriteAllTextAsync(path, _viewModel.SourceDocument, Utf8NoBom);
            _viewModel.MarkSaved(path);
            return true;
        }
        catch (Exception ex)
        {
            await ShowMessageDialogAsync("Save failed", ex.Message);
            return false;
        }
    }

    private async Task ShowMessageDialogAsync(string title, string message)
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
            Title = title,
            Width = 440,
            MinHeight = 160,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Background = StudioVisual.SurfaceBrush,
            Content = new Border
            {
                Padding = new Thickness(24),
                Child = new StackPanel
                {
                    Spacing = 16,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = message,
                            TextWrapping = TextWrapping.Wrap,
                            Foreground = StudioVisual.TextPrimaryBrush,
                            LineHeight = 22,
                        },
                        ok,
                    },
                },
            },
        };

        ok.Click += (_, _) => w.Close();
        await w.ShowDialog(this);
    }

    private async Task<UnsavedChoice> ShowUnsavedChangesDialogAsync()
    {
        var choice = UnsavedChoice.Cancel;

        var save = new Button
        {
            Content = "Save",
            Padding = new Thickness(16, 8),
            CornerRadius = StudioVisual.ButtonRadius,
            Background = StudioVisual.AccentBrush,
            Foreground = new SolidColorBrush(Color.Parse("#0B0F14")),
        };
        var discard = new Button
        {
            Content = "Don't save",
            Padding = new Thickness(16, 8),
            CornerRadius = StudioVisual.ButtonRadius,
            Background = StudioVisual.SurfaceElevatedBrush,
            Foreground = StudioVisual.TextPrimaryBrush,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(1),
        };
        var cancel = new Button
        {
            Content = "Cancel",
            Padding = new Thickness(16, 8),
            CornerRadius = StudioVisual.ButtonRadius,
            Background = StudioVisual.SurfaceElevatedBrush,
            Foreground = StudioVisual.TextPrimaryBrush,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(1),
        };

        var w = new Window
        {
            Title = "RoboSharp Studio",
            Width = 420,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Background = StudioVisual.SurfaceBrush,
            Content = new Border
            {
                Padding = new Thickness(24),
                Child = new StackPanel
                {
                    Spacing = 20,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "Save changes to the current document?",
                            FontSize = 15,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = StudioVisual.TextPrimaryBrush,
                            TextWrapping = TextWrapping.Wrap,
                        },
                        new TextBlock
                        {
                            Text = "Your source buffer has unsaved edits. Choose Save to write the .robo file, Don't save to discard them, or Cancel to stay in the editor.",
                            Foreground = StudioVisual.TextMutedBrush,
                            TextWrapping = TextWrapping.Wrap,
                            LineHeight = 20,
                        },
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing = 10,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Children = { save, discard, cancel },
                        },
                    },
                },
            },
        };

        save.Click += (_, _) =>
        {
            choice = UnsavedChoice.Save;
            w.Close();
        };
        discard.Click += (_, _) =>
        {
            choice = UnsavedChoice.Discard;
            w.Close();
        };
        cancel.Click += (_, _) =>
        {
            choice = UnsavedChoice.Cancel;
            w.Close();
        };

        await w.ShowDialog(this);
        return choice;
    }

    private enum UnsavedChoice
    {
        Cancel,
        Save,
        Discard,
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
            BoxShadow = StudioVisual.ToolbarShadow,
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
            Background = Brushes.Transparent,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(1, 0, 1, 0),
            ResizeDirection = GridResizeDirection.Columns,
        };
        grid.Children.Add(split1);
        Grid.SetColumn(grid.Children[^1], 1);

        grid.Children.Add(BuildEditorPane());
        Grid.SetColumn(grid.Children[^1], 2);

        var split2 = new GridSplitter
        {
            Width = 6,
            Background = Brushes.Transparent,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(1, 0, 1, 0),
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
        var lessonHeading = new TextBlock
        {
            Text = "Lesson & map",
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.AccentBrush,
            Margin = new Thickness(0, 0, 0, 8),
        };

        var profileCaption = new TextBlock
        {
            Text = "Profile (which commands work)",
            FontSize = 11,
            Foreground = StudioVisual.TextMutedBrush,
            Margin = new Thickness(0, 0, 0, 4),
        };

        var profileBox = new ComboBox
        {
            MinWidth = 220,
            Margin = new Thickness(0, 0, 0, 10),
        };
        foreach (var id in LessonBuiltinProfiles.OrderedProfileIds)
            profileBox.Items.Add(new ProfilePick(id, LessonBuiltinProfiles.GetDisplayName(id)));
        profileBox.SelectedItem = profileBox.Items.Cast<ProfilePick>().First(p => p.Id == _viewModel.SelectedProfileId);
        profileBox.SelectionChanged += (_, _) =>
        {
            if (profileBox.SelectedItem is ProfilePick pick)
                _viewModel.SelectedProfileId = pick.Id;
        };

        var worldCaption = new TextBlock
        {
            Text = "World (size & obstacles)",
            FontSize = 11,
            Foreground = StudioVisual.TextMutedBrush,
            Margin = new Thickness(0, 0, 0, 4),
        };

        var worldBox = new ComboBox
        {
            MinWidth = 220,
            Margin = new Thickness(0, 0, 0, 10),
        };
        foreach (var (id, title) in RobotWorldPresets.OrderedPresets)
            worldBox.Items.Add(new WorldPick(id, title));
        worldBox.SelectedItem = worldBox.Items.Cast<WorldPick>().First(p => p.Id == _viewModel.SelectedWorldPresetId);
        worldBox.SelectionChanged += (_, _) =>
        {
            if (worldBox.SelectedItem is WorldPick pick)
                _viewModel.SelectedWorldPresetId = pick.Id;
        };

        var runStatus = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            LineHeight = 18,
            FontSize = 12,
            Foreground = StudioVisual.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 12),
        };
        runStatus.Bind(TextBlock.TextProperty, new Binding(nameof(MainWindowViewModel.LiveRunStatus))
        {
            Mode = BindingMode.OneWay,
        });

        var karelTitle = new TextBlock
        {
            Text = "Karel world",
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.AccentBrush,
            Margin = new Thickness(0, 0, 0, 8),
        };

        _karelWorld = new KarelWorldGridView();

        var hint = new TextBlock
        {
            Text =
                "Tiles: dark = wall, blue-gray = floor, teal tint = goal. Arrows show the robot facing.\n\n" +
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
            Children =
            {
                lessonHeading, profileCaption, profileBox, worldCaption, worldBox, runStatus,
                karelTitle, _karelWorld, hint,
            },
        };

        return new Border
        {
            Background = StudioVisual.SurfaceBrush,
            CornerRadius = StudioVisual.PanelRadius,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(16),
            BoxShadow = StudioVisual.SubtleCardShadow,
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
            BoxShadow = StudioVisual.SubtleCardShadow,
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
            BoxShadow = StudioVisual.SubtleCardShadow,
            Child = scroll,
        };
    }

    private Control BuildPanelCard(IStudioPanel panel)
    {
        var title = new TextBlock
        {
            Text = panel.DisplayName,
            FontSize = 13,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.AccentBrush,
        };

        var header = new StackPanel
        {
            Spacing = 0,
            Margin = new Thickness(0, 0, 0, 8),
        };
        header.Children.Add(title);
        if (panel.InspectorSubtitle is { Length: > 0 } sub)
        {
            header.Children.Add(new TextBlock
            {
                Text = sub,
                FontSize = 11,
                Foreground = StudioVisual.TextMutedBrush,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 16,
                Margin = new Thickness(0, 4, 0, 0),
            });
        }

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
            BoxShadow = StudioVisual.SubtleCardShadow,
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

    private void OnRunProgress(StudioRunProgress progress) =>
        ApplyKarelSnapshot(progress.World);

    private void ApplyKarelSnapshot(RobotWorldSnapshot snapshot)
    {
        if (_karelWorld is null)
            return;
        _karelWorld.Update(snapshot);
    }

    private sealed record ProfilePick(string Id, string Title)
    {
        public override string ToString() => Title;
    }

    private sealed record WorldPick(string Id, string Title)
    {
        public override string ToString() => Title;
    }
}
