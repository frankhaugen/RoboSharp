namespace RoboSharp.Locales;

/// <summary>
/// Culture-specific teaching copy for RoboSharp hosts. Implementations use hardcoded strings in C# — no .resx —
/// so the text stays versioned, grep-friendly, and easy to inspect in class.
/// </summary>
public interface ITeachingLocale
{
    /// <summary>BCP 47-style tag, e.g. <c>en</c>.</summary>
    string LocaleId { get; }

    IStudioShellTexts Shell { get; }
    IStudioSidebarTexts Sidebar { get; }
    IStudioPanelTexts Panels { get; }
    IPipelineTeachingTexts Pipeline { get; }
}