namespace RoboSharp.IO.Tests;

public class PhysicalRoboFileSystemTests
{
    [Test]
    public async Task WriteThenReadRoundTripsText()
    {
        var temp = Directory.CreateTempSubdirectory("robosharp_io_");
        try
        {
            var fs = new PhysicalRoboFileSystem(new DirectoryInfo(temp.FullName));
            var paths = new RoboPathService();
            var fileUri = paths.Combine(fs.Root.Uri, "hello.robo");
            var file = fs.GetFile(fileUri);

            await file.WriteAllTextAsync("abc");
            var text = await file.ReadAllTextAsync();

            await Assert.That(text).IsEqualTo("abc");
            await Assert.That(file.Exists()).IsTrue();
        }
        finally
        {
            temp.Delete(recursive: true);
        }
    }

    [Test]
    public async Task DoesNotEscapeRoot_ThrowsWhenUriOutsideRoot()
    {
        var temp = Directory.CreateTempSubdirectory("robosharp_io_");
        try
        {
            var fs = new PhysicalRoboFileSystem(new DirectoryInfo(temp.FullName));
            var foreign = new UriBuilder(RoboUriSchemes.Memory, string.Empty, -1, "/elsewhere/file.robo").Uri;

            await Assert.That(() => fs.GetFile(foreign)).Throws<ArgumentOutOfRangeException>();
        }
        finally
        {
            temp.Delete(recursive: true);
        }
    }
}
