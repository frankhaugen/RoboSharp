namespace RoboSharp.Locales;

/// <summary>Ordered teaching lessons for Studio: each ties a profile default, copy, and starter source.</summary>
public interface IStudioLessonCatalog
{
    /// <summary>Stable ids in teaching order (first = gentlest on-ramp).</summary>
    IReadOnlyList<StudioLessonDefinition> OrderedLessons { get; }

    /// <summary>Returns the lesson for <paramref name="lessonId"/> or the first lesson if unknown.</summary>
    StudioLessonDefinition Get(string lessonId);
}

/// <param name="DefaultProfileId"><c>LessonBuiltinProfiles</c> id — fixed for this lesson (ribbon switch updates it).</param>
/// <param name="DefaultWorldPresetId"><c>RobotWorldPresets</c> id — practice map for this lesson.</param>
/// <param name="GoalSectionBody">Didactic copy for the practice-map section (not the map name; that comes from presets).</param>
/// <param name="CommandsSectionBody">Didactic copy for what the compiler allows in this lesson.</param>
/// <param name="TaskChallengeBody">Left-rail mission copy: what the learner should try (imperative, short).</param>
/// <param name="VisiblePanelIds">Which inspector tabs to show (<see cref="StudioPanelIds"/>). Order in the UI follows each panel's <c>Order</c>.</param>
public sealed record StudioLessonDefinition(
    string Id,
    string Title,
    string TaskChallengeBody,
    string KeywordsSection,
    string SyntaxSection,
    string ExampleSource,
    string DefaultProfileId,
    string DefaultWorldPresetId,
    string GoalSectionBody,
    string CommandsSectionBody,
    IReadOnlyList<string> VisiblePanelIds);
