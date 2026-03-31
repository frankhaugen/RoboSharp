namespace RoboSharp.Locales.Latin;

internal sealed class LatinPanelTexts : IStudioPanelTexts
{
    public string LessonToolboxTitle => "Arca instrumentorum lectionis";
    public string LessonToolboxSubtitle =>
        "Nomina quae compilator hac lectione accipit — cum profilio lectionis cohaerent, non cum omni verbo RoboSharp umquam discet.";
    public string LessonToolboxLead => "Quae vocare licet";
    public string LessonToolboxGuide =>
        "Haec index est **profilium**: builtina et typi quibus programma tuum nunc uti potest. " +
        LatinTeachingExplainer.ProfilesVsGrammar +
        " Post **Aedifica**, lineae infra sunt index vivus ex illo profilio.";
    public string LessonToolboxFooter =>
        "Si nomen quod exspectas deest, fortasse lectionem antecessisti — muta lectionem in fasciā aut postea profilius latiore utere.";
    public string LessonToolboxBuildPrompt =>
        "(Semel aedifica ut profilium activum in hanc imaginem trahas.)";

    public string TokensTitle => "Signa";
    public string TokensSubtitle =>
        "Exitus lexici: quomodo textus planus in fluxum partium signatarum ante syntacticum agens factus sit.";
    public string TokensLead => "Analysis lexicalis";
    public string TokensGuide =>
        "**Lexicum** textum a sinistra ad dexteram legit et **signa** facit — verba clavis, nomina, numeri, interpunctio, commentaria. " +
        "Quaelibet linea infra unum signum: genus, ubi incipit, longitudo, textus ipsus (\\r, \\n, \\t effugata).\r\n\r\n" +
        LatinTeachingExplainer.LexerToParser;
    public string TokensColumnHeader => "genus                     @init  long  textus";
    public string TokensFootnote =>
        "Signa sunt imago ab ultima **Aedificatione**. Muta editorum, dein Aedifica iterum. Signa mira saepe significant litteram erraticam quam lexicum «malum» notavit.";

    public string SyntaxTreeTitle => "Arbor syntactica";
    public string SyntaxTreeSubtitle =>
        "Quomodo syntacticus signa in formas grammaticas — proceduras, cuneos, expressiones — coegerit.";
    public string SyntaxTreeLead => "Parsing";
    public string SyntaxTreeGuide =>
        "**Syntacticus** flumen signorum legit et **arborem syntacticam** aedificat: nodi inclusi qui grammaticae RoboSharp congruunt. " +
        "Indentatio in effusione structuram parentis/filii ostendit. Arbor munda hic saepe significat «syntaxis bona est; si quid adhuc male est, specta nexum aut typōs», non lexicum.\r\n\r\n" +
        LatinTeachingExplainer.PhasesDependOnEachOther;
    public string SyntaxTreeFootnote =>
        "Arbor textus tantum lectu est quem copiare potes. Eadem structura quam gradus nexūs deinde perambulat.";

    public string DiagnosticsTitle => "Nuntii compilatōris";
    public string DiagnosticsSubtitle =>
        "Lineae syntacticae, significativae, exsecutoriae ab ultima Aedificatione vel Cursu — quaelibet gradu signata.";
    public string DiagnosticsPreamble =>
        "# Nuntii compilatōris\r\n" +
        LatinTeachingExplainer.WhatIsASourceSpan +
        "\r\n\r\n" +
        "• **lexica** — lexicum/syntacticus arborem validam facere non potuit.\r\n" +
        "• **significativa** — regulae nexūs/types post syntaxin felicem defecerunt. " + LatinTeachingExplainer.WhatIsBinding + "\r\n" +
        "• **exsecutoria** — interpres culpam post IL demissam nuntiavit. " + LatinTeachingExplainer.StdoutVsDiagnostics + "\r\n" +
        "\r\n";

    public string DiagnosticsNone =>
        "(Nulla nuntiata in his gradibus — ultima Aedificatio/Cursus nullas culpas hic nuntiavit.)";

    public string DiagnosticsRuntimePrefix => "exsecutorius ";

    public string DiagnosticsLead => "Signa compilatōris et cursūs";
    public string DiagnosticsGuide =>
        "Lineae syntacticae, significativae, exsecutoriae ab ultima Aedificatione vel Cursu — quaelibet gradu signata.";
    public string DiagnosticsFooter =>
        "Monitum: **Visum → Nuntii compilatōris…** hanc indicem in fenestra propria aperit.";

    public string BoundTreeTitle => "Arbor nexa";
    public string BoundTreeSubtitle =>
        "Analysis significativa: nomina ad symbola, expressiones ad typōs — intratura demissionis.";
    public string BoundTreeLead => "Nexus et typī";
    public string BoundTreeGuide =>
        "**Nexus** nomina ad declarationes resolvit, typōs probat, significationem arbori adnectit. " +
        LatinTeachingExplainer.WhatIsBinding +
        "\r\n\r\nQuod infra vidēs est programma **nexum**: stratum quod gradus demissionis IL consumit. Cum hic textus apparet, compilator sensum satis intellexit ut conetur instructiones fingere.";
    public string BoundTreeFootnote =>
        "Si haec tabula vacua est aut brevem notam monstrat, corrige quod dicit, dein Aedifica. **Visum → Nuntii compilatōris** pro indice pleno.";
    public string BoundTreeNeedParseFirst =>
        "Nexus post syntaxin felicem tantum currit. Primum nuntiata syntactica corrige (arbor + Nuntii compilatōris), dein Aedifica.";
    public string BoundTreeSemanticsStopped =>
        "Analysis significativa cito substitit — aperi **Visum → Nuntii compilatōris** pro nuntiis nexūs (nomina ignota, typī falsi, fines profilii).";
    public string BoundTreeUnexpectedEmpty =>
        "(Textus arboris nexae deest quamquam demissio successum nuntiavit — inopinatum; Nuntii compilatōris vide et indicium mitte si manet.)";
    public string BoundTreeBuildPrompt => "Aedifica ut ductum renoves et hanc tabulam impleas cum compilatio felix est.";

    public string IlTitle => "IL (demissa)";
    public string IlSubtitle =>
        "Index fictarum instructionum quas interpres gradatim agit — non IL CLR; vide ductum et notam post Curr.";
    public string IlLead => "Demissio";
    public string IlGuide =>
        LatinTeachingExplainer.FakeIlVersusDotNet +
        "\r\n\r\nQuaelibet instructio unus gradus est quem interpres docens exsequitur. Dum **Curr** gradatim agit, linea praesens illustratur ut motum IL cum roboto coniungas.";

    public string IlWaitingForLowering =>
        "IL post nexum et demissionem felicem tantum apparet. Nuntiata significativa clara fac, dein Aedifica.";
    public string IlNoTextUnexpected =>
        "(Demissio successum nuntiavit sed textus IL deest — inopinatum. Aedifica iterum et Nuntii compilatōris vide.)";

    public string IlCopyDisassembly => "Copia textum IL";

    public string SharpAssemblyTitle => "SharpAssembly (docens)";
    public string SharpAssemblySubtitle =>
        "IL in formam mnemonum quasi-assembleriam — idem gradūs, syntaxis similior «verae» asm (non est CPU).";
    public string SharpAssemblyLead => "Sub IL in scala";
    public string SharpAssemblyGuide =>
        "**SharpAssembly** est lectio eadem IL RoboSharp quam interpres agit. Non est IL CLR nec x86/ARM. " +
        "Adhibe ad operandōs et fluxum postquam tabulam IL intellegis.";
    public string SharpAssemblyFooter =>
        "Confer lineatim cum **IL (demissa)** — numerī et scopī saltūs congruere debent.";
    public string SharpAssemblyWaitingForProgram =>
        "(Programma demissum nondum est — corrige **Nuntii compilatōris**, dein Aedifica.)";

    public string FakeMachineCodeTitle => "Verba machinae (docentia)";
    public string FakeMachineCodeSubtitle =>
        "Hexadecimales 32-bit fictae per instructionem — quomodo «binae in filo» videri possint sine ISA vera.";
    public string FakeMachineCodeLead => "Codificatio (non machina vera)";
    public string FakeMachineCodeGuide =>
        "Quod **verbum** est codificatio ficta deterministica opcodis + operandōrum pro hac VM docente. " +
        "CPU reales aliter disponunt; hoc tantum ad intuitionem hex.";
    public string FakeMachineCodeFooter =>
        "Columnae dexterae IL iterant ut nexum probes; hex tantum scholae causa.";
    public string FakeMachineCodeWaitingForProgram =>
        "(Programma demissum nondum est — Aedifica post compilationem felicem.)";

    public string WorldRuntimeTitle => "Relatio cursūs";
    public string WorldRuntimeSubtitle =>
        "Post Curr: responsum meta, summarium orbis, print(), stderr, eventus — ex menu Visum apertum.";
    public string WorldRuntimeLead => "Imago cursūs";
    public string WorldRuntimeFooter =>
        "Etiam **Visum → Relatio cursūs…** in fenestra propria aperire potes.";
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
            "# Relatio cursūs\r\n" +
            "Aedificatio tantum compila. Curr iterum compila, dein IL in reticulo gradatim agit.\r\n" +
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
                "Monitum: voca imperata ad summum scopum (exempli gratia move();) et nuntiata syntactica/significativa clara fac. " +
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
                : "Substitit cum culpa — lege nuntium infra et **Visum → Nuntii compilatōris** pro rebus compilationis.\r\n") +
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
