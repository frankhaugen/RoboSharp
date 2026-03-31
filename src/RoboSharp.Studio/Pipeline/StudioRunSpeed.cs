namespace RoboSharp.Studio.Pipeline;

/// <summary>Delay between interpreter instruction steps when running from Studio (Karel-style visibility).</summary>
public enum StudioRunSpeed
{
    Realtime,
    Slow,
    Glacial,
}

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
