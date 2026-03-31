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
                "Tantum move() et turnLeft() nunc necesse sunt. Aedifica ut tabulas renoves, Curr ut robotem programma tuum in huius lectionis campo exercitationis agere videas.",
                "Verba clavis: integer (genus), while, nomina procedurarum quas declares.\n" +
                "Imperata hac lectione: move, turnLeft.",
                "Sententiae finiuntur ;\n" +
                "Voca imperatum: move();\n" +
                "Procedura: void Nomen() { … }\n" +
                "while (condicio) { … } ad repetitionem.",
                StudioLessonSharedExamples.FirstMoves,
                "basic-movement",
                "goal-corner",
                "Provocatio huius lectionis est charta parva cum tegula meta clara. Curr semper in hoc agro utitur ut pauca imperata discas et effectum statim videas.",
                "Programma tantum builtina quae haec lectio introduxit vocare potest. Si nomen reicitur, saepe significat imperatum nondum lectio aperuit — non orthographiam falsam esse.",
                [
                    StudioPanelIds.LessonToolbox,
                    StudioPanelIds.Diagnostics,
                    StudioPanelIds.WorldRuntime,
                ]),

            [StudioLessonIds.Steering] = new(
                StudioLessonIds.Steering,
                "Gubernatio",
                "Adde turnRight() ut quamvis partem versus. Fabula manet «robotem quo vis ducere», sed nunc conversiones eligere potes.",
                "Eadem verba clavis; nulla nova.\n" +
                "Imperata: move, turnLeft, turnRight.",
                "Eadem regulae. Consocia conversiones et motus.",
                StudioLessonSharedExamples.Steering,
                "movement-turns",
                "goal-corner",
                "Eadem fere charta meta atque antea: iter breve et meta perspicua. Exerce gubernationem cum ambabus conversionibus sine forma agri nova.",
                "turnRight ad imperata licita accedit. Profilium huius lectionis fixum est — nomen infra lege et post Aedifica tabulam instrumentorum.",
                [
                    StudioPanelIds.LessonToolbox,
                    StudioPanelIds.Tokens,
                    StudioPanelIds.Diagnostics,
                    StudioPanelIds.WorldRuntime,
                ]),

            [StudioLessonIds.LoopsAndPrint] = new(
                StudioLessonIds.LoopsAndPrint,
                "Gyri et print",
                "while ad repetendum, print() ad numeros et textum ostendendos. Gyri formas faciunt; print() exitum in cursu visibilem reddit.",
                "Verba: integer, while, print.\n" +
                "Imperata: move, turnLeft, turnRight, print (numeri et stringae inter signa).",
                "print(\"textus\");\nprint(42);\ninteger n = 0;\nwhile (n < 3) { … n = n + 1; }",
                StudioLessonSharedExamples.LoopsAndPrint,
                "movement-print",
                "open-playground",
                "Solum apertum cum meta spatium dat ad formas repetendas, robotem longius ire spectandum, et lineas print() sine angulis arctis legere.",
                "while et print nunc pars sunt quam nexus accipit, cum imperatis motus quae iam nosti. Aedifica ut profilium activum in tabulam instrumentorum trahas.",
                [
                    StudioPanelIds.LessonToolbox,
                    StudioPanelIds.Tokens,
                    StudioPanelIds.SyntaxTree,
                    StudioPanelIds.Diagnostics,
                    StudioPanelIds.WorldRuntime,
                ]),

            [StudioLessonIds.Sensing] = new(
                StudioLessonIds.Sensing,
                "Parietes videre",
                "Interroga reticulum cum frontIsClear(), leftIsClear(), rightIsClear(). Cum while conjunge ut meatus petas, non caecum offendas.",
                "Nova imperata: frontIsClear, leftIsClear, rightIsClear.",
                "Exemplum: while (frontIsClear()) { move(); }",
                StudioLessonSharedExamples.Sensing,
                "with-sensing",
                "corridor-maze",
                "Labyrinthus meatus sensum necessarium facit: parietes cohibent, ergo robot antequam graditur videre debet. Haec est geographia exercitationis huius lectionis.",
                "Profilium tres praedicata clara addit. Arbor syntactica et Diagnostica adde ut parser et nexus condiciones tuas intellegant.",
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
                "Arca plena",
                "Omnia quae profilium docendi sinit: procedurae longiores, aenigmata opulentiora, omnia priora in uno loco.",
                "Verba priora et quae cursus addit (vide tabulam instrumentorum post Aedifica).\n" +
                "Pro singulis nominibus vide panellem \"Lesson toolbox\".",
                "Eadem regulae RoboSharp; proceduris longa cohibe.",
                StudioLessonSharedExamples.FullLanguage,
                "full",
                "arena-12",
                "Medium ager spatium et structuram temperat: satis loci programmatibus maioribus, meta et parietes in scaena clarae.",
                "Nomen profili pleni infra congruit cum profilo quod compilator usat. Post Aedifica tabula instrumentorum index auctoritativus nominum licitorum est.",
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
