namespace RoboSharp.IO;

public interface IRoboFileSystem
{
    IRoboDirectory Root { get; }

    IRoboFile GetFile(Uri uri);
    IRoboDirectory GetDirectory(Uri uri);

    bool FileExists(Uri uri);
    bool DirectoryExists(Uri uri);
}
