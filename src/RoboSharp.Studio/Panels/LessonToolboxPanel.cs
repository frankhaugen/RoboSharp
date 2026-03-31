using Avalonia;
using Avalonia.Controls;
using RoboSharp.Locales;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

/// <summary>Shows which built-ins the active lesson profile allows — like a tiny cheat sheet for kids.</summary>
public sealed class LessonToolboxPanel : IStudioPanel
{
    private readonly ITeachingLocale _locale;
    private TextBox? _text;

    public LessonToolboxPanel(ITeachingLocale locale) =>
        _locale = locale;

    public int Order => 7;

    public string DisplayName => _locale.Panels.LessonToolboxTitle;

    public string? InspectorSubtitle => _locale.Panels.LessonToolboxSubtitle;

    public Control CreateView()
    {
        _text = StudioCopyableText.CreateReadOnlyOutput();

        return new Border
        {
            Padding = new Thickness(8),
            Child = _text,
        };
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        if (_text is null)
            return;

        if (snapshot.LessonProfileHelpText is { Length: > 0 } help)
        {
            _text.Text = _locale.Panels.LessonToolboxPreamble + help;
            return;
        }

        _text.Text = _locale.Panels.LessonToolboxPreamble + _locale.Panels.LessonToolboxBuildPrompt;
    }
}
