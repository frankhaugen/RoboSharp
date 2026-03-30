using System.Xml.Linq;

namespace RoboSharp.Architecture.Tests;

/// <summary>
/// Ensures <c>RoboSharp.slnx</c> lists <c>docs/**</c> as <b>sibling</b> solution folders per directory
/// (<c>/docs/</c>, <c>/docs/diagrams/</c>, …), not one flat list and not nested <c>Folder</c> inside <c>/docs/</c>.
/// </summary>
public class SlnxDocsLayoutTests
{
    [Test]
    public async Task Solution_lists_doc_files_in_sibling_folders_per_directory_under_docs()
    {
        var slnxPath = Path.Combine(FindRepoRoot(), "RoboSharp.slnx");
        await Assert.That(File.Exists(slnxPath)).IsTrue();

        var doc = XDocument.Load(slnxPath);
        var solution = doc.Root ?? throw new InvalidOperationException("Missing Solution root.");

        var docFolders = solution.Elements("Folder")
            .Where(e => ((string?)e.Attribute("Name") ?? "").StartsWith("/docs/", StringComparison.Ordinal))
            .ToList();

        await Assert.That(docFolders.Count).IsGreaterThanOrEqualTo(3);

        var names = docFolders.Select(e => (string?)e.Attribute("Name")).Where(n => n is not null).ToHashSet();
        await Assert.That(names.Contains("/docs/")).IsTrue();
        await Assert.That(names.Contains("/docs/diagrams/")).IsTrue();
        await Assert.That(names.Contains("/docs/diagrams/architecture/")).IsTrue();

        foreach (var folder in docFolders)
            await Assert.That(folder.Elements("Folder").Any()).IsFalse();

        var rootDocs = docFolders.First(e => (string?)e.Attribute("Name") == "/docs/");
        var rootPaths = rootDocs.Elements("File").Select(e => (string?)e.Attribute("Path")).ToHashSet(StringComparer.OrdinalIgnoreCase);
        await Assert.That(rootPaths.Contains("docs/README.md")).IsTrue();

        var diagrams = docFolders.First(e => (string?)e.Attribute("Name") == "/docs/diagrams/");
        var diagramPaths = diagrams.Elements("File").Select(e => (string?)e.Attribute("Path")).ToHashSet(StringComparer.OrdinalIgnoreCase);
        await Assert.That(diagramPaths.Contains("docs/diagrams/project-references.md")).IsTrue();
        await Assert.That(diagramPaths.Contains("docs/diagrams/nuget-references.md")).IsTrue();

        var arch = docFolders.First(e => (string?)e.Attribute("Name") == "/docs/diagrams/architecture/");
        var archPaths = arch.Elements("File").Select(e => (string?)e.Attribute("Path")).ToHashSet(StringComparer.OrdinalIgnoreCase);
        await Assert.That(archPaths.Contains("docs/diagrams/architecture/notes.md")).IsTrue();
    }

    static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "RoboSharp.slnx");
            if (File.Exists(candidate))
                return dir.FullName;
            dir = dir.Parent;
        }

        throw new InvalidOperationException("RoboSharp.slnx not found when walking up from test base directory.");
    }
}
