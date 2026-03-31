using Avalonia;
using Avalonia.Controls;

using RoboSharp.Locales;
using RoboSharp.Application.Teaching;
using RoboSharp.Studio.Shell;
using RoboSharp.Toolchain;

namespace RoboSharp.Studio.Panels;

public sealed class BoundTreePipelinePanel : IStudioPanel
{
    private readonly ITeachingLocale _locale;
    private TextBox? _text;

    public BoundTreePipelinePanel(ITeachingLocale locale) =>
        _locale = locale;

    public string PanelId => StudioPanelIds.BoundTree;

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