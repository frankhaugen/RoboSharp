using System.Xml.Linq;

namespace RoboSharp.Architecture.Tests;

/// <summary>Guards direct project-reference edges for teaching layers and hosts (see <c>AGENTS.md</c> dependency direction).</summary>
public class ProjectDependencyGuardTests
{
    private static readonly Dictionary<string, HashSet<string>> AllowedProjectReferences = new(StringComparer.OrdinalIgnoreCase)
    {
        ["RoboSharp.Language"] = new(StringComparer.OrdinalIgnoreCase),
        ["RoboSharp.Semantics"] = new(StringComparer.OrdinalIgnoreCase) { "RoboSharp.Language" },
        ["RoboSharp.IL"] = new(StringComparer.OrdinalIgnoreCase) { "RoboSharp.Semantics" },
        ["RoboSharp.World"] = new(StringComparer.OrdinalIgnoreCase),
        ["RoboSharp.Runtime"] = new(StringComparer.OrdinalIgnoreCase) { "RoboSharp.IL", "RoboSharp.World" },
        ["RoboSharp.IO"] = new(StringComparer.OrdinalIgnoreCase),
        ["RoboSharp.Workspaces"] = new(StringComparer.OrdinalIgnoreCase) { "RoboSharp.IO" },
        ["RoboSharp.Toolchain"] = new(StringComparer.OrdinalIgnoreCase)
        {
            "RoboSharp.Language",
            "RoboSharp.Semantics",
            "RoboSharp.IL",
            "RoboSharp.Runtime",
            "RoboSharp.World",
            "RoboSharp.IO",
            "RoboSharp.Workspaces",
        },
        ["RoboSharp.Application"] = new(StringComparer.OrdinalIgnoreCase)
        {
            "RoboSharp.IL",
            "RoboSharp.Runtime",
            "RoboSharp.Toolchain",
            "RoboSharp.Workspaces",
            "RoboSharp.World",
        },
        ["RoboSharp.Hosting"] = new(StringComparer.OrdinalIgnoreCase) { "RoboSharp.Application", "RoboSharp.Workspaces" },
        ["RoboSharp.Player"] = new(StringComparer.OrdinalIgnoreCase)
        {
            "RoboSharp.Application",
            "RoboSharp.IO",
            "RoboSharp.Toolchain",
            "RoboSharp.World",
        },
        ["RoboSharp.Studio"] = new(StringComparer.OrdinalIgnoreCase)
        {
            "RoboSharp.Hosting",
            "RoboSharp.Language",
            "RoboSharp.Toolchain",
            "RoboSharp.Semantics",
            "RoboSharp.IL",
            "RoboSharp.Runtime",
            "RoboSharp.World",
        },
        ["RoboSharp.Web"] = new(StringComparer.OrdinalIgnoreCase) { "RoboSharp.Hosting", "RoboSharp.World" },
    };

    [Test]
    public async Task Each_src_project_only_references_allowed_projects()
    {
        var repoRoot = FindRepoRoot();
        var srcRoot = Path.Combine(repoRoot, "src");
        await Assert.That(Directory.Exists(srcRoot)).IsTrue();

        foreach (var csprojPath in Directory.EnumerateFiles(srcRoot, "*.csproj", SearchOption.AllDirectories))
        {
            var projectName = Path.GetFileNameWithoutExtension(csprojPath);
            if (!AllowedProjectReferences.TryGetValue(projectName, out var allowed))
            {
                throw new InvalidOperationException(
                    $"Add '{projectName}' and its allowed ProjectReference set to {nameof(ProjectDependencyGuardTests)}.{nameof(AllowedProjectReferences)}.");
            }

            var doc = XDocument.Load(csprojPath);
            var references = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var include in doc.Descendants("ProjectReference").Select(e => e.Attribute("Include")?.Value))
            {
                if (string.IsNullOrWhiteSpace(include))
                    continue;
                var name = ProjectNameFromInclude(include);
                if (name is not null)
                    references.Add(name);
            }

            foreach (var r in references)
            {
                if (!allowed.Contains(r))
                {
                    throw new InvalidOperationException(
                        $"Project '{projectName}' references '{r}', which is not in the allowed set for this guard. Update the project or adjust {nameof(AllowedProjectReferences)} if intentional.");
                }
            }
        }
    }

    private static string? ProjectNameFromInclude(string include)
    {
        var file = Path.GetFileName(include.Trim());
        return file.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)
            ? Path.GetFileNameWithoutExtension(file)
            : null;
    }

    private static string FindRepoRoot()
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
