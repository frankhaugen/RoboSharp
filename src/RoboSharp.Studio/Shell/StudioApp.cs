using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using RoboSharp.Studio.Composition;

namespace RoboSharp.Studio.Shell;

public sealed class StudioApp : global::Avalonia.Application
{
    public override void Initialize()
    {
        Styles.Add(new FluentTheme());
        // Required for AvaloniaEdit TextEditor to paint text/line numbers with Fluent (see AvaloniaEdit install docs).
        Styles.Add(new StyleInclude(new Uri("avares://AvaloniaEdit"))
        {
            Source = new Uri("avares://AvaloniaEdit/Themes/Fluent/AvaloniaEdit.xaml"),
        });
        RequestedThemeVariant = ThemeVariant.Dark;
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = CompositionRoot.GetRequiredService<MainWindow>();

        base.OnFrameworkInitializationCompleted();
    }
}
