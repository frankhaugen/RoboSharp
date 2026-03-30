namespace RoboSharp.Workspaces;

public sealed class ProjectLoadException : Exception
{
    public ProjectLoadException(string message)
        : base(message)
    {
    }

    public ProjectLoadException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
