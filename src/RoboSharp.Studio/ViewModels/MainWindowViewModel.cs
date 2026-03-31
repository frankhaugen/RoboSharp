using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using Avalonia.Threading;
using RoboSharp.Locales;
using RoboSharp.Semantics;
using RoboSharp.Application.Teaching;
using RoboSharp.World;

namespace RoboSharp.Studio.ViewModels;

/// <summary>
/// Shell state: sample source buffer, separate Build vs Run (run compiles then steps the interpreter).
/// </summary>
public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IPipelineInspectionService _pipeline;
    private readonly ITeachingLocale _locale;
    private CancellationTokenSource? _runCancellation;

    private string _sourceDocument;
    private string? _documentPath;
    private bool _isDirty;
    private bool _loadingContent;

    private PipelineSnapshot? _currentSnapshot;
    private StudioRunSpeed _selectedRunSpeed = StudioRunSpeed.Slow;
    private string _selectedLessonId = StudioLessonIds.FirstMoves;
    private string _selectedProfileId;
    private string _selectedWorldPresetId;
    private string _liveRunStatus;
    private IReadOnlyList<RunSpeedOption> _runSpeedOptions;

    public MainWindowViewModel(IPipelineInspectionService pipeline, ITeachingLocale locale)
    {
        _pipeline = pipeline;
        _locale = locale;
        var initial = locale.Lessons.Get(_selectedLessonId);
        _selectedProfileId = initial.DefaultProfileId;
        _selectedWorldPresetId = initial.DefaultWorldPresetId;
        _sourceDocument = initial.ExampleSource;
        _liveRunStatus = locale.Shell.DefaultLiveRunStatus;
        _runSpeedOptions = BuildRunSpeedOptions(locale);
        BuildCommand = new DelegateCommand(Build);
        RunCommand = new AsyncDelegateCommand(RunAsync);
        LoadLessonExampleCommand = new DelegateCommand(LoadLessonExampleIntoEditor);
    }

    public IReadOnlyList<RunSpeedOption> RunSpeedOptions => _runSpeedOptions;

    /// <summary>Rebuilds run-speed labels and status line after the shell language changes.</summary>
    public void ApplyLocaleRefresh()
    {
        _runSpeedOptions = BuildRunSpeedOptions(_locale);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RunSpeedOptions)));
        _liveRunStatus = _locale.Shell.DefaultLiveRunStatus;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LiveRunStatus)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTitle)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRunSpeedOption)));
        NotifyLessonPresentationChanged();
    }

    static IReadOnlyList<RunSpeedOption> BuildRunSpeedOptions(ITeachingLocale locale) =>
    [
        new(StudioRunSpeed.Realtime, locale.Shell.RunSpeedRealtimeShort, locale.Shell.RunSpeedRealtime),
        new(StudioRunSpeed.Slow, locale.Shell.RunSpeedSlowShort, locale.Shell.RunSpeedSlow),
        new(StudioRunSpeed.Glacial, locale.Shell.RunSpeedGlacialShort, locale.Shell.RunSpeedGlacial),
    ];

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
            var name = DocumentPath is null ? _locale.Shell.UntitledFileName : Path.GetFileName(DocumentPath);
            return _locale.Shell.FormatWindowTitle(name, IsDirty);
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
            _sourceDocument = string.IsNullOrEmpty(text) ? text : text.ReplaceLineEndings("\n");
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRunSpeedOption)));
        }
    }

    public RunSpeedOption? SelectedRunSpeedOption
    {
        get => RunSpeedOptions.FirstOrDefault(o => o.Speed == _selectedRunSpeed);
        set
        {
            if (value is null || value.Speed == _selectedRunSpeed)
                return;
            _selectedRunSpeed = value.Speed;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRunSpeed)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRunSpeedOption)));
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

    /// <summary>Lesson builtin profile id — follows <see cref="SelectedLessonId"/>.</summary>
    public string SelectedProfileId => _selectedProfileId;

    /// <summary>World preset id — follows <see cref="SelectedLessonId"/>.</summary>
    public string SelectedWorldPresetId => _selectedWorldPresetId;

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
    public ICommand LoadLessonExampleCommand { get; }

    /// <summary>Teaching track id (<see cref="IStudioLessonCatalog.OrderedLessons"/>).</summary>
    public string SelectedLessonId
    {
        get => _selectedLessonId;
        set
        {
            if (_selectedLessonId == value)
                return;
            _selectedLessonId = value;
            var def = _locale.Lessons.Get(value);
            _selectedProfileId = def.DefaultProfileId;
            _selectedWorldPresetId = def.DefaultWorldPresetId;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedLessonId)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedProfileId)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedWorldPresetId)));
            NotifyLessonPresentationChanged();
            Build();
        }
    }

    public string CurrentLessonTitle => _locale.Lessons.Get(_selectedLessonId).Title;

    public string CurrentLessonStartBlurb => _locale.Lessons.Get(_selectedLessonId).StartHereBlurb;

    public string CurrentLessonKeywords => _locale.Lessons.Get(_selectedLessonId).KeywordsSection;

    public string CurrentLessonSyntax => _locale.Lessons.Get(_selectedLessonId).SyntaxSection;

    public string CurrentLessonGoalSectionBody => _locale.Lessons.Get(_selectedLessonId).GoalSectionBody;

    public string CurrentLessonCommandsSectionBody => _locale.Lessons.Get(_selectedLessonId).CommandsSectionBody;

    public string CurrentLessonWorldDisplayName => RobotWorldPresets.GetDisplayName(_selectedWorldPresetId);

    public string CurrentLessonProfileDisplayName => LessonBuiltinProfiles.GetDisplayName(_selectedProfileId);

    void NotifyLessonPresentationChanged()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLessonTitle)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLessonStartBlurb)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLessonKeywords)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLessonSyntax)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLessonGoalSectionBody)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLessonCommandsSectionBody)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLessonWorldDisplayName)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLessonProfileDisplayName)));
    }

    void LoadLessonExampleIntoEditor() =>
        LoadDocument(path: null, text: _locale.Lessons.Get(_selectedLessonId).ExampleSource);

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
            LiveRunStatus = _locale.Shell.LiveRunInProgress;
            var options = CreatePipelineOptions();
            var progress = new Progress<StudioRunProgress>(p =>
                Dispatcher.UIThread.Post(() =>
                {
                    LiveRunStatus = _locale.Shell.FormatLiveRunProgress(
                        p.InstructionsExecutedSoFar,
                        p.InstructionDescription);
                    RunProgressUpdated?.Invoke(p);
                }));

            var snap = await _pipeline
                .InspectBuildAndRunAsync(SourceDocument, SelectedRunSpeed, options, progress, token)
                .ConfigureAwait(true);

            CurrentSnapshot = snap;
            PipelineUpdated?.Invoke(snap);

            if (snap.LessonOutcomeSummary is { } story)
                LiveRunStatus = _locale.Shell.FormatLessonOutcomeLine(story, snap.LessonScore);
            else if (snap.RuntimeSucceeded == true)
                LiveRunStatus = _locale.Shell.LiveRunFinished;
            else
                LiveRunStatus = snap.RuntimeFaultMessage ?? _locale.Shell.LiveRunFaultFallback;
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
