namespace RoboSharp.Studio.Settings;

/// <summary>User-global Studio preferences persisted under LocalApplicationData.</summary>
public sealed class StudioUserSettings
{
    /// <summary>BCP 47-style id, e.g. <c>en</c> or <c>la</c>.</summary>
    public string? LocaleId { get; set; }
}
