namespace RoboSharp.IO;

public interface IRoboNode
{
    Uri Uri { get; }
    string Name { get; }
    IRoboDirectory? Parent { get; }
    bool Exists();
}
