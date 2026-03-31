namespace RoboSharp.Locales.English;

internal sealed class EnglishSidebarTexts : IStudioSidebarTexts
{
    public string LessonTaskHeading => "Your challenge";
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
    public string WorldDockTitle => "Robot & arena";
    public string WorldDockSubtitle =>
        "Legend: dark = wall, blue-gray = floor, teal = goal. Arrows = robot facing. " +
        "Build refreshes compiler stages; Run steps IL here at your chosen speed. " +
        "Full compiler messages: View → Compiler messages. Run transcript: View → Run report.\n\n" +
        EnglishTeachingExplainer.ProfilesVsGrammar;
}
