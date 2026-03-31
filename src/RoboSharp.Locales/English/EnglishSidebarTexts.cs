namespace RoboSharp.Locales.English;

internal sealed class EnglishSidebarTexts : IStudioSidebarTexts
{
    public string StartHereHeading => "Start here";
    public string LessonPickerCaption => "Lesson (where you are in the track)";
    public string GoalCaption => "Goal (map / challenge — pick separately)";
    public string CommandsCaption => "Commands allowed (profile)";
    public string KeywordsHeading => "Keywords & built-ins for this lesson";
    public string SyntaxHeading => "Syntax reminders for this lesson";
    public string LoadLessonExampleButton => "Load this lesson's example into the editor";
    public string WorldPreviewHeading => "World preview";
    public string WorldPreviewHint =>
        "Legend: dark tiles = wall, blue-gray = floor, teal tint = goal tile. Arrows show which way the robot faces.\n\n" +
        "Build runs the compiler only and refreshes tokens, tree, diagnostics, bound tree, and IL panels. " +
        "Run compiles again, then executes on the goal map you picked at the step speed you chose (Realtime / Slow / Glacial).\n\n" +
        EnglishTeachingExplainer.ProfilesVsGrammar;
}
