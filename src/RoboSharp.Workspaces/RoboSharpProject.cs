namespace RoboSharp.Workspaces;

/// <summary>
/// In-memory representation of a loaded <c>.robosharp</c> project. Paths use forward slashes relative to the project root directory.
/// </summary>
public sealed record RoboSharpProject(string? Name, IReadOnlyList<string> SourceFilesRelative, string StartupFileRelative);
