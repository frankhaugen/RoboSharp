namespace RoboSharp.Locales.English;

internal sealed class EnglishSidebarTexts : IStudioSidebarTexts
{
    public string StartHereHeading => "What you're learning";
    public string LessonRibbonSubtitle =>
        "Choose a lesson on the ribbon — the practice map, compiler rules, and inspector tabs all follow that lesson.";
    public string LessonSectionGoalHeading => "Practice map (this lesson)";
    public string LessonWorldNameLabel => "Arena";
    public string LessonSectionCommandsHeading => "What you may write (this lesson)";
    public string LessonProfileNameLabel => "Compiler profile";
    public string LessonSectionReferenceHeading => "Quick reference";
    public string KeywordsHeading => "Keywords & built-ins for this lesson";
    public string SyntaxHeading => "Syntax reminders for this lesson";
    public string LoadLessonExampleButton => "Load this lesson's example into the editor";
    public string WorldPreviewHeading => "World preview";
    public string WorldPreviewHint =>
        "Legend: dark tiles = wall, blue-gray = floor, teal tint = goal tile. Arrows show which way the robot faces.\n\n" +
        "Build runs the compiler only and refreshes the pipeline tabs. " +
        "Run compiles again, then steps on this lesson’s arena at the speed you chose (Realtime / Slow / Glacial).\n\n" +
        EnglishTeachingExplainer.ProfilesVsGrammar;
}
