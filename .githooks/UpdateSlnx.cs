#:property TargetFramework=net10.0
#:property UseArtifactsOutput=false
#:property TreatWarningsAsErrors=false
#:property EnforceCodeStyleInBuild=false
#:property PublishAot=false
#:property PackAsTool=false

using System.Text;

string root;
try
{
    root = Path.GetFullPath(args.Length > 0 ? args[0] : FindRepoRoot());
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex.Message);
    return 1;
}

var slnxPath = Path.Combine(root, "RoboSharp.slnx");
if (!Directory.Exists(root))
{
    Console.Error.WriteLine($"Repository root is invalid: {root}");
    return 1;
}

var docFiles = CollectDocs(root);
var infraFiles = CollectInfrastructure(root);
var srcProjects = DiscoverProjects(root, "src");
var testProjects = DiscoverProjects(root, "tests");

var sb = new StringBuilder();
sb.AppendLine("<Solution>");

// SLNX / IDE: one top-level <Folder> per on-disk directory under docs/ (siblings under <Solution>), e.g.
// /docs/, /docs/diagrams/, /docs/diagrams/architecture/ — each contains only <File/> entries (no nested Folder).
AppendDocSolutionFolders(sb, docFiles);
AppendFileFolder(sb, "/infrastructure/", infraFiles);

AppendProjectFolder(sb, "/src/", srcProjects);
AppendProjectFolder(sb, "/tests/", testProjects);

sb.AppendLine("</Solution>");

var newContent = sb.ToString().ReplaceLineEndings("\n");
var utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
File.WriteAllText(slnxPath, newContent, utf8NoBom);
return 0;

static void AppendDocSolutionFolders(StringBuilder sb, IReadOnlyList<string> docFilesRelative)
{
    if (docFilesRelative.Count == 0)
        return;

    var groups = docFilesRelative
        .GroupBy(DocDirectoryToSlnxFolderName)
        .OrderBy(g => g.Key, StringComparer.Ordinal);
    foreach (var g in groups)
        AppendFileFolder(sb, g.Key, g.Order(StringComparer.Ordinal).ToList());
}

/// <summary>
/// Maps <c>docs/diagrams/a.md</c> → <c>/docs/diagrams/</c>; <c>docs/README.md</c> → <c>/docs/</c>.
/// </summary>
static string DocDirectoryToSlnxFolderName(string repoRelativeFilePath)
{
    var normalized = repoRelativeFilePath.Replace('\\', '/');
    var parent = Path.GetDirectoryName(normalized);
    if (string.IsNullOrEmpty(parent))
        return "/docs/";
    return "/" + parent.Replace('\\', '/') + "/";
}

static void AppendFileFolder(StringBuilder sb, string folderName, IReadOnlyList<string> paths)
{
    if (paths.Count == 0)
        return;

    sb.AppendLine($"  <Folder Name=\"{folderName}\">");
    foreach (var p in paths)
        sb.AppendLine($"    <File Path=\"{EscapeXmlAttribute(p)}\" />");
    sb.AppendLine("  </Folder>");
}

static void AppendProjectFolder(StringBuilder sb, string folderName, IReadOnlyList<string> paths)
{
    sb.AppendLine($"  <Folder Name=\"{folderName}\">");
    foreach (var p in paths)
        sb.AppendLine($"    <Project Path=\"{EscapeXmlAttribute(p)}\" />");
    sb.AppendLine("  </Folder>");
}

static string EscapeXmlAttribute(string value) =>
    value.Replace("&", "&amp;", StringComparison.Ordinal)
        .Replace("\"", "&quot;", StringComparison.Ordinal)
        .Replace("<", "&lt;", StringComparison.Ordinal);

static List<string> CollectDocs(string root)
{
    var docsDir = Path.Combine(root, "docs");
    if (!Directory.Exists(docsDir))
        return [];

    return Directory.EnumerateFiles(docsDir, "*", SearchOption.AllDirectories)
        .Select(f => ToSlnRelativePath(root, f))
        .Order(StringComparer.Ordinal)
        .ToList();
}

/// <summary>
/// Root-level and hook-tooling files that define how the repo builds and is governed.
/// Paths are repo-relative with forward slashes.
/// </summary>
static List<string> CollectInfrastructure(string root)
{
        string[] candidates =
        [
            "README.md",
            "AGENTS.md",
            "Directory.Build.props",
            "Directory.Build.targets",
            "Directory.Packages.props",
            "global.json",
            "nuget.config",
            ".editorconfig",
            ".gitignore",
            ".gitattributes",
            "LICENSE",
            ".githooks/GenerateDocDiagrams.cs",
            ".githooks/UpdateSlnx.cs",
            ".githooks/pre-commit",
            ".githooks/README.md",
        ];

    return candidates
        .Where(rel => File.Exists(Path.Combine(root, rel)))
        .Select(rel => rel.Replace('\\', '/'))
        .Order(StringComparer.Ordinal)
        .ToList();
}

static List<string> DiscoverProjects(string root, string segment)
{
    var dir = Path.Combine(root, segment);
    if (!Directory.Exists(dir))
        return [];

    return Directory.EnumerateFiles(dir, "*.csproj", SearchOption.AllDirectories)
        .Select(f => ToSlnRelativePath(root, f))
        .Order(StringComparer.OrdinalIgnoreCase)
        .ToList();
}

static string ToSlnRelativePath(string root, string fullPath)
{
    var rel = Path.GetRelativePath(root, fullPath);
    return rel.Replace('\\', '/');
}

static string FindRepoRoot()
{
    var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (dir is not null)
    {
        if (File.Exists(Path.Combine(dir.FullName, "RoboSharp.slnx")))
            return dir.FullName;
        dir = dir.Parent;
    }

    throw new InvalidOperationException(
        "Could not locate RoboSharp.slnx; pass the repository root as the first argument.");
}
