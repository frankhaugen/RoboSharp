using RoboSharp.Locales;
using RoboSharp.Studio.Settings;

namespace RoboSharp.Studio.Shell;

/// <summary>
/// Dynamically changeable <see cref="ITeachingLocale"/> for Studio: <see cref="SetLocaleId"/> swaps the active language pack at runtime
/// (no app restart). Persisted choice overrides <see cref="TeachingLocaleResolver.EnvironmentVariableName"/> when set; otherwise the
/// environment variable applies on first launch only.
/// </summary>
public sealed class StudioLocaleHost : ITeachingLocale
{
    private ITeachingLocale _inner;

    public StudioLocaleHost()
    {
        var file = StudioUserSettingsStore.Load();
        var id = string.IsNullOrWhiteSpace(file.LocaleId)
            ? Environment.GetEnvironmentVariable(TeachingLocaleResolver.EnvironmentVariableName)
            : file.LocaleId;
        _inner = TeachingLocaleResolver.Create(id);
    }

    /// <summary>Fired after <see cref="SetLocaleId"/> switches the active pack.</summary>
    public event EventHandler? Changed;

    public string LocaleId => _inner.LocaleId;

    public IStudioShellTexts Shell => _inner.Shell;

    public IStudioSidebarTexts Sidebar => _inner.Sidebar;

    public IStudioLessonCatalog Lessons => _inner.Lessons;

    public IStudioPanelTexts Panels => _inner.Panels;

    public IPipelineTeachingTexts Pipeline => _inner.Pipeline;

    /// <summary>
    /// Switches UI copy immediately (dynamic locale change at runtime). Pass <c>null</c>, empty, or any unknown id for English;
    /// <c>la</c> / <c>latin</c> for the demo pack. Persists <see cref="LocaleId"/> to disk and raises <see cref="Changed"/>.
    /// </summary>
    public void SetLocaleId(string? localeId)
    {
        var next = TeachingLocaleResolver.Create(localeId);
        if (string.Equals(next.LocaleId, _inner.LocaleId, StringComparison.Ordinal))
            return;

        _inner = next;
        StudioUserSettingsStore.Save(new StudioUserSettings { LocaleId = _inner.LocaleId });
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
