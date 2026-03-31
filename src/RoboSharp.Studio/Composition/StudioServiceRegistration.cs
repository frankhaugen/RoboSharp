using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoboSharp.Hosting;
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

        services.AddRoboSharpHosting();

        services.AddSingleton<IPipelineInspectionService, PipelineInspectionService>();
        services.AddSingleton<ISyntaxTreeSerializer, SyntaxTreeSerializer>();

        services.AddSingleton<MainWindowViewModel>();

        // Inspection panels: registration order is top-to-bottom in the stack (pipeline narrative).
        services.AddSingleton<IStudioPanel, ColoredSourcePreviewPanel>();
        services.AddSingleton<IStudioPanel, LessonToolboxPanel>();
        services.AddSingleton<IStudioPanel, TokenPipelinePanel>();
        services.AddSingleton<IStudioPanel, SyntaxTreePipelinePanel>();
        services.AddSingleton<IStudioPanel, DiagnosticsPipelinePanel>();
        services.AddSingleton<IStudioPanel, BoundTreePipelinePanel>();
        services.AddSingleton<IStudioPanel, IlPipelinePanel>();
        services.AddSingleton<IStudioPanel, WorldRuntimePipelinePanel>();

        services.AddSingleton<MainWindow>();

        return services;
    }
}
