using System.Text;
using RoboSharp.Language;
using RoboSharp.Workspaces;

namespace RoboSharp.Toolchain;

/// <summary>Builds a single compilation unit from workspace sources and writes IL / executable (and optional syntax dump) artifacts.</summary>
public sealed class WorkspaceBuildService
{
    public async ValueTask<WorkspaceBuildResult> BuildAsync(IRoboWorkspace workspace, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(workspace);

        var combined = await CombineSourcesAsync(workspace, cancellationToken).ConfigureAwait(false);
        var compileResult = RoboSharpCompiler.Compile(combined);

        if (!compileResult.Succeeded)
        {
            return new WorkspaceBuildResult { Success = false, CompileResult = compileResult };
        }

        var config = workspace.ActiveConfiguration;
        var json = RoboExecutableJsonSerializer.Serialize(compileResult.Executable!);

        var ilFile = workspace.GetArtifactFile(ArtifactKind.Il, config);
        var exeFile = workspace.GetArtifactFile(ArtifactKind.Executable, config);

        await ilFile.WriteAllTextAsync(json, cancellationToken).ConfigureAwait(false);
        await exeFile.WriteAllTextAsync(json, cancellationToken).ConfigureAwait(false);

        if (compileResult.SyntaxTree is not null)
        {
            var serializer = new SyntaxTreeSerializer();
            var syntaxFile = workspace.GetArtifactFile(
                ArtifactKind.SyntaxTree,
                config,
                workspace.Project.StartupFileRelative);
            await syntaxFile.WriteAllTextAsync(serializer.Serialize(compileResult.SyntaxTree.Root), cancellationToken)
                .ConfigureAwait(false);
        }

        return new WorkspaceBuildResult { Success = true, CompileResult = compileResult };
    }

    private static async Task<string> CombineSourcesAsync(IRoboWorkspace workspace, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();
        foreach (var file in workspace.GetSourceFiles())
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }

            sb.Append(await file.ReadAllTextAsync(cancellationToken).ConfigureAwait(false));
        }

        return sb.ToString();
    }
}
