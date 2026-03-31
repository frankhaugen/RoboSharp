namespace RoboSharp.Locales;

/// <summary>Ordered teaching lessons for Studio: each ties a profile default, copy, and starter source.</summary>
public interface IStudioLessonCatalog
{
    /// <summary>Stable ids in teaching order (first = gentlest on-ramp).</summary>
    IReadOnlyList<StudioLessonDefinition> OrderedLessons { get; }

    /// <summary>Returns the lesson for <paramref name="lessonId"/> or the first lesson if unknown.</summary>
    StudioLessonDefinition Get(string lessonId);
}

/// <param name="DefaultProfileId">Suggested <c>LessonBuiltinProfiles</c> id applied when the user picks this lesson.</param>
/// <param name="VisiblePanelIds">Which inspector tabs to show (<see cref="StudioPanelIds"/>). Order in the UI follows each panel's <c>Order</c>.</param>
public sealed record StudioLessonDefinition(
    string Id,
    string Title,
    string StartHereBlurb,
    string KeywordsSection,
    string SyntaxSection,
    string ExampleSource,
    string DefaultProfileId,
    IReadOnlyList<string> VisiblePanelIds);
