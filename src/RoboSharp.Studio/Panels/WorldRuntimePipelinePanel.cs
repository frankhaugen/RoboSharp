using Avalonia;
using Avalonia.Controls;

using RoboSharp.Locales;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

public sealed class WorldRuntimePipelinePanel : IStudioPanel
{
    private readonly ITeachingLocale _locale;
    private TextBox? _text;

    public WorldRuntimePipelinePanel(ITeachingLocale locale) =>
        _locale = locale;

    public string PanelId => StudioPanelIds.WorldRuntime;

    public int Order => 60;

    public string DisplayName => _locale.Panels.WorldRuntimeTitle;

    public string? InspectorSubtitle => _locale.Panels.WorldRuntimeSubtitle;

    public Control CreateView()
    {
        _text = StudioCopyableText.CreateReadOnlyOutput();

        return new Border
        {
            Padding = new Thickness(4),
            Child = _text,
        };
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        if (_text is null)
            return;

        _text.Text = _locale.Panels.FormatWorldRuntimePanel(
            snapshot.RuntimeSucceeded,
            snapshot.IlDisassemblyText is { Length: > 0 },
            snapshot.LessonOutcomeSummary,
            snapshot.LessonScore,
            snapshot.WorldAfterRunSummary,
            snapshot.RuntimeFaultMessage,
            snapshot.RuntimeStdout,
            snapshot.RuntimeStderr);
    }
}