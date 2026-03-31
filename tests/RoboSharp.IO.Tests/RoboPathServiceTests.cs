namespace RoboSharp.IO.Tests;

public class RoboPathServiceTests
{
    private static readonly Uri Root = new UriBuilder(RoboUriSchemes.Memory, string.Empty, -1, "/root/").Uri;

    private readonly IRoboPathService _paths = new RoboPathService();

    [Test]
    public async Task Combine_AppendsRelativeSegments()
    {
        var child = _paths.Combine(Root, "a/b");
        await Assert.That(_paths.GetRelativePath(Root, child)).IsEqualTo("a/b");
    }

    [Test]
    public async Task Combine_NormalizesDotSegments()
    {
        var child = _paths.Combine(Root, "a/./b");
        await Assert.That(_paths.GetRelativePath(Root, child)).IsEqualTo("a/b");
    }

    [Test]
    public async Task Combine_RejectsParentEscape()
    {
        await Assert.That(() => _paths.Combine(Root, "..")).Throws<ArgumentException>();
    }

    [Test]
    public async Task IsUnderRoot_ReturnsTrueForDescendants()
    {
        var child = _paths.Combine(Root, "x/y");
        await Assert.That(_paths.IsUnderRoot(Root, child)).IsTrue();
    }

    [Test]
    public async Task IsUnderRoot_ReturnsFalseForForeignScheme()
    {
        var other = new UriBuilder(RoboUriSchemes.Memory, string.Empty, -1, "/other/").Uri;
        await Assert.That(_paths.IsUnderRoot(Root, other)).IsFalse();
    }
}
