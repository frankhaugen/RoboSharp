using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoboSharp.Hosting;
using RoboSharp.Language;
using RoboSharp.Locales;
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

        services.AddSingleton<StudioLocaleHost>();
        services.AddSingleton<ITeachingLocale>(static sp => sp.GetRequiredService<StudioLocaleHost>());
        services.AddSingleton<IPipelineInspectionService, PipelineInspectionService>();
        services.AddSingleton<ISyntaxTreeSerializer, SyntaxTreeSerializer>();

        services.AddSingleton<MainWindowViewModel>();

        // Inspection panels: registration order is top-to-bottom in the stack (pipeline narrative).
        services.AddSingleton<IStudioPanel>(sp => new LessonToolboxPanel(sp.GetRequiredService<ITeachingLocale>()));
        services.AddSingleton<IStudioPanel>(sp => new TokenPipelinePanel(sp.GetRequiredService<ITeachingLocale>()));
        services.AddSingleton<IStudioPanel>(sp => new SyntaxTreePipelinePanel(
            sp.GetRequiredService<ISyntaxTreeSerializer>(),
            sp.GetRequiredService<ITeachingLocale>()));
        services.AddSingleton<IStudioPanel>(sp => new DiagnosticsPipelinePanel(sp.GetRequiredService<ITeachingLocale>()));
        services.AddSingleton<IStudioPanel>(sp => new BoundTreePipelinePanel(sp.GetRequiredService<ITeachingLocale>()));
        services.AddSingleton<IStudioPanel>(sp => new IlPipelinePanel(sp.GetRequiredService<ITeachingLocale>()));
        services.AddSingleton<IStudioPanel>(sp => new WorldRuntimePipelinePanel(sp.GetRequiredService<ITeachingLocale>()));

        services.AddSingleton<MainWindow>(static sp =>
            new MainWindow(
                sp.GetRequiredService<MainWindowViewModel>(),
                sp.GetRequiredService<StudioLocaleHost>(),
                sp.GetServices<IStudioPanel>()));

        return services;
    }
}
