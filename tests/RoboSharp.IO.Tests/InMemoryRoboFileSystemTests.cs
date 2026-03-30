namespace RoboSharp.IO.Tests;

public class InMemoryRoboFileSystemTests
{
    private static InMemoryRoboFileSystem CreateFileSystem()
    {
        var root = new UriBuilder(RoboUriSchemes.Memory, string.Empty, -1, "/root/").Uri;
        return new InMemoryRoboFileSystem(root);
    }

    [Test]
    public async Task WriteThenReadRoundTripsText()
    {
        var fs = CreateFileSystem();
        var paths = new RoboPathService();
        var fileUri = paths.Combine(fs.Root.Uri, "hello.robo");
        var file = fs.GetFile(fileUri);

        await file.WriteAllTextAsync("abc");
        var text = await file.ReadAllTextAsync();

        await Assert.That(text).IsEqualTo("abc");
        await Assert.That(file.Exists()).IsTrue();
    }

    [Test]
    public async Task EnumerateFiles_IsDeterministic()
    {
        var fs = CreateFileSystem();
        var paths = new RoboPathService();
        await fs.GetFile(paths.Combine(fs.Root.Uri, "b.robo")).WriteAllTextAsync("b");
        await fs.GetFile(paths.Combine(fs.Root.Uri, "a.robo")).WriteAllTextAsync("a");

        var names = fs.Root.EnumerateFiles().Select(f => f.Name).ToArray();

        await Assert.That(names).IsEquivalentTo(new[] { "a.robo", "b.robo" });
    }

    [Test]
    public async Task OpenWrite_CommitsOnDispose()
    {
        var fs = CreateFileSystem();
        var paths = new RoboPathService();
        var fileUri = paths.Combine(fs.Root.Uri, "out.bin");
        var file = fs.GetFile(fileUri);

        await using (var stream = await file.OpenWriteAsync())
        {
            await stream.WriteAsync("data"u8.ToArray());
        }

        var text = await file.ReadAllTextAsync();
        await Assert.That(text).IsEqualTo("data");
    }

    [Test]
    public async Task Delete_RemovesFile()
    {
        var fs = CreateFileSystem();
        var paths = new RoboPathService();
        var fileUri = paths.Combine(fs.Root.Uri, "x.robo");
        var file = fs.GetFile(fileUri);
        await file.WriteAllTextAsync("x");
        await file.DeleteAsync();

        await Assert.That(file.Exists()).IsFalse();
        await Assert.That(fs.FileExists(fileUri)).IsFalse();
    }

    [Test]
    public async Task DirectoryExists_ForImplicitParentOfFile()
    {
        var fs = CreateFileSystem();
        var paths = new RoboPathService();
        var dirUri = paths.Combine(fs.Root.Uri, "src");
        await fs.GetFile(paths.Combine(dirUri, "a.robo")).WriteAllTextAsync("!");

        await Assert.That(fs.DirectoryExists(dirUri)).IsTrue();
    }
}
