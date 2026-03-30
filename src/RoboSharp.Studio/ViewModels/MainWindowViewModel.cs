using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Threading;
using RoboSharp.Studio.Pipeline;
using RoboSharp.World;

namespace RoboSharp.Studio.ViewModels;

/// <summary>
/// Shell state: sample source buffer, separate Build vs Run (run compiles then steps the interpreter).
/// </summary>
public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IPipelineInspectionService _pipeline;
    private CancellationTokenSource? _runCancellation;

    private string _sourceDocument = """
        // RoboSharp Studio — Karel-style robot on a grid (see left pane after Build / Run)
        void main()
        {
            move();
        }

        """;

    private PipelineSnapshot? _currentSnapshot;
    private StudioRunSpeed _selectedRunSpeed = StudioRunSpeed.Slow;

    public MainWindowViewModel(IPipelineInspectionService pipeline)
    {
        _pipeline = pipeline;
        BuildCommand = new DelegateCommand(Build);
        RunCommand = new AsyncDelegateCommand(RunAsync);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Fired after Build or Run with the new snapshot (all inspection panels).</summary>
    public event Action<PipelineSnapshot>? PipelineUpdated;

    /// <summary>Fired during Run between interpreter steps so the Karel pane can animate.</summary>
    public event Action<RobotWorldSnapshot>? KarelFrameUpdated;

    public string SourceDocument
    {
        get => _sourceDocument;
        set
        {
            if (_sourceDocument == value)
                return;
            _sourceDocument = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SourceDocument)));
        }
    }

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

    public ICommand BuildCommand { get; }
    public ICommand RunCommand { get; }

    public void Build()
    {
        var snap = _pipeline.InspectBuildOnly(SourceDocument);
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
            var progress = new Progress<RobotWorldSnapshot>(snap =>
                Dispatcher.UIThread.Post(() => KarelFrameUpdated?.Invoke(snap)));

            var snap = await _pipeline
                .InspectBuildAndRunAsync(SourceDocument, SelectedRunSpeed, progress, token)
                .ConfigureAwait(true);

            CurrentSnapshot = snap;
            PipelineUpdated?.Invoke(snap);
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
