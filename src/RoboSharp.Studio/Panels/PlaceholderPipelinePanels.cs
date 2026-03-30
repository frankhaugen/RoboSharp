using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

/// <summary>Explicit placeholders keep extension points visible in the tab strip (didactic).</summary>
public sealed class BoundTreePlaceholderPanel : IStudioPanel
{
    public int Order => 40;

    public string DisplayName => "Bound tree";

    public Control CreateView() =>
        Wrap(CreateBody(
            "RoboSharp.Semantics is not wired yet.",
            "This tab will show the bound tree and symbols once binding lands."));

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
    }

    private static TextBlock CreateBody(string title, string detail) =>
        new()
        {
            Text = title + Environment.NewLine + Environment.NewLine + detail,
            FontSize = 13,
            LineHeight = 20,
            Foreground = StudioVisual.TextMutedBrush,
            TextWrapping = TextWrapping.Wrap,
        };

    private static Border Wrap(Control c) =>
        new()
        {
            Padding = new Thickness(16),
            Child = c,
        };
}

public sealed class IlPlaceholderPanel : IStudioPanel
{
    public int Order => 50;

    public string DisplayName => "IL";

    public Control CreateView() =>
        new Border
        {
            Padding = new Thickness(16),
            Child = new TextBlock
            {
                Text = "RoboSharp.IL lowering will appear here after semantics.\n\nStudents will see fake opcodes and operands, not CLR IL.",
                FontSize = 13,
                LineHeight = 20,
                Foreground = StudioVisual.TextMutedBrush,
                TextWrapping = TextWrapping.Wrap,
            },
        };

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
    }
}

public sealed class WorldPlaceholderPanel : IStudioPanel
{
    public int Order => 60;

    public string DisplayName => "World";

    public Control CreateView() =>
        new Border
        {
            Padding = new Thickness(16),
            Child = new TextBlock
            {
                Text = "RobotWorld snapshots and ASCII projection will attach to the runtime host.\n\nSee docs/world and docs/rendering.",
                FontSize = 13,
                LineHeight = 20,
                Foreground = StudioVisual.TextMutedBrush,
                TextWrapping = TextWrapping.Wrap,
            },
        };

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
    }
}
