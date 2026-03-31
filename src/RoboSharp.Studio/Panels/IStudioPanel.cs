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

    /// <summary>Live interpreter tick while Run is stepping (world + IL cursor). Default: no-op.</summary>
    void OnRunProgress(StudioRunProgress progress) { }

    /// <summary>Re-apply strings from the current <see cref="RoboSharp.Locales.ITeachingLocale"/> after the user changes language.</summary>
    void ApplyLocale(PipelineSnapshot? lastSnapshot)
    {
        if (lastSnapshot is not null)
            OnSnapshotChanged(lastSnapshot);
    }
}
