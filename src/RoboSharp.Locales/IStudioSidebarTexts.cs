namespace RoboSharp.Locales;

public interface IStudioSidebarTexts
{
    /// <summary>Hero heading for the left rail (mission / task).</summary>
    string LessonTaskHeading { get; }
    /// <summary>Short line under the lesson ribbon (teaching chrome).</summary>
    string LessonRibbonSubtitle { get; }
    string LessonSectionGoalHeading { get; }
    string LessonWorldNameLabel { get; }
    string LessonSectionCommandsHeading { get; }
    string LessonProfileNameLabel { get; }
    string LessonSectionReferenceHeading { get; }
    string KeywordsHeading { get; }
    string SyntaxHeading { get; }
    string LoadLessonExampleButton { get; }
    /// <summary>Title above the world grid in the bottom dock.</summary>
    string WorldDockTitle { get; }
    /// <summary>Hint under the world grid (legend, Build vs Run).</summary>
    string WorldDockSubtitle { get; }
}
