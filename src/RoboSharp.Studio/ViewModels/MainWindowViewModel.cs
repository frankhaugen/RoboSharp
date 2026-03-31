using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using Avalonia.Threading;
using RoboSharp.Semantics;
using RoboSharp.Studio.Pipeline;
using RoboSharp.World;

namespace RoboSharp.Studio.ViewModels;

/// <summary>
/// Shell state: sample source buffer, separate Build vs Run (run compiles then steps the interpreter).
/// </summary>
public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    /// <summary>Starter buffer for first launch (untitled, not dirty until edited).</summary>
    public const string DefaultStarterSource = """
        // RoboSharp Studio — top-level call + procedure (return type optional on procedures).
        MoveMany(5);

        MoveMany(integer stepsCount)
        {
            print("Moving multiple steps!");

            integer index = 0;
            while (index < stepsCount)
            {
                move();
                index = index + 1;
            }
        }

        """;

    private readonly IPipelineInspectionService _pipeline;
    private CancellationTokenSource? _runCancellation;

    private string _sourceDocument = DefaultStarterSource;
    private string? _documentPath;
    private bool _isDirty;
    private bool _loadingContent;

    private PipelineSnapshot? _currentSnapshot;
    private StudioRunSpeed _selectedRunSpeed = StudioRunSpeed.Slow;
    private string _selectedProfileId = LessonBuiltinProfiles.MovementAndPrintId;
    private string _selectedWorldPresetId = RobotWorldPresets.GoalCornerId;
    private string _liveRunStatus = "Choose lesson profile + map below, then Build or Run.";

    public MainWindowViewModel(IPipelineInspectionService pipeline)
    {
        _pipeline = pipeline;
        BuildCommand = new DelegateCommand(Build);
        RunCommand = new AsyncDelegateCommand(RunAsync);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Fired after Build or Run with the new snapshot (all inspection panels).</summary>
    public event Action<PipelineSnapshot>? PipelineUpdated;

    /// <summary>Fired during Run between IL steps — world snapshot + instruction hint for the status line.</summary>
    public event Action<StudioRunProgress>? RunProgressUpdated;

    /// <summary>Full path when the buffer is linked to disk; <see langword="null"/> for a new unsaved document.</summary>
    public string? DocumentPath
    {
        get => _documentPath;
        private set
        {
            if (_documentPath == value)
                return;
            _documentPath = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DocumentPath)));
            NotifyWindowTitleChanged();
        }
    }

    /// <summary>True when the buffer differs from the last saved/opened state.</summary>
    public bool IsDirty
    {
        get => _isDirty;
        private set
        {
            if (_isDirty == value)
                return;
            _isDirty = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDirty)));
            NotifyWindowTitleChanged();
        }
    }

    /// <summary>Title bar text: file name, dirty star, app name.</summary>
    public string WindowTitle
    {
        get
        {
            var name = DocumentPath is null ? "Untitled.robo" : Path.GetFileName(DocumentPath);
            var star = IsDirty ? " *" : "";
            return $"{name}{star} — RoboSharp Studio";
        }
    }

    public string SourceDocument
    {
        get => _sourceDocument;
        set
        {
            if (_sourceDocument == value)
                return;
            _sourceDocument = value;
            if (!_loadingContent)
                IsDirty = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SourceDocument)));
        }
    }

    /// <summary>Replace buffer from disk (or New); does not mark dirty.</summary>
    public void LoadDocument(string? path, string text)
    {
        _loadingContent = true;
        try
        {
            DocumentPath = path;
            _sourceDocument = text;
            IsDirty = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SourceDocument)));
        }
        finally
        {
            _loadingContent = false;
        }
    }

    /// <summary>Empty untitled document.</summary>
    public void NewUntitledDocument() =>
        LoadDocument(path: null, text: string.Empty);

    /// <summary>After a successful save to the current path.</summary>
    public void MarkSaved(string savedPath)
    {
        _loadingContent = true;
        try
        {
            DocumentPath = savedPath;
            IsDirty = false;
        }
        finally
        {
            _loadingContent = false;
        }
    }

    /// <summary>Save to existing path without changing path text (Save after edits).</summary>
    public void MarkSavedInPlace()
    {
        _loadingContent = true;
        try
        {
            IsDirty = false;
        }
        finally
        {
            _loadingContent = false;
        }
    }

    private void NotifyWindowTitleChanged() =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTitle)));

    public StudioRunSpeed SelectedRunSpeed
    {
        get => _selectedRunSpeed;
        set
        {
            if (_selectedRunSpeed == value)
                return;
            _selectedRunSpeed = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRunSpeed)));
        }
    }

    public PipelineSnapshot? CurrentSnapshot
    {
        get => _currentSnapshot;
        private set
        {
            _currentSnapshot = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentSnapshot)));
        }
    }

    /// <summary>Lesson builtin profile id (<see cref="LessonBuiltinProfiles.OrderedProfileIds"/>).</summary>
    public string SelectedProfileId
    {
        get => _selectedProfileId;
        set
        {
            if (_selectedProfileId == value)
                return;
            _selectedProfileId = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedProfileId)));
            Build();
        }
    }

    /// <summary>World preset id (<see cref="RobotWorldPresets.OrderedPresets"/>).</summary>
    public string SelectedWorldPresetId
    {
        get => _selectedWorldPresetId;
        set
        {
            if (_selectedWorldPresetId == value)
                return;
            _selectedWorldPresetId = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedWorldPresetId)));
            Build();
        }
    }

    /// <summary>Shown under the world during Run and after for kids-friendly feedback.</summary>
    public string LiveRunStatus
    {
        get => _liveRunStatus;
        private set
        {
            if (_liveRunStatus == value)
                return;
            _liveRunStatus = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LiveRunStatus)));
        }
    }

    public ICommand BuildCommand { get; }
    public ICommand RunCommand { get; }

    public StudioPipelineOptions CreatePipelineOptions() =>
        new(
            LessonBuiltinProfiles.GetProvider(SelectedProfileId),
            () => RobotWorldPresets.Create(SelectedWorldPresetId),
            LessonBuiltinProfiles.GetDisplayName(SelectedProfileId),
            RobotWorldPresets.GetDisplayName(SelectedWorldPresetId));

    public void Build()
    {
        var snap = _pipeline.InspectBuildOnly(SourceDocument, CreatePipelineOptions());
        CurrentSnapshot = snap;
        PipelineUpdated?.Invoke(snap);
    }

    private async Task RunAsync()
    {
        _runCancellation?.Cancel();
        _runCancellation?.Dispose();
        _runCancellation = new CancellationTokenSource();
        var token = _runCancellation.Token;

        try
        {
            LiveRunStatus = "Running…";
            var options = CreatePipelineOptions();
            var progress = new Progress<StudioRunProgress>(p =>
                Dispatcher.UIThread.Post(() =>
                {
                    LiveRunStatus =
                        $"{p.InstructionsExecutedSoFar} IL steps · {p.InstructionDescription ?? "…"}";
                    RunProgressUpdated?.Invoke(p);
                }));

            var snap = await _pipeline
                .InspectBuildAndRunAsync(SourceDocument, SelectedRunSpeed, options, progress, token)
                .ConfigureAwait(true);

            CurrentSnapshot = snap;
            PipelineUpdated?.Invoke(snap);

            if (snap.LessonOutcomeSummary is { } story)
                LiveRunStatus = snap.LessonScore is { } sc ? $"{story}  →  Score: {sc}" : story;
            else if (snap.RuntimeSucceeded == true)
                LiveRunStatus = "Finished run.";
            else
                LiveRunStatus = snap.RuntimeFaultMessage ?? "Run stopped.";
        }
        catch (OperationCanceledException)
        {
            // New Run cancelled the previous token; ignore.
        }
    }

    private sealed class DelegateCommand : ICommand
    {
        private readonly Action _execute;

        public DelegateCommand(Action execute) => _execute = execute;

#pragma warning disable CS0067
        public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute();
    }

    private sealed class AsyncDelegateCommand : ICommand
    {
        private readonly Func<Task> _execute;

        public AsyncDelegateCommand(Func<Task> execute) => _execute = execute;

#pragma warning disable CS0067
        public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _ = _execute();
    }
}
