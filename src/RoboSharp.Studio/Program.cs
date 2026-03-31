using Avalonia;
#if DEBUG
using AvaloniaMcp.Diagnostics;
#endif
using Microsoft.Extensions.DependencyInjection;
using RoboSharp.Studio.Composition;

namespace RoboSharp.Studio;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddRoboSharpStudio();
        var provider = services.BuildServiceProvider();
        CompositionRoot.Initialize(provider);

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<Shell.StudioApp>()
            .UsePlatformDetect()
            .WithInterFont()
#if DEBUG
            .UseMcpDiagnostics()
#endif
            .LogToTrace();
}
