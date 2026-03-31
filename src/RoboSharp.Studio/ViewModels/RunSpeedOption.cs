using RoboSharp.Studio.Pipeline;

namespace RoboSharp.Studio.ViewModels;

/// <summary>ComboBox item: human-readable run speed caption + underlying enum for the interpreter loop.</summary>
public sealed record RunSpeedOption(StudioRunSpeed Speed, string Caption)
{
    public override string ToString() => Caption;
}
