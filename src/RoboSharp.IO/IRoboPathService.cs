namespace RoboSharp.IO;

public interface IRoboPathService
{
    Uri Combine(Uri baseUri, string relativePath);
    string GetRelativePath(Uri fromDirectory, Uri toNode);
    bool IsUnderRoot(Uri root, Uri candidate);
}
