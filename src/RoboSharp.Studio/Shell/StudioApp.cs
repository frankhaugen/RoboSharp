using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using RoboSharp.Studio.Composition;

namespace RoboSharp.Studio.Shell;

public sealed class StudioApp : global::Avalonia.Application
{
    public override void Initialize()
    {
        Styles.Add(new FluentTheme());
        RequestedThemeVariant = ThemeVariant.Dark;
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = CompositionRoot.GetRequiredService<MainWindow>();

        base.OnFrameworkInitializationCompleted();
    }
}
