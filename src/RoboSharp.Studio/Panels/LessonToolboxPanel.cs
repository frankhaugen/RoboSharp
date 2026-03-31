using Avalonia;
using Avalonia.Controls;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

/// <summary>Shows which built-ins the active lesson profile allows — like a tiny cheat sheet for kids.</summary>
public sealed class LessonToolboxPanel : IStudioPanel
{
    private TextBox? _text;

    public int Order => 7;

    public string DisplayName => "Lesson toolbox";

    public string? InspectorSubtitle =>
        "Commands allowed in the profile you picked in the left sidebar. Smaller profiles keep puzzles focused.";

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

        const string preamble =
            "# Lesson toolbox\r\n" +
            "Matches the Lesson profile dropdown. If compile says a name is unknown, pick a profile that includes it or switch to Full toolbox.\r\n\r\n";

        if (snapshot.LessonProfileHelpText is { Length: > 0 } help)
        {
            _text.Text = preamble + help;
            return;
        }

        _text.Text = preamble + "(Build once to load profile + world labels here.)";
    }
}
