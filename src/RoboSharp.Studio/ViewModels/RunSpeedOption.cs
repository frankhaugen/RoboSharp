using RoboSharp.Studio.Pipeline;

namespace RoboSharp.Studio.ViewModels;

/// <summary>ComboBox item: short label in the toolbar, full sentence as tooltip / teaching copy.</summary>
public sealed record RunSpeedOption(StudioRunSpeed Speed, string ShortCaption, string FullCaption)
{
    public override string ToString() => ShortCaption;
}
