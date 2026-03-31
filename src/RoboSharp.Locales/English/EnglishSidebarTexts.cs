namespace RoboSharp.Locales.English;

internal sealed class EnglishSidebarTexts : IStudioSidebarTexts
{
    public string LessonAndMapHeading => "Lesson & map";
    public string ProfileCaption => "Profile (which commands exist for this lesson)";
    public string WorldCaption => "World (size, walls, and goal)";
    public string WorldPreviewHeading => "World preview";
    public string WorldPreviewHint =>
        "Legend: dark tiles = wall, blue-gray = floor, teal tint = goal tile. Arrows show which way the robot faces.\n\n" +
        "Build runs the compiler only and refreshes tokens, tree, diagnostics, bound tree, and IL panels. " +
        "Run compiles again, then executes on this map at the step speed you chose (Realtime / Slow / Glacial).\n\n" +
        EnglishTeachingExplainer.ProfilesVsGrammar;
}
