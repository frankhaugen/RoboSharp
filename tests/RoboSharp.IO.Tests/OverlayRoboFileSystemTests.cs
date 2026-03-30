namespace RoboSharp.IO.Tests;

public class OverlayRoboFileSystemTests
{
    [Test]
    public async Task ReadsBaseWhenOverlayEmpty()
    {
        var temp = Directory.CreateTempSubdirectory("robosharp_io_");
        try
        {
            var physical = new PhysicalRoboFileSystem(new DirectoryInfo(temp.FullName));
            var paths = new RoboPathService();
            var fileUri = paths.Combine(physical.Root.Uri, "base.robo");
            await physical.GetFile(fileUri).WriteAllTextAsync("from-base");

            var overlayLayer = new InMemoryRoboFileSystem(physical.Root.Uri);
            var composite = new OverlayRoboFileSystem(physical, overlayLayer);

            var text = await composite.GetFile(fileUri).ReadAllTextAsync();
            await Assert.That(text).IsEqualTo("from-base");
        }
        finally
        {
            temp.Delete(recursive: true);
        }
    }

    [Test]
    public async Task OverlayWriteMasksBaseRead()
    {
        var temp = Directory.CreateTempSubdirectory("robosharp_io_");
        try
        {
            var physical = new PhysicalRoboFileSystem(new DirectoryInfo(temp.FullName));
            var paths = new RoboPathService();
            var fileUri = paths.Combine(physical.Root.Uri, "dual.robo");
            await physical.GetFile(fileUri).WriteAllTextAsync("base");

            var overlayLayer = new InMemoryRoboFileSystem(physical.Root.Uri);
            var composite = new OverlayRoboFileSystem(physical, overlayLayer);
            await composite.GetFile(fileUri).WriteAllTextAsync("overlay");

            var text = await composite.GetFile(fileUri).ReadAllTextAsync();
            await Assert.That(text).IsEqualTo("overlay");
        }
        finally
        {
            temp.Delete(recursive: true);
        }
    }

    [Test]
    public async Task Delete_TombstonesBaseFile()
    {
        var temp = Directory.CreateTempSubdirectory("robosharp_io_");
        try
        {
            var physical = new PhysicalRoboFileSystem(new DirectoryInfo(temp.FullName));
            var paths = new RoboPathService();
            var fileUri = paths.Combine(physical.Root.Uri, "gone.robo");
            await physical.GetFile(fileUri).WriteAllTextAsync("still-on-disk");

            var overlayLayer = new InMemoryRoboFileSystem(physical.Root.Uri);
            var composite = new OverlayRoboFileSystem(physical, overlayLayer);
            await composite.GetFile(fileUri).DeleteAsync();

            await Assert.That(composite.FileExists(fileUri)).IsFalse();
            await Assert.That(physical.FileExists(fileUri)).IsTrue();
        }
        finally
        {
            temp.Delete(recursive: true);
        }
    }
}
