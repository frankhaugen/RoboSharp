using System.Text.Json;

namespace RoboSharp.Studio.Settings;

/// <summary>Loads and saves <see cref="StudioUserSettings"/> as JSON (BCL only).</summary>
public static class StudioUserSettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static string FilePath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RoboSharp",
            "Studio",
            "user-settings.json");

    public static StudioUserSettings Load()
    {
        try
        {
            var path = FilePath;
            if (!File.Exists(path))
                return new StudioUserSettings();
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<StudioUserSettings>(json) ?? new StudioUserSettings();
        }
        catch
        {
            return new StudioUserSettings();
        }
    }

    public static void Save(StudioUserSettings settings)
    {
        var path = FilePath;
        var dir = Path.GetDirectoryName(path);
        if (dir is not null)
            Directory.CreateDirectory(dir);
        File.WriteAllText(path, JsonSerializer.Serialize(settings, JsonOptions));
    }
}
