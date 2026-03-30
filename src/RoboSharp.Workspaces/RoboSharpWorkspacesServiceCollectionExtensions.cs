using Microsoft.Extensions.DependencyInjection;
using RoboSharp.IO;

namespace RoboSharp.Workspaces;

public static class RoboSharpWorkspacesServiceCollectionExtensions
{
    public static IServiceCollection AddRoboSharpWorkspaces(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IRoboPathService, RoboPathService>();
        services.AddSingleton<IProjectLoader, ProjectLoader>();
        services.AddSingleton<IBuildArtifactLayout, DefaultBuildArtifactLayout>();
        services.AddSingleton<IWorkspaceLoader, WorkspaceLoader>();

        return services;
    }
}
