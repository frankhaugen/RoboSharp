namespace RoboSharp.Locales.Latin;

internal sealed class LatinStudioLessonCatalog : IStudioLessonCatalog
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
                "Primi motus",
                "Hinc incipe: tantum move() et turnLeft() necesse sunt. Aedifica ut tabulas doceas renoves, Curr ut robotem in meta quam infra eligis videas.",
                "Verba clavis: integer (genus), while, nomina procedurarum quas declares.\n" +
                "Imperata hac lectione: move, turnLeft.",
                "Sententiae finiuntur ;\n" +
                "Voca imperatum: move();\n" +
                "Procedura: void Nomen() { … }\n" +
                "while (condicio) { … } ad repetitionem.",
                StudioLessonSharedExamples.FirstMoves,
                "basic-movement"),

            [StudioLessonIds.Steering] = new(
                StudioLessonIds.Steering,
                "Gubernatio",
                "Adde turnRight() ut quovis vertas. Metam seorsum elige — lectio tantum quae imperata compilator sinat mutat.",
                "Eadem verba clavis; nulla nova.\n" +
                "Imperata: move, turnLeft, turnRight.",
                "Eadem regulae. Consocia conversiones et motus.",
                StudioLessonSharedExamples.Steering,
                "movement-turns"),

            [StudioLessonIds.LoopsAndPrint] = new(
                StudioLessonIds.LoopsAndPrint,
                "Gyri et print",
                "while ad repetendum, print() ad valores ostendendos. Charta meta tua electio manet.",
                "Verba: integer, while, print.\n" +
                "Imperata: move, turnLeft, turnRight, print (numeri et stringae inter signa).",
                "print(\"textus\");\nprint(42);\ninteger n = 0;\nwhile (n < 3) { … n = n + 1; }",
                StudioLessonSharedExamples.LoopsAndPrint,
                "movement-print"),

            [StudioLessonIds.Sensing] = new(
                StudioLessonIds.Sensing,
                "Parietes videre",
                "Interroga reticulum cum frontIsClear(), leftIsClear(), rightIsClear(). Cum while conjunge; labyrinthum elige cum paratus sis.",
                "Nova imperata: frontIsClear, leftIsClear, rightIsClear.",
                "Exemplum: while (frontIsClear()) { move(); }",
                StudioLessonSharedExamples.Sensing,
                "with-sensing"),

            [StudioLessonIds.FullLanguage] = new(
                StudioLessonIds.FullLanguage,
                "Arca plena",
                "Omnia quae profilium docendi sinit. Programmata maiora exerce. Metam seorsum elige.",
                "Verba priora et quae cursus addit (vide tabulam instrumentorum post Aedifica).\n" +
                "Pro singulis nominibus vide panellem \"Lesson toolbox\".",
                "Eadem regulae RoboSharp; proceduris longa cohibe.",
                StudioLessonSharedExamples.FullLanguage,
                "full"),
        };

        return map;
    }
}
