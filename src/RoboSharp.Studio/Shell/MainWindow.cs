using System.IO;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Platform.Storage;
using RoboSharp.Locales;
using RoboSharp.Semantics;
using RoboSharp.Studio.Editor;
using RoboSharp.Studio.Panels;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.ViewModels;
using RoboSharp.World;

namespace RoboSharp.Studio.Shell;

public sealed class MainWindow : Window
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    private readonly MainWindowViewModel _viewModel;
    private readonly StudioLocaleHost _locale;
    private readonly IReadOnlyList<IStudioPanel> _panels;
    private FilePickerFileType _roboSourceFileType;
    private Grid? _chromeGrid;
    private Grid? _workspaceGrid;
    private Border? _sidebarBorder;
    private Border? _inspectorBorder;
    private PipelineSnapshot? _lastPipelineSnapshot;
    private RobotWorldGridView? _worldGridPreview;
    private RoboSharpSourceEditor? _sourceEditor;
    private bool _closeBypassDirtyCheck;

    public MainWindow(MainWindowViewModel viewModel, StudioLocaleHost locale, IEnumerable<IStudioPanel> panels)
    {
        _viewModel = viewModel;
        _locale = locale;
        _roboSourceFileType = new FilePickerFileType(locale.Shell.RoboFileTypeDescription)
        {
            Patterns = ["*.robo"],
        };
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
        _locale.Changed += OnTeachingLocaleChanged;
        _viewModel.Build();
    }

    private Control BuildRoot()
    {
        var grid = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,Auto,*"),
        };
        _chromeGrid = grid;

        var menu = BuildMenu();
        grid.Children.Add(menu);
        Grid.SetRow(menu, 0);

        var toolbar = BuildToolbar();
        grid.Children.Add(toolbar);
        Grid.SetRow(toolbar, 1);

        _workspaceGrid = BuildMainWorkspace();
        grid.Children.Add(_workspaceGrid);
        Grid.SetRow(_workspaceGrid, 2);

        return grid;
    }

    private void OnTeachingLocaleChanged(object? sender, EventArgs e)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Post(() => OnTeachingLocaleChanged(sender, e));
            return;
        }

        _roboSourceFileType = new FilePickerFileType(_locale.Shell.RoboFileTypeDescription)
        {
            Patterns = ["*.robo"],
        };

        if (_chromeGrid is not null)
        {
            ReplaceChromeRow(_chromeGrid, 0, BuildMenu());
            ReplaceChromeRow(_chromeGrid, 1, BuildToolbar());
        }

        if (_workspaceGrid is not null)
        {
            var sb = (Border)BuildSidebar();
            ReplaceChildAtGridColumn(_workspaceGrid, 0, sb);
            _sidebarBorder = sb;

            var ins = (Border)BuildInspectorColumn();
            ReplaceChildAtGridColumn(_workspaceGrid, 4, ins);
            _inspectorBorder = ins;
        }

        _viewModel.ApplyLocaleRefresh();

        if (_lastPipelineSnapshot is { } snap)
        {
            foreach (var p in _panels)
                p.ApplyLocale(snap);
            _sourceEditor?.ApplyDiagnosticSpans(snap.SourceDiagnosticSpans);
            if (snap.WorldVisualization is { } w)
                ApplyWorldGridPreviewSnapshot(w);
        }
        else
            _viewModel.Build();
    }

    private static void ReplaceChromeRow(Grid grid, int row, Control replacement)
    {
        for (var i = grid.Children.Count - 1; i >= 0; i--)
        {
            if (Grid.GetRow(grid.Children[i]) == row)
            {
                grid.Children.RemoveAt(i);
                break;
            }
        }

        grid.Children.Add(replacement);
        Grid.SetRow(replacement, row);
    }

    private static void ReplaceChildAtGridColumn(Grid grid, int column, Control replacement)
    {
        for (var i = grid.Children.Count - 1; i >= 0; i--)
        {
            if (Grid.GetColumn(grid.Children[i]) == column)
            {
                grid.Children.RemoveAt(i);
                break;
            }
        }

        grid.Children.Add(replacement);
        Grid.SetColumn(replacement, column);
    }

    private Menu BuildMenu()
    {
        var fileNew = new MenuItem
        {
            Header = _locale.Shell.MenuNew,
            HotKey = new KeyGesture(Key.N, KeyModifiers.Control),
        };
        fileNew.Click += (_, _) => _ = NewDocumentWithPromptAsync();

        var fileOpen = new MenuItem
        {
            Header = _locale.Shell.MenuOpen,
            HotKey = new KeyGesture(Key.O, KeyModifiers.Control),
        };
        fileOpen.Click += (_, _) => _ = OpenDocumentWithPromptAsync();

        var fileSave = new MenuItem
        {
            Header = _locale.Shell.MenuSave,
            HotKey = new KeyGesture(Key.S, KeyModifiers.Control),
        };
        fileSave.Click += (_, _) => _ = TrySaveDocumentAsync();

        var fileSaveAs = new MenuItem
        {
            Header = _locale.Shell.MenuSaveAs,
            HotKey = new KeyGesture(Key.S, KeyModifiers.Control | KeyModifiers.Shift),
        };
        fileSaveAs.Click += (_, _) => _ = TrySaveAsAsync();

        var fileExit = new MenuItem { Header = _locale.Shell.MenuExit };
        fileExit.Click += (_, _) => Close();

        var about = new MenuItem { Header = _locale.Shell.MenuAbout };
        about.Click += (_, _) => ShowAbout();

        var langEnglish = new MenuItem { Header = _locale.Shell.LanguageEnglishMenuLabel };
        langEnglish.Click += (_, _) => _locale.SetLocaleId("en");

        var langLatin = new MenuItem { Header = _locale.Shell.LanguageLatinMenuLabel };
        langLatin.Click += (_, _) => _locale.SetLocaleId("la");

        var languageMenu = new MenuItem
        {
            Header = _locale.Shell.LanguageMenuHeader,
            Items = { langEnglish, langLatin },
        };

        var settingsMenu = new MenuItem
        {
            Header = _locale.Shell.SettingsMenuHeader,
            Items = { languageMenu },
        };

        return new Menu
        {
            Background = StudioVisual.SurfaceBrush,
            Foreground = StudioVisual.TextPrimaryBrush,
            Items =
            {
                new MenuItem
                {
                    Header = _locale.Shell.FileMenuHeader,
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
                settingsMenu,
                new MenuItem
                {
                    Header = _locale.Shell.HelpMenuHeader,
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
            Title = _locale.Shell.OpenFilePickerTitle,
            AllowMultiple = false,
            FileTypeFilter = [_roboSourceFileType],
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
            await ShowMessageDialogAsync(_locale.Shell.OpenFailedTitle, ex.Message);
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
                await ShowMessageDialogAsync(_locale.Shell.SaveFailedTitle, ex.Message);
                return false;
            }
        }

        return await TrySaveAsAsync();
    }

    private async Task<bool> TrySaveAsAsync()
    {
        var suggested = _viewModel.DocumentPath is { } p
            ? Path.GetFileName(p)
            : _locale.Shell.UntitledFileName;

        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = _locale.Shell.SaveFilePickerTitle,
            DefaultExtension = "robo",
            ShowOverwritePrompt = true,
            SuggestedFileName = suggested,
            FileTypeChoices = [_roboSourceFileType],
        });

        if (file is null)
            return false;

        var path = file.TryGetLocalPath();
        if (path is null)
        {
            await ShowMessageDialogAsync(
                _locale.Shell.SaveFailedTitle,
                _locale.Shell.SaveNoLocalPathMessage);
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
            await ShowMessageDialogAsync(_locale.Shell.SaveFailedTitle, ex.Message);
            return false;
        }
    }

    private async Task ShowMessageDialogAsync(string title, string message)
    {
        var ok = new Button
        {
            Content = _locale.Shell.DialogOk,
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
            Content = _locale.Shell.ButtonSave,
            Padding = new Thickness(16, 8),
            CornerRadius = StudioVisual.ButtonRadius,
            Background = StudioVisual.AccentBrush,
            Foreground = new SolidColorBrush(Color.Parse("#0B0F14")),
        };
        var discard = new Button
        {
            Content = _locale.Shell.ButtonDontSave,
            Padding = new Thickness(16, 8),
            CornerRadius = StudioVisual.ButtonRadius,
            Background = StudioVisual.SurfaceElevatedBrush,
            Foreground = StudioVisual.TextPrimaryBrush,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(1),
        };
        var cancel = new Button
        {
            Content = _locale.Shell.ButtonCancel,
            Padding = new Thickness(16, 8),
            CornerRadius = StudioVisual.ButtonRadius,
            Background = StudioVisual.SurfaceElevatedBrush,
            Foreground = StudioVisual.TextPrimaryBrush,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(1),
        };

        var w = new Window
        {
            Title = _locale.Shell.UnsavedDialogTitle,
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
                            Text = _locale.Shell.UnsavedDialogHeading,
                            FontSize = 15,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = StudioVisual.TextPrimaryBrush,
                            TextWrapping = TextWrapping.Wrap,
                        },
                        new TextBlock
                        {
                            Text = _locale.Shell.UnsavedDialogBody,
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
            Content = _locale.Shell.DialogOk,
            HorizontalAlignment = HorizontalAlignment.Right,
            Padding = new Thickness(20, 8),
            CornerRadius = StudioVisual.ButtonRadius,
            Background = StudioVisual.AccentBrush,
            Foreground = new SolidColorBrush(Color.Parse("#0B0F14")),
        };

        var w = new Window
        {
            Title = _locale.Shell.AboutTitle,
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
                            Text = _locale.Shell.AboutAppName,
                            FontSize = 22,
                            FontWeight = FontWeight.Bold,
                            Foreground = StudioVisual.AccentBrush,
                        },
                        new TextBlock
                        {
                            Text = _locale.Shell.AboutBody,
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
            Content = _locale.Shell.ToolbarBuild,
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
            Content = _locale.Shell.ToolbarRun,
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
            Text = _locale.Shell.ToolbarStepSpeed,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = StudioVisual.TextMutedBrush,
            FontSize = 13,
        };

        var speedBox = new ComboBox
        {
            MinWidth = 120,
            ItemsSource = _viewModel.RunSpeedOptions,
            VerticalAlignment = VerticalAlignment.Center,
        };
        speedBox.Bind(ComboBox.SelectedItemProperty, new Binding(nameof(MainWindowViewModel.SelectedRunSpeedOption))
        {
            Mode = BindingMode.TwoWay,
        });

        var title = new TextBlock
        {
            Text = _locale.Shell.ToolbarAppTitle,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 20,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.AccentBrush,
            Margin = new Thickness(0, 0, 24, 0),
        };

        var subtitle = new TextBlock
        {
            Text = _locale.Shell.ToolbarSubtitle,
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

    private Grid BuildMainWorkspace()
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("280,6,*,6,460"),
            Margin = new Thickness(12),
        };

        _sidebarBorder = BuildSidebar();
        grid.Children.Add(_sidebarBorder);
        Grid.SetColumn(_sidebarBorder, 0);

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

        _inspectorBorder = BuildInspectorColumn();
        grid.Children.Add(_inspectorBorder);
        Grid.SetColumn(_inspectorBorder, 4);

        return grid;
    }

    private Border BuildSidebar()
    {
        var lessonHeading = new TextBlock
        {
            Text = _locale.Sidebar.LessonAndMapHeading,
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.AccentBrush,
            Margin = new Thickness(0, 0, 0, 8),
        };

        var profileCaption = new TextBlock
        {
            Text = _locale.Sidebar.ProfileCaption,
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
            Text = _locale.Sidebar.WorldCaption,
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

        var worldPreviewTitle = new TextBlock
        {
            Text = _locale.Sidebar.WorldPreviewHeading,
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.AccentBrush,
            Margin = new Thickness(0, 0, 0, 8),
        };

        _worldGridPreview = new RobotWorldGridView();

        var hint = new TextBlock
        {
            Text = _locale.Sidebar.WorldPreviewHint,
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
                worldPreviewTitle, _worldGridPreview, hint,
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
        _sourceEditor = new RoboSharpSourceEditor
        {
            MinHeight = 360,
        };
        _sourceEditor.SetDocumentText(_viewModel.SourceDocument, suspendEvents: true);
        _sourceEditor.TextChanged += text =>
        {
            if (_viewModel.SourceDocument != text)
                _viewModel.SourceDocument = text;
        };
        _viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(MainWindowViewModel.SourceDocument))
                _sourceEditor?.SetDocumentText(_viewModel.SourceDocument, suspendEvents: true);
        };

        return _sourceEditor;
    }

    private Border BuildInspectorColumn()
    {
        var tabs = new TabControl
        {
            TabStripPlacement = Dock.Left,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        foreach (var panel in _panels)
        {
            tabs.Items.Add(new TabItem
            {
                Header = panel.DisplayName,
                Content = BuildInspectorTabContent(panel),
            });
        }

        return new Border
        {
            Background = StudioVisual.SurfaceBrush,
            CornerRadius = StudioVisual.PanelRadius,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(8),
            BoxShadow = StudioVisual.SubtleCardShadow,
            Child = tabs,
        };
    }

    private static Control BuildInspectorTabContent(IStudioPanel panel)
    {
        var body = panel.CreateView();
        var bodyScroll = new ScrollViewer
        {
            Content = body,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
        };

        var inner = new Grid();
        if (panel.InspectorSubtitle is { Length: > 0 } sub)
        {
            inner.RowDefinitions = new RowDefinitions("Auto,*");
            var subtitle = new TextBlock
            {
                Text = sub,
                FontSize = 11,
                Foreground = StudioVisual.TextMutedBrush,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 16,
                Margin = new Thickness(0, 0, 0, 8),
            };
            inner.Children.Add(subtitle);
            Grid.SetRow(subtitle, 0);
            inner.Children.Add(bodyScroll);
            Grid.SetRow(bodyScroll, 1);
        }
        else
        {
            inner.RowDefinitions = new RowDefinitions("*");
            inner.Children.Add(bodyScroll);
            Grid.SetRow(bodyScroll, 0);
        }

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
        _lastPipelineSnapshot = snapshot;
        foreach (var panel in _panels)
            panel.OnSnapshotChanged(snapshot);

        _sourceEditor?.ApplyDiagnosticSpans(snapshot.SourceDiagnosticSpans);

        if (snapshot.WorldVisualization is { } w)
            ApplyWorldGridPreviewSnapshot(w);
    }

    private void OnRunProgress(StudioRunProgress progress)
    {
        ApplyWorldGridPreviewSnapshot(progress.World);
        foreach (var panel in _panels)
            panel.OnRunProgress(progress);
    }

    private void ApplyWorldGridPreviewSnapshot(RobotWorldSnapshot snapshot)
    {
        if (_worldGridPreview is null)
            return;
        _worldGridPreview.Update(snapshot);
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
