using System.Text.Json.Serialization;

namespace RoboSharp.Workspaces;

internal sealed class RoboSharpProjectDocument
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("startupFile")]
    public string? StartupFile { get; set; }

    [JsonPropertyName("sourceFiles")]
    public List<string>? SourceFiles { get; set; }
}
