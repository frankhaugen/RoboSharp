using Avalonia;
using Avalonia.Controls;
using RoboSharp.Locales;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;
using RoboSharp.Toolchain;

namespace RoboSharp.Studio.Panels;

public sealed class BoundTreePipelinePanel : IStudioPanel
{
    private readonly ITeachingLocale _locale;
    private TextBox? _text;

    public BoundTreePipelinePanel(ITeachingLocale locale) =>
        _locale = locale;

    public int Order => 40;

    public string DisplayName => _locale.Panels.BoundTreeTitle;

    public string? InspectorSubtitle => _locale.Panels.BoundTreeSubtitle;

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

        if (snapshot.BoundTreeText is { Length: > 0 } body)
        {
            _text.Text = _locale.Panels.BoundTreePreamble + body;
            return;
        }

        var note = snapshot.CompileReachedPhase switch
        {
            CompilePhase.Parse => _locale.Panels.BoundTreeNeedParseFirst,
            CompilePhase.Semantics => _locale.Panels.BoundTreeSemanticsStopped,
            CompilePhase.Lowered => _locale.Panels.BoundTreeUnexpectedEmpty,
            _ => _locale.Panels.BoundTreeBuildPrompt,
        };

        _text.Text = _locale.Panels.BoundTreePreamble + note;
    }
}

public sealed class IlPipelinePanel : IStudioPanel
{
    private readonly ITeachingLocale _locale;
    private TextBox? _text;

    public IlPipelinePanel(ITeachingLocale locale) =>
        _locale = locale;

    public int Order => 50;

    public string DisplayName => _locale.Panels.IlTitle;

    public string? InspectorSubtitle => _locale.Panels.IlSubtitle;

    public Control CreateView()
    {
        _text = StudioCopyableText.CreateReadOnlyOutput(fontSize: 11);

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

        if (snapshot.IlDisassemblyText is { Length: > 0 } il)
        {
            var body = il;
            if (snapshot.IlExecutionFootnote is { Length: > 0 } foot)
                body += "\r\n\r\n" + foot;
            _text.Text = _locale.Panels.IlPreamble + body;
            return;
        }

        _text.Text = _locale.Panels.IlPreamble + (snapshot.CompileReachedPhase < CompilePhase.Lowered
            ? _locale.Panels.IlWaitingForLowering
            : _locale.Panels.IlNoTextUnexpected);
    }
}

public sealed class WorldRuntimePipelinePanel : IStudioPanel
{
    private readonly ITeachingLocale _locale;
    private TextBox? _text;

    public WorldRuntimePipelinePanel(ITeachingLocale locale) =>
        _locale = locale;

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
