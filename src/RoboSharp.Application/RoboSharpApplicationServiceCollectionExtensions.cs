using Microsoft.Extensions.DependencyInjection;
using RoboSharp.Toolchain;

namespace RoboSharp.Application;

public static class RoboSharpApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddRoboSharpApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<WorkspaceBuildService>();
        services.AddSingleton<IRoboSharpExecutionService, RoboSharpExecutionService>();

        return services;
    }
}
