namespace RoboSharp.Locales.English;

internal sealed class EnglishStudioLessonCatalog : IStudioLessonCatalog
{
    private static readonly Dictionary<string, StudioLessonDefinition> ById = CreateMap();

    public IReadOnlyList<StudioLessonDefinition> OrderedLessons { get; } =
    [
        ById[StudioLessonIds.FirstMoves],
        ById[StudioLessonIds.Steering],
        ById[StudioLessonIds.LoopsAndPrint],
        ById[StudioLessonIds.Sensing],
        ById[StudioLessonIds.FullLanguage],
    ];

    public StudioLessonDefinition Get(string lessonId) =>
        ById.TryGetValue(lessonId, out var d) ? d : OrderedLessons[0];

    private static Dictionary<string, StudioLessonDefinition> CreateMap()
    {
        var map = new Dictionary<string, StudioLessonDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            [StudioLessonIds.FirstMoves] = new(
                StudioLessonIds.FirstMoves,
                "First moves",
                "Use only move() and turnLeft(). Build, then Run, and get the robot to the goal on this lesson’s map.",
                "Keywords: integer (type), while, procedure names you declare.\n" +
                "Built-ins in this lesson: move, turnLeft.",
                "Syntax: statements end with ;\n" +
                "Call a built-in like move();\n" +
                "Declare a procedure: void Name() { … }\n" +
                "Use while (condition) { … } for repetition.",
                StudioLessonSharedExamples.FirstMoves,
                "basic-movement",
                "goal-corner",
                "This lesson’s challenge is a compact map with a clear goal tile. Run always uses this arena so you can focus on a few commands and see cause and effect immediately.",
                "Your program may only call built-ins that this lesson has introduced. If a name is rejected, it usually means the lesson has not unlocked that command yet — not that the spelling is wrong.",
                [
                    StudioPanelIds.LessonToolbox,
                    StudioPanelIds.Tokens,
                ]),

            [StudioLessonIds.Steering] = new(
                StudioLessonIds.Steering,
                "Steering",
                "Use turnRight() too—plan turns and moves so the robot reaches the goal.",
                "Keywords: same as before; no new keywords.\n" +
                "Built-ins: move, turnLeft, turnRight.",
                "Same statement rules. Chain turns and moves to face any direction.",
                """
                // Lesson: steering — face each direction then step forward.
                move();
                turnRight();
                move();
                turnRight();
                move();

                """,
                "movement-turns",
                "goal-corner",
                "Same style of goal map as before: a short path and a visible goal. Practice steering with both turn directions without juggling a new arena shape yet.",
                "turnRight joins the allowed built-ins. The compiler profile for this lesson is fixed — read the section below to see its friendly name and the toolbox panel after Build for the exact list.",
                [
                    StudioPanelIds.LessonToolbox,
                    StudioPanelIds.Tokens,
                ]),

            [StudioLessonIds.LoopsAndPrint] = new(
                StudioLessonIds.LoopsAndPrint,
                "Loops and print",
                "Repeat work with while; show numbers and strings with print(). Run and read what the program prints live.",
                "Keywords: integer, while, print.\n" +
                "Built-ins: move, turnLeft, turnRight, print (numbers and strings in quotes).",
                "print(\"text\");\nprint(42);\ninteger n = 0;\nwhile (n < 3) { … n = n + 1; }",
                StudioLessonSharedExamples.LoopsAndPrint,
                "movement-print",
                "open-playground",
                "An open floor with a goal gives you space to repeat patterns, watch the robot travel farther, and read print() lines without tight corners getting in the way.",
                "while and print are now part of what the binder accepts, together with the movement commands you already know. Build loads the active profile into the Lesson toolbox tab.",
                [
                    StudioPanelIds.LessonToolbox,
                    StudioPanelIds.Tokens,
                    StudioPanelIds.SyntaxTree,
                ]),

            [StudioLessonIds.Sensing] = new(
                StudioLessonIds.Sensing,
                "Seeing walls",
                "Ask the grid with frontIsClear, leftIsClear, and rightIsClear; loop so the robot follows the corridor maze without bumping walls.",
                "Built-ins added: frontIsClear, leftIsClear, rightIsClear (return truth you can use in while/if when available).",
                "Typical pattern: while (frontIsClear()) { move(); }",
                StudioLessonSharedExamples.Sensing,
                "with-sensing",
                "corridor-maze",
                "A corridor maze makes sensing meaningful: walls constrain you, so the robot must look before it steps. This is the lesson’s practice geography — Run always uses it.",
                "The profile adds the three clear-predicates. Use the syntax tree and diagnostics tabs to confirm the parser and binder understand your conditions before you worry about maze strategy.",
                [
                    StudioPanelIds.LessonToolbox,
                    StudioPanelIds.Tokens,
                    StudioPanelIds.SyntaxTree,
                    StudioPanelIds.BoundTree,
                ]),

            [StudioLessonIds.FullLanguage] = new(
                StudioLessonIds.FullLanguage,
                "Full toolbox",
                "Combine everything: longer procedures, the full built-in set, and this arena—trace Tokens through IL, then Run to the goal.",
                "Includes prior keywords plus whatever your course adds (arrays, pick/drop, etc. when in profile).\n" +
                "See the Lesson toolbox panel after Build for the exact list.",
                "Same RoboSharp statement and block rules; use procedures to organize longer code.",
                StudioLessonSharedExamples.FullLanguage,
                "full",
                "arena-12",
                "A medium arena balances space and structure: enough room for bigger programs while keeping goals and walls legible on screen.",
                "The full teaching profile name below matches the Lesson profile the compiler uses. After Build, the Lesson toolbox tab is the authoritative checklist of names you can call.",
                [
                    StudioPanelIds.LessonToolbox,
                    StudioPanelIds.Tokens,
                    StudioPanelIds.SyntaxTree,
                    StudioPanelIds.BoundTree,
                    StudioPanelIds.Il,
                ]),
        };

        return map;
    }
}
