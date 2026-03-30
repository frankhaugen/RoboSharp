using Avalonia.Controls;
using RoboSharp.Studio.Pipeline;

namespace RoboSharp.Studio.Panels;

/// <summary>
/// One inspectable stage or placeholder in the right-rail tabs. Order = tab order (pipeline narrative).
/// </summary>
public interface IStudioPanel
{
    /// <summary>Sort key for tab strip (lower = left).</summary>
    int Order { get; }

    string DisplayName { get; }

    /// <summary>Build the view once; subsequent updates go through <see cref="OnSnapshotChanged"/>.</summary>
    Control CreateView();

    void OnSnapshotChanged(PipelineSnapshot snapshot);
}
