namespace RoboSharp.Studio.Pipeline;

/// <summary>Delay between interpreter instruction steps when running from Studio (step-by-step visibility).</summary>
public enum StudioRunSpeed
{
    Realtime,
    Slow,
    Glacial,
}