namespace RoboSharp.IO;

public interface IRoboFile : IRoboNode
{
    ValueTask<string> ReadAllTextAsync(CancellationToken cancellationToken = default);
    ValueTask WriteAllTextAsync(string content, CancellationToken cancellationToken = default);

    ValueTask<Stream> OpenReadAsync(CancellationToken cancellationToken = default);
    ValueTask<Stream> OpenWriteAsync(bool overwrite = true, CancellationToken cancellationToken = default);

    ValueTask DeleteAsync(CancellationToken cancellationToken = default);
    ValueTask<DateTimeOffset?> GetLastWriteTimeUtcAsync(CancellationToken cancellationToken = default);
}
