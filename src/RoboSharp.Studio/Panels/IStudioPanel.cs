using Avalonia.Controls;
using RoboSharp.Studio.Pipeline;

namespace RoboSharp.Studio.Panels;

/// <summary>
/// One inspectable pipeline stage in the right-rail panel stack. Order = top-to-bottom narrative.
/// </summary>
public interface IStudioPanel
{
    /// <summary>Sort key for the inspector stack (lower = higher in the list).</summary>
    int Order { get; }

    string DisplayName { get; }

    /// <summary>Short line under the title explaining what this pane shows (not included in the copyable body).</summary>
    string? InspectorSubtitle => null;

    /// <summary>Build the view once; subsequent updates go through <see cref="OnSnapshotChanged"/>.</summary>
    Control CreateView();

    void OnSnapshotChanged(PipelineSnapshot snapshot);
}
