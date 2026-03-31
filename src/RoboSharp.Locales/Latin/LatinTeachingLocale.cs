namespace RoboSharp.Locales.Latin;

/// <summary>
/// Optional demo locale: playful Latin for classrooms and tests. Enable Studio with environment variable
/// <c>ROBOSHARP_LOCALE=la</c> (see <see cref="TeachingLocaleResolver"/>).
/// </summary>
public sealed class LatinTeachingLocale : ITeachingLocale
{
    public LatinTeachingLocale()
    {
        Shell = new LatinShellTexts();
        Sidebar = new LatinSidebarTexts();
        Panels = new LatinPanelTexts();
        Pipeline = new LatinPipelineTexts();
    }

    /// <summary>BCP 47 tag for Latin.</summary>
    public string LocaleId => "la";

    public IStudioShellTexts Shell { get; }
    public IStudioSidebarTexts Sidebar { get; }
    public IStudioPanelTexts Panels { get; }
    public IPipelineTeachingTexts Pipeline { get; }
}
