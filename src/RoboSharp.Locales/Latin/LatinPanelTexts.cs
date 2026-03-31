namespace RoboSharp.Locales.Latin;

internal sealed class LatinPanelTexts : IStudioPanelTexts
{
    public string ColoredSourceTitle => "Colores syntaxeos";
    public string ColoredSourceSubtitle =>
        "Verba clavis, literalia, commentaria tincta post ultimam Aedificationem. Editor principalis vivus est; haec tabula est imago lexici cum Aedificas.";
    public string ColoredSourcePreamble =>
        "# Colores syntaxeos (imago lexici)\r\n" +
        LatinTeachingExplainer.LexerToParser +
        "\r\n\r\nHaec tabula eundem textum cum coloribus lexici ostendit. Aedifica ut post mutationes renoves.\r\n\r\n";
    public string ColoredSourceEmpty => "(Nihil ostendere — fons vacuus est aut Aedificatio nondum cucurrit.)";

    public string LessonToolboxTitle => "Arca instrumentorum lectionis";
    public string LessonToolboxSubtitle =>
        "Builtina quae profilium quod elegisti permittit. Profilia minora aenigmata continent; nomina ignota saepe profilium significant, non errorem grammaticae.";
    public string LessonToolboxPreamble =>
        "# Arca instrumentorum\r\n" +
        LatinTeachingExplainer.ProfilesVsGrammar +
        "\r\n\r\nIndex infra congruit cum selecto profili lectionis.\r\n\r\n";
    public string LessonToolboxBuildPrompt =>
        "(Semel aedifica ut profilium activum et orbis titulos in imaginem trahas.)";

    public string TokensTitle => "Signa";
    public string TokensSubtitle =>
        "Exitus lexici: una linea per signum (genus, index initii, longitudo, textus effugatus). Copia cum Ctrl+A, Ctrl+C intra arcam.";
    public string TokensPreamble =>
        "# Signa lexica (analysis lexicalis)\r\n" +
        "Quaelibet linea unum signum: genus, index @initii, longitudo, et litterae ipsae (\\r, \\n, \\t effugatae).\r\n" +
        LatinTeachingExplainer.LexerToParser +
        "\r\n\r\n";

    public string SyntaxTreeTitle => "Arbor syntactica";
    public string SyntaxTreeSubtitle =>
        "Exitus syntactici: nodi inclusi. Arbor bona + diagnostica saepe significant gradum proximum esse nexum, non lexicum.";
    public string SyntaxTreePreamble =>
        "# Arbor syntactica (parsing)\r\n" +
        "Syntaxis concreta: indentatio ostendit quomodo signa in declarationes, sententias, expressiones coierint.\r\n" +
        LatinTeachingExplainer.PhasesDependOnEachOther +
        "\r\n\r\n";

    public string DiagnosticsTitle => "Diagnostica";
    public string DiagnosticsSubtitle =>
        "Nuntii syntactici, significativi, et exsecutorii ab ultima Aedificatione vel Cursu. Quaelibet linea gradu signata est.";
    public string DiagnosticsPreamble =>
        "# Diagnostica (compilatorem et interpretem)\r\n" +
        LatinTeachingExplainer.WhatIsASourceSpan +
        "\r\n\r\n" +
        "• lexica — lexicum/syntacticus arborem validam facere non potuit.\r\n" +
        "• significativa — regulae nexūs/types post syntaxin felicem defecerunt. " + LatinTeachingExplainer.WhatIsBinding + "\r\n" +
        "• exsecutorius — interpres culpam post IL demissam nuntiavit (post Curr). " + LatinTeachingExplainer.StdoutVsDiagnostics + "\r\n" +
        "\r\n";

    public string DiagnosticsNone =>
        "(Nulla diagnostica in his gradibus — ultima Aedificatio/Cursus nullas culpas hic nuntiavit.)";

    public string DiagnosticsRuntimePrefix => "exsecutorius ";

    public string BoundTreeTitle => "Arbor nexa";
    public string BoundTreeSubtitle =>
        "Analysis significativa: singulum nomen ad symbolum resolutum, omnis expressio typata. Hinc demissio IL fictam facit.";
    public string BoundTreePreamble =>
        "# Arbor nexa (analysis significativa)\r\n" +
        LatinTeachingExplainer.WhatIsBinding +
        "\r\n\r\nEffusio infra est stratum significationis quod gradus demissionis IL consumit.\r\n\r\n";

    public string BoundTreeNeedParseFirst =>
        "Nexus post syntaxin felicem tantum currit. Primum diagnostica syntactica corrige (arbor + titulus tabulae), dein Aedifica.";
    public string BoundTreeSemanticsStopped =>
        "Analysis significativa cito substitit — vide Diagnostica pro nuntiis nexūs. Saepe discordia typorum aut nomen ignotum in profilio.";
    public string BoundTreeUnexpectedEmpty =>
        "(Textus arboris nexae deest quamquam demissio successum nuntiavit — inopinatum; vide Diagnostica et indicium mitte si manet.)";
    public string BoundTreeBuildPrompt => "Aedifica ut ductum renoves et hanc tabulam impleas cum compilatio felix est.";

    public string IlTitle => "IL (demissa)";
    public string IlSubtitle =>
        "Dissectio IL fictae: opcodes et operandi quos interpres agit. Non IL CLR — vide explicator in nota post Curr.";
    public string IlPreamble =>
        "# IL ficta (demissio)\r\n" +
        LatinTeachingExplainer.FakeIlVersusDotNet +
        "\r\n\r\n";

    public string IlWaitingForLowering =>
        "IL post nexum et demissionem felicem apparet: typi validi, nomina resoluta, et introitus currens. Diagnostica significativa clara fac, dein Aedifica.";
    public string IlNoTextUnexpected =>
        "(Demissio successum nuntiavit sed textus IL deest — inopinatum. Aedifica iterum et Diagnostica vide.)";

    public string IlCopyDisassembly => "Copia textum IL";

    public string WorldRuntimeTitle => "Orbis et interpres";
    public string WorldRuntimeSubtitle =>
        "Post Curr: summarium orbis, perfectio aut culpa, print() stdout, et stderr. Secta signata sunt ut exempla maneant clara.";
    public string FormatWorldRuntimePanel(
        bool? runtimeSucceeded,
        bool hasRunnableIl,
        string? lessonOutcome,
        int? lessonScore,
        string? worldAfterRunSummary,
        string? runtimeFaultMessage,
        string? runtimeStdout,
        string? runtimeStderr)
    {
        var doc =
            "# Orbis et interpres\r\n" +
            "Aedificatio tantum compila. Curr iterum compila, dein IL in orbe Kareliano gradatim agit.\r\n" +
            LatinTeachingExplainer.StdoutVsDiagnostics +
            "\r\n\r\n";

        if (runtimeSucceeded is null)
        {
            if (hasRunnableIl)
            {
                return doc +
                    "## Status exsecutionis\r\n" +
                    "Programma pro hac imagine feliciter compilatum est, sed interpres nondum cucurrit.\r\n" +
                    "\r\n" +
                    "## Quid deinde\r\n" +
                    "Preme Curr. Studium iterum compila, dein IL ad celeritatem electam gradatim agit ut robotem spectes.\r\n" +
                    "\r\n" +
                    "## Exitus standardus (print)\r\n" +
                    "Exitus ex print() in programmate. Post cursum felicem impletur.\r\n" +
                    "\r\n" +
                    "(nondum cursum)\r\n" +
                    "\r\n" +
                    "## Error standardus\r\n" +
                    "Canalis exsecutorius pro culpis et finibus — non idem ac print().\r\n" +
                    "\r\n" +
                    "(nondum cursum)\r\n";
            }

            return doc +
                "## Status exsecutionis\r\n" +
                "Demissio nondum programma currens effecit, ergo Curr IL sensatum exsequi non potest.\r\n" +
                "\r\n" +
                "## Exitus standardus (print)\r\n" +
                "(non praesto donec compilatio felix sit)\r\n" +
                "\r\n" +
                "## Error standardus\r\n" +
                "(non praesto donec compilatio felix sit)\r\n" +
                "\r\n" +
                "Monitum: voca imperata ad summum scopum (exempli gratia move();) et diagnostica syntactica/significativa clara fac. " +
                LatinTeachingExplainer.PhasesDependOnEachOther +
                "\r\n";
        }

        var goalSection =
            lessonOutcome is { } lo
                ? "## Meta et numerus\r\n" +
                  "Responsum lectionis: quid ad metam accidit et numerus simplex exercitationis.\r\n\r\n" +
                  lo.TrimEnd() + "\r\n" +
                  (lessonScore is { } sc ? $"\r\nNumerus: {sc}\r\n" : "\r\n")
                : "";

        var worldSection =
            "\r\n## Status orbis (post ultimum cursum)\r\n" +
            "Locus robotis, directio, et metadata chartae ex imagine orbis.\r\n" +
            "\r\n" +
            (string.IsNullOrWhiteSpace(worldAfterRunSummary)
                ? "(nullum summarium in imagine)\r\n"
                : worldAfterRunSummary.TrimEnd() + "\r\n");

        var outcomeSection =
            "\r\n" +
            "## Eventus interpretis\r\n" +
            "Res structa ab interprete (non exceptio .NET — RoboSharp culpas ut data nuntiat).\r\n" +
            "\r\n" +
            (runtimeSucceeded == true
                ? "Perfectum sine culpa.\r\n"
                : "Substitit cum culpa — lege nuntium infra et Diagnostica pro rebus compilationis.\r\n") +
            (string.IsNullOrWhiteSpace(runtimeFaultMessage)
                ? ""
                : "\r\n" + runtimeFaultMessage.TrimEnd() + "\r\n");

        var stdoutSection =
            "\r\n" +
            "## Exitus standardus (print)\r\n" +
            "Omne quod programma print() scripsit.\r\n" +
            "\r\n" +
            (string.IsNullOrWhiteSpace(runtimeStdout)
                ? "(nullus exitus)\r\n"
                : runtimeStdout.TrimEnd() + "\r\n");

        var stderrSection =
            "\r\n" +
            "## Error standardus\r\n" +
            "Canalis exsecutorius pro culpis et finibus — non idem canalis ac print().\r\n" +
            "\r\n" +
            (string.IsNullOrWhiteSpace(runtimeStderr)
                ? "(nihil)\r\n"
                : runtimeStderr.TrimEnd() + "\r\n");

        return doc + goalSection + worldSection + outcomeSection + stdoutSection + stderrSection;
    }
}
