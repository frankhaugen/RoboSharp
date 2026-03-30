using Microsoft.Extensions.DependencyInjection;
using RoboSharp.Application;
using RoboSharp.Workspaces;

namespace RoboSharp.Hosting;

public static class RoboSharpHostingServiceCollectionExtensions
{
    public static IServiceCollection AddRoboSharpHosting(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddRoboSharpWorkspaces();
        services.AddRoboSharpApplication();

        return services;
    }
}
