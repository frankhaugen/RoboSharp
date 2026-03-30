using System.ComponentModel;
using System.Windows.Input;
using RoboSharp.Studio.Pipeline;

namespace RoboSharp.Studio.ViewModels;

/// <summary>
/// Shell state: sample source buffer + pipeline refresh. Panels listen via <see cref="PipelineUpdated"/>.
/// </summary>
public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IPipelineInspectionService _pipeline;
    private string _sourceDocument = """
        // RoboSharp Studio — Language pipeline (lexer + parser)
        integer x = 1 + 2 * 3;

        """;

    private PipelineSnapshot? _currentSnapshot;

    public MainWindowViewModel(IPipelineInspectionService pipeline)
    {
        _pipeline = pipeline;
        RefreshCommand = new DelegateCommand(RunPipeline);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Fired after each successful <see cref="RunPipeline"/> with the new snapshot.</summary>
    public event Action<PipelineSnapshot>? PipelineUpdated;

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

    public PipelineSnapshot? CurrentSnapshot
    {
        get => _currentSnapshot;
        private set
        {
            _currentSnapshot = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentSnapshot)));
        }
    }

    public ICommand RefreshCommand { get; }

    public void RunPipeline()
    {
        var snap = _pipeline.Inspect(SourceDocument);
        CurrentSnapshot = snap;
        PipelineUpdated?.Invoke(snap);
    }

    private sealed class DelegateCommand : ICommand
    {
        private readonly Action _execute;

        public DelegateCommand(Action execute) => _execute = execute;

#pragma warning disable CS0067 // Used by WPF-style command consumers; Avalonia may not raise.
        public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute();
    }
}
