namespace RoboSharp.Studio.Pipeline;

public static class StudioRunSpeedExtensions
{
    public static int StepDelayMilliseconds(this StudioRunSpeed speed) =>
        speed switch
        {
            StudioRunSpeed.Realtime => 0,
            StudioRunSpeed.Slow => 110,
            StudioRunSpeed.Glacial => 220,
            _ => 0,
        };
}