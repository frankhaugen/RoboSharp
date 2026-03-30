using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoboSharp.Language;
using RoboSharp.Studio.Panels;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;
using RoboSharp.Studio.ViewModels;

namespace RoboSharp.Studio.Composition;

/// <summary>
/// Central DI registration for the Studio host — grouped for teaching (shell, pipeline, panels).
/// </summary>
public static class StudioServiceRegistration
{
    public static IServiceCollection AddRoboSharpStudio(this IServiceCollection services)
    {
        services.AddLogging(static b => b.AddConsole().SetMinimumLevel(LogLevel.Warning));

        services.AddSingleton<IPipelineInspectionService, PipelineInspectionService>();
        services.AddSingleton<ISyntaxTreeSerializer, SyntaxTreeSerializer>();

        services.AddSingleton<MainWindowViewModel>();

        // Inspection panels: registration order is visible tab order (didactic).
        services.AddSingleton<IStudioPanel, TokenPipelinePanel>();
        services.AddSingleton<IStudioPanel, SyntaxTreePipelinePanel>();
        services.AddSingleton<IStudioPanel, DiagnosticsPipelinePanel>();
        services.AddSingleton<IStudioPanel, BoundTreePlaceholderPanel>();
        services.AddSingleton<IStudioPanel, IlPlaceholderPanel>();
        services.AddSingleton<IStudioPanel, WorldPlaceholderPanel>();

        services.AddSingleton<MainWindow>();

        return services;
    }
}
