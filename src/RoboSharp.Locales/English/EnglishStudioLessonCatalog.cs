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
                "Start here: you only need move() and turnLeft(). Run Build to refresh the teaching panels, then Run to watch the robot on the goal map you pick below.",
                "Keywords: integer (type), while, procedure names you declare.\n" +
                "Built-ins in this lesson: move, turnLeft.",
                "Syntax: statements end with ;\n" +
                "Call a built-in like move();\n" +
                "Declare a procedure: void Name() { … }\n" +
                "Use while (condition) { … } for repetition.",
                StudioLessonSharedExamples.FirstMoves,
                "basic-movement",
                [
                    StudioPanelIds.LessonToolbox,
                    StudioPanelIds.Diagnostics,
                    StudioPanelIds.WorldRuntime,
                ]),

            [StudioLessonIds.Steering] = new(
                StudioLessonIds.Steering,
                "Steering",
                "Add turnRight() so you can steer anywhere. Pick a goal map separately — the lesson only changes which commands the compiler allows.",
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
                [
                    StudioPanelIds.LessonToolbox,
                    StudioPanelIds.Tokens,
                    StudioPanelIds.Diagnostics,
                    StudioPanelIds.WorldRuntime,
                ]),

            [StudioLessonIds.LoopsAndPrint] = new(
                StudioLessonIds.LoopsAndPrint,
                "Loops and print",
                "Use while to repeat and print() to show values. Your goal map is still your choice — practice the same code on different arenas.",
                "Keywords: integer, while, print.\n" +
                "Built-ins: move, turnLeft, turnRight, print (numbers and strings in quotes).",
                "print(\"text\");\nprint(42);\ninteger n = 0;\nwhile (n < 3) { … n = n + 1; }",
                StudioLessonSharedExamples.LoopsAndPrint,
                "movement-print",
                [
                    StudioPanelIds.LessonToolbox,
                    StudioPanelIds.Tokens,
                    StudioPanelIds.SyntaxTree,
                    StudioPanelIds.Diagnostics,
                    StudioPanelIds.WorldRuntime,
                ]),

            [StudioLessonIds.Sensing] = new(
                StudioLessonIds.Sensing,
                "Seeing walls",
                "Ask the grid questions with frontIsClear(), leftIsClear(), rightIsClear(). Combine with while to navigate; pick a maze-style goal when you are ready.",
                "Built-ins added: frontIsClear, leftIsClear, rightIsClear (return truth you can use in while/if when available).",
                "Typical pattern: while (frontIsClear()) { move(); }",
                StudioLessonSharedExamples.Sensing,
                "with-sensing",
                [
                    StudioPanelIds.LessonToolbox,
                    StudioPanelIds.Tokens,
                    StudioPanelIds.SyntaxTree,
                    StudioPanelIds.Diagnostics,
                    StudioPanelIds.BoundTree,
                    StudioPanelIds.WorldRuntime,
                ]),

            [StudioLessonIds.FullLanguage] = new(
                StudioLessonIds.FullLanguage,
                "Full toolbox",
                "Everything the teaching profile allows: practice larger programs. Still choose your goal map on its own — this lesson only widens the command set.",
                "Includes prior keywords plus whatever your course adds (arrays, pick/drop, etc. when in profile).\n" +
                "See the Lesson toolbox panel after Build for the exact list.",
                "Same RoboSharp statement and block rules; use procedures to organize longer code.",
                StudioLessonSharedExamples.FullLanguage,
                "full",
                [
                    StudioPanelIds.LessonToolbox,
                    StudioPanelIds.Tokens,
                    StudioPanelIds.SyntaxTree,
                    StudioPanelIds.Diagnostics,
                    StudioPanelIds.BoundTree,
                    StudioPanelIds.Il,
                    StudioPanelIds.WorldRuntime,
                ]),
        };

        return map;
    }
}
