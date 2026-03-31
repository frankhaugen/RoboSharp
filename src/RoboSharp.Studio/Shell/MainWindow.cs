using System;
using System.Collections.Generic;
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
using RoboSharp.Studio.Editor;
using RoboSharp.Studio.Panels;
using RoboSharp.Application.Teaching;
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
    private StackPanel? _inspectorPanelStack;
    private readonly DiagnosticsPipelinePanel _diagnosticsPanel;
    private readonly WorldRuntimePipelinePanel _worldRuntimePanel;
    private Window? _diagnosticsFlyout;
    private Window? _runReportFlyout;
    private bool _closeBypassDirtyCheck;
    private readonly List<(string Id, Button Btn)> _lessonRibbonEntries = new();
    private ComboBox? _speedCombo;

    public MainWindow(MainWindowViewModel viewModel, StudioLocaleHost locale, IEnumerable<IStudioPanel> panels)
    {
        _viewModel = viewModel;
        _locale = locale;
        _roboSourceFileType = new FilePickerFileType(locale.Shell.RoboFileTypeDescription)
        {
            Patterns = ["*.robo"],
        };
        _panels = panels.OrderBy(p => p.Order).ToList();
        _diagnosticsPanel = _panels.OfType<DiagnosticsPipelinePanel>().Single();
        _worldRuntimePanel = _panels.OfType<WorldRuntimePipelinePanel>().Single();

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

        _viewModel.PropertyChanged += OnViewModelPropertyChangedForShellCombos;

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
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto,*"),
        };
        _chromeGrid = grid;

        var menu = BuildMenu();
        grid.Children.Add(menu);
        Grid.SetRow(menu, 0);

        var ribbon = BuildLessonRibbon();
        grid.Children.Add(ribbon);
        Grid.SetRow(ribbon, 1);

        var toolbar = BuildToolbar();
        grid.Children.Add(toolbar);
        Grid.SetRow(toolbar, 2);

        _workspaceGrid = BuildMainWorkspace();
        grid.Children.Add(_workspaceGrid);
        Grid.SetRow(_workspaceGrid, 3);

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

            var editor = BuildEditorPane();
            ReplaceChildAtGridColumn(_workspaceGrid, 2, editor);

            var ins = (Border)BuildInspectorColumn();
            ReplaceChildAtGridColumn(_workspaceGrid, 4, ins);
            _inspectorBorder = ins;
        }

        _diagnosticsFlyout?.Close();
        _runReportFlyout?.Close();

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

        RefreshFlyoutsIfOpen();
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

        var viewDiagnostics = new MenuItem { Header = _locale.Shell.MenuViewCompilerDiagnostics };
        viewDiagnostics.Click += (_, _) => OpenDiagnosticsFlyout();

        var viewRunReport = new MenuItem { Header = _locale.Shell.MenuViewRunReport };
        viewRunReport.Click += (_, _) => OpenRunReportFlyout();

        var viewMenu = new MenuItem
        {
            Header = _locale.Shell.ViewMenuHeader,
            Items = { viewDiagnostics, viewRunReport },
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
                viewMenu,
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

    private void OnViewModelPropertyChangedForShellCombos(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(MainWindowViewModel.SelectedLessonId):
                SyncLessonRibbonFromViewModel();
                PopulateInspectorStack();
                break;
        }
    }

    private void SyncLessonRibbonFromViewModel()
    {
        if (_lessonRibbonEntries.Count == 0)
            return;
        var accent = StudioVisual.AccentBrush;
        var rest = StudioVisual.SurfaceElevatedBrush;
        var onFg = new SolidColorBrush(Color.Parse("#0B0F14"));
        foreach (var (lessonId, btn) in _lessonRibbonEntries)
        {
            var on = string.Equals(lessonId, _viewModel.SelectedLessonId, StringComparison.OrdinalIgnoreCase);
            btn.Background = on ? accent : rest;
            btn.Foreground = on ? onFg : StudioVisual.TextPrimaryBrush;
            btn.FontWeight = on ? FontWeight.SemiBold : FontWeight.Normal;
        }
    }

    private Control BuildLessonRibbon()
    {
        _lessonRibbonEntries.Clear();

        var hint = new TextBlock
        {
            Text = _locale.Sidebar.LessonRibbonSubtitle,
            FontSize = 11,
            Foreground = StudioVisual.TextMutedBrush,
            TextWrapping = TextWrapping.Wrap,
            LineHeight = 16,
            Margin = new Thickness(0, 0, 0, 8),
        };

        var strip = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
        };

        foreach (var L in _locale.Lessons.OrderedLessons)
        {
            var id = L.Id;
            var btn = new Button
            {
                Content = L.Title,
                Padding = new Thickness(14, 8),
                CornerRadius = StudioVisual.ButtonRadius,
                BorderBrush = StudioVisual.BorderSubtleBrush,
                BorderThickness = new Thickness(1),
                Cursor = new Cursor(StandardCursorType.Hand),
            };
            btn.Click += (_, _) => _viewModel.SelectedLessonId = id;
            _lessonRibbonEntries.Add((id, btn));
            strip.Children.Add(btn);
        }

        var scroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
            Content = strip,
        };

        var stack = new StackPanel
        {
            Spacing = 0,
            Children = { hint, scroll },
        };

        SyncLessonRibbonFromViewModel();

        return new Border
        {
            Background = StudioVisual.SurfaceElevatedBrush,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(16, 10, 16, 8),
            Child = stack,
        };
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
            MinWidth = 96,
            MaxWidth = 220,
            ItemsSource = _viewModel.RunSpeedOptions,
            VerticalAlignment = VerticalAlignment.Center,
        };
        speedBox.Bind(ComboBox.SelectedItemProperty, new Binding(nameof(MainWindowViewModel.SelectedRunSpeedOption))
        {
            Mode = BindingMode.TwoWay,
        });
        _speedCombo = speedBox;
        speedBox.SelectionChanged += (_, _) =>
        {
            if (speedBox.SelectedItem is RunSpeedOption o)
                ToolTip.SetTip(speedBox, o.FullCaption);
        };

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
            TextWrapping = TextWrapping.Wrap,
            Foreground = StudioVisual.TextMutedBrush,
            FontSize = 13,
            LineHeight = 18,
            Margin = new Thickness(0, 6, 0, 0),
        };

        var topRow = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 16,
            Children = { title, build, run, speedLabel, speedBox },
        };

        var grid = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,Auto"),
        };
        grid.Children.Add(topRow);
        Grid.SetRow(topRow, 0);
        grid.Children.Add(subtitle);
        Grid.SetRow(subtitle, 1);

        if (speedBox.SelectedItem is RunSpeedOption initial)
            ToolTip.SetTip(speedBox, initial.FullCaption);

        return new Border
        {
            Background = StudioVisual.SurfaceBrush,
            BorderThickness = new Thickness(0, 0, 0, 1),
            BorderBrush = StudioVisual.BorderSubtleBrush,
            Padding = new Thickness(16, 10),
            BoxShadow = StudioVisual.ToolbarShadow,
            Child = grid,
        };
    }

    private Grid BuildMainWorkspace()
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("328,6,*,6,520"),
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
        var taskHeading = new TextBlock
        {
            Text = _locale.Sidebar.LessonTaskHeading,
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.AccentBrush,
            Margin = new Thickness(0, 0, 0, 8),
        };

        var taskBody = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            LineHeight = 18,
            FontSize = 12,
            Foreground = StudioVisual.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 8),
        };
        taskBody.Bind(TextBlock.TextProperty, new Binding(nameof(MainWindowViewModel.CurrentLessonTaskChallengeBody)));

        var loadExample = new Button
        {
            Content = _locale.Sidebar.LoadLessonExampleButton,
            Margin = new Thickness(0, 0, 0, 16),
            Padding = new Thickness(12, 8),
            CornerRadius = StudioVisual.ButtonRadius,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = StudioVisual.SurfaceElevatedBrush,
            Foreground = StudioVisual.TextPrimaryBrush,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(1),
            Cursor = new Cursor(StandardCursorType.Hand),
        };
        loadExample.Bind(Button.CommandProperty, new Binding(nameof(MainWindowViewModel.LoadLessonExampleCommand)));

        var goalSectionHeading = new TextBlock
        {
            Text = _locale.Sidebar.LessonSectionGoalHeading,
            FontSize = 13,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.AccentBrush,
            Margin = new Thickness(0, 0, 0, 6),
        };

        var goalSectionBody = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            LineHeight = 18,
            FontSize = 11,
            Foreground = StudioVisual.TextMutedBrush,
            Margin = new Thickness(0, 0, 0, 8),
        };
        goalSectionBody.Bind(TextBlock.TextProperty, new Binding(nameof(MainWindowViewModel.CurrentLessonGoalSectionBody)));

        var worldLabel = new TextBlock
        {
            Text = _locale.Sidebar.LessonWorldNameLabel,
            FontSize = 10,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.TextMutedBrush,
            Margin = new Thickness(0, 0, 0, 2),
        };

        var worldName = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            LineHeight = 20,
            FontSize = 13,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 8),
        };
        worldName.Bind(TextBlock.TextProperty, new Binding(nameof(MainWindowViewModel.CurrentLessonWorldDisplayName)));

        var commandsSectionHeading = new TextBlock
        {
            Text = _locale.Sidebar.LessonSectionCommandsHeading,
            FontSize = 13,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.AccentBrush,
            Margin = new Thickness(0, 16, 0, 6),
        };

        var commandsSectionBody = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            LineHeight = 18,
            FontSize = 11,
            Foreground = StudioVisual.TextMutedBrush,
            Margin = new Thickness(0, 0, 0, 8),
        };
        commandsSectionBody.Bind(TextBlock.TextProperty, new Binding(nameof(MainWindowViewModel.CurrentLessonCommandsSectionBody)));

        var profileLabel = new TextBlock
        {
            Text = _locale.Sidebar.LessonProfileNameLabel,
            FontSize = 10,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.TextMutedBrush,
            Margin = new Thickness(0, 0, 0, 2),
        };

        var profileName = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            LineHeight = 20,
            FontSize = 13,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 12),
        };
        profileName.Bind(TextBlock.TextProperty, new Binding(nameof(MainWindowViewModel.CurrentLessonProfileDisplayName)));

        var referenceHeading = new TextBlock
        {
            Text = _locale.Sidebar.LessonSectionReferenceHeading,
            FontSize = 13,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.AccentBrush,
            Margin = new Thickness(0, 0, 0, 6),
        };

        var kwHeading = new TextBlock
        {
            Text = _locale.Sidebar.KeywordsHeading,
            FontSize = 12,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 4),
        };

        var kwBody = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            LineHeight = 18,
            FontSize = 11,
            Foreground = StudioVisual.TextMutedBrush,
            Margin = new Thickness(0, 0, 0, 10),
        };
        kwBody.Bind(TextBlock.TextProperty, new Binding(nameof(MainWindowViewModel.CurrentLessonKeywords)));

        var synHeading = new TextBlock
        {
            Text = _locale.Sidebar.SyntaxHeading,
            FontSize = 12,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 4),
        };

        var synBody = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            LineHeight = 18,
            FontSize = 11,
            Foreground = StudioVisual.TextMutedBrush,
            Margin = new Thickness(0, 0, 0, 12),
        };
        synBody.Bind(TextBlock.TextProperty, new Binding(nameof(MainWindowViewModel.CurrentLessonSyntax)));

        var stack = new StackPanel
        {
            Spacing = 0,
            Children =
            {
                taskHeading,
                taskBody,
                loadExample,
                goalSectionHeading,
                goalSectionBody,
                worldLabel,
                worldName,
                commandsSectionHeading,
                commandsSectionBody,
                profileLabel,
                profileName,
                referenceHeading,
                kwHeading,
                kwBody,
                synHeading,
                synBody,
            },
        };

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
            Padding = new Thickness(16),
            BoxShadow = StudioVisual.SubtleCardShadow,
            Child = scroll,
        };
    }

    private Control BuildEditorPane()
    {
        var grid = new Grid
        {
            RowDefinitions = new RowDefinitions("*,6,Auto"),
            MinHeight = 360,
        };

        _sourceEditor = new RoboSharpSourceEditor
        {
            MinHeight = 200,
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

        grid.Children.Add(_sourceEditor);
        Grid.SetRow(_sourceEditor, 0);

        var editorRowSplit = new GridSplitter
        {
            Height = 6,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = Brushes.Transparent,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(0, 1, 0, 1),
            ResizeDirection = GridResizeDirection.Rows,
        };
        grid.Children.Add(editorRowSplit);
        Grid.SetRow(editorRowSplit, 1);

        var dockTitle = new TextBlock
        {
            Text = _locale.Sidebar.WorldDockTitle,
            FontSize = 13,
            FontWeight = FontWeight.SemiBold,
            Foreground = StudioVisual.AccentBrush,
            Margin = new Thickness(0, 0, 0, 8),
        };

        _worldGridPreview = new RobotWorldGridView();

        var runStatus = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            LineHeight = 18,
            FontSize = 12,
            Foreground = StudioVisual.TextPrimaryBrush,
            Margin = new Thickness(0, 10, 0, 8),
        };
        runStatus.Bind(TextBlock.TextProperty, new Binding(nameof(MainWindowViewModel.LiveRunStatus))
        {
            Mode = BindingMode.OneWay,
        });

        var dockHint = new TextBlock
        {
            Text = _locale.Sidebar.WorldDockSubtitle,
            TextWrapping = TextWrapping.Wrap,
            Foreground = StudioVisual.TextMutedBrush,
            LineHeight = 18,
            FontSize = 11,
        };

        var dockStack = new StackPanel
        {
            Spacing = 0,
            Children = { dockTitle, _worldGridPreview, runStatus, dockHint },
        };

        var worldDock = new Border
        {
            Background = StudioVisual.SurfaceElevatedBrush,
            BorderBrush = StudioVisual.BorderSubtleBrush,
            BorderThickness = new Thickness(0, 1, 0, 0),
            Padding = new Thickness(12, 10, 12, 12),
            MinHeight = 200,
            Child = dockStack,
        };
        grid.Children.Add(worldDock);
        Grid.SetRow(worldDock, 2);

        if (_lastPipelineSnapshot?.WorldVisualization is { } w0)
            ApplyWorldGridPreviewSnapshot(w0);

        return grid;
    }

    private Border BuildInspectorColumn()
    {
        _inspectorPanelStack = new StackPanel
        {
            Spacing = 0,
        };

        var scroll = new ScrollViewer
        {
            Content = _inspectorPanelStack,
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };

        PopulateInspectorStack();

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

    private void PopulateInspectorStack()
    {
        if (_inspectorPanelStack is null)
            return;

        _inspectorPanelStack.Children.Clear();
        foreach (var panel in VisiblePanelsOrdered())
        {
            var section = new StackPanel
            {
                Spacing = 0,
                Margin = new Thickness(0, 0, 0, 18),
            };

            var title = new TextBlock
            {
                Text = panel.DisplayName,
                FontSize = 14,
                FontWeight = FontWeight.SemiBold,
                Foreground = StudioVisual.TierBrush(panel.AbstractionTier),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 4),
            };
            section.Children.Add(title);

            if (panel.InspectorSubtitle is { Length: > 0 } sub)
            {
                section.Children.Add(new TextBlock
                {
                    Text = sub,
                    FontSize = 11,
                    Foreground = StudioVisual.TextMutedBrush,
                    TextWrapping = TextWrapping.Wrap,
                    LineHeight = 16,
                    Margin = new Thickness(0, 0, 0, 8),
                });
            }

            section.Children.Add(panel.CreateView());
            _inspectorPanelStack.Children.Add(section);
        }

        if (_lastPipelineSnapshot is { } snap)
        {
            foreach (var panel in VisiblePanelsOrdered())
                panel.OnSnapshotChanged(snap);
        }
    }

    private IEnumerable<IStudioPanel> VisiblePanelsOrdered()
    {
        var lesson = _locale.Lessons.Get(_viewModel.SelectedLessonId);
        var allow = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var id in lesson.VisiblePanelIds)
            allow.Add(id);
        return _panels.Where(p => allow.Contains(p.PanelId)).OrderBy(p => p.Order);
    }

    private static Control BuildInspectorFlyoutBody(IStudioPanel panel)
    {
        return new ScrollViewer
        {
            Content = panel.CreateView(),
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
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

        RefreshFlyoutsIfOpen();
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

    private void OpenDiagnosticsFlyout()
    {
        if (_diagnosticsFlyout is { IsVisible: true } existing)
        {
            existing.Activate();
            return;
        }

        var window = CreatePipelineFlyoutWindow(_locale.Shell.MenuViewCompilerDiagnostics, _diagnosticsPanel);
        window.Closed += (_, _) =>
        {
            if (ReferenceEquals(window, _diagnosticsFlyout))
                _diagnosticsFlyout = null;
        };
        _diagnosticsFlyout = window;
        window.Show(this);
    }

    private void OpenRunReportFlyout()
    {
        if (_runReportFlyout is { IsVisible: true } existing)
        {
            existing.Activate();
            return;
        }

        var window = CreatePipelineFlyoutWindow(_locale.Shell.MenuViewRunReport, _worldRuntimePanel);
        window.Closed += (_, _) =>
        {
            if (ReferenceEquals(window, _runReportFlyout))
                _runReportFlyout = null;
        };
        _runReportFlyout = window;
        window.Show(this);
    }

    private Window CreatePipelineFlyoutWindow(string title, IStudioPanel panel)
    {
        var window = new Window
        {
            Title = title,
            Width = 640,
            Height = 520,
            MinWidth = 420,
            MinHeight = 320,
            Background = StudioVisual.BackgroundDeepBrush,
            Foreground = StudioVisual.TextPrimaryBrush,
            FontFamily = StudioVisual.UiFontFamily,
        };

        window.Content = BuildInspectorFlyoutBody(panel);
        if (_lastPipelineSnapshot is { } snap)
            panel.OnSnapshotChanged(snap);

        return window;
    }

    private void RefreshFlyoutsIfOpen()
    {
        if (_lastPipelineSnapshot is not { } snap)
            return;

        if (_diagnosticsFlyout is { IsVisible: true })
            _diagnosticsPanel.OnSnapshotChanged(snap);

        if (_runReportFlyout is { IsVisible: true })
            _worldRuntimePanel.OnSnapshotChanged(snap);
    }

}
