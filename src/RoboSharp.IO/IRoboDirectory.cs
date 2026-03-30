namespace RoboSharp.IO;

public interface IRoboDirectory : IRoboNode
{
    IEnumerable<IRoboDirectory> EnumerateDirectories();
    IEnumerable<IRoboFile> EnumerateFiles();

    IRoboDirectory GetDirectory(string relativeName);
    IRoboFile GetFile(string relativeName);

    ValueTask EnsureExistsAsync(CancellationToken cancellationToken = default);
    ValueTask DeleteAsync(bool recursive, CancellationToken cancellationToken = default);
}
