namespace RoboSharp.Locales.English;

/// <summary>Default English teaching copy. Swap <see cref="ITeachingLocale"/> in DI for another language pack (also code-based).</summary>
public sealed class EnglishTeachingLocale : ITeachingLocale
{
    public EnglishTeachingLocale()
    {
        Shell = new EnglishShellTexts();
        Sidebar = new EnglishSidebarTexts();
        Lessons = new EnglishStudioLessonCatalog();
        Panels = new EnglishPanelTexts();
        Pipeline = new EnglishPipelineTexts();
    }

    public string LocaleId => "en";

    public IStudioShellTexts Shell { get; }
    public IStudioSidebarTexts Sidebar { get; }
    public IStudioLessonCatalog Lessons { get; }
    public IStudioPanelTexts Panels { get; }
    public IPipelineTeachingTexts Pipeline { get; }
}
