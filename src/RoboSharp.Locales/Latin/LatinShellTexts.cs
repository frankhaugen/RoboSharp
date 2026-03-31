namespace RoboSharp.Locales.Latin;

internal sealed class LatinShellTexts : IStudioShellTexts
{
    public string WindowTitleSuffix => "— RoboSharp Studium";

    public string FormatWindowTitle(string fileDisplayName, bool dirty) =>
        $"{fileDisplayName}{(dirty ? " *" : "")} {WindowTitleSuffix}";

    public string UntitledFileName => "Sine nomine.robo";
    public string FileMenuHeader => "_Tabula";
    public string MenuNew => "_Nova";
    public string MenuOpen => "_Aperi…";
    public string MenuSave => "_Serva";
    public string MenuSaveAs => "Serva _aliter…";
    public string MenuExit => "E_xi";
    public string HelpMenuHeader => "_Auxilium";
    public string MenuAbout => "De _hoc…";
    public string RoboFileTypeDescription => "Fons RoboSharp (.robo)";
    public string OpenFilePickerTitle => "Aperi fontem RoboSharp";
    public string SaveFilePickerTitle => "Serva fontem RoboSharp";
    public string DialogOk => "Ita";
    public string SaveFailedTitle => "Servare non potuit";
    public string OpenFailedTitle => "Aperire non potuit";
    public string SaveNoLocalPathMessage =>
        "Iter locale ad filem reperire non potui. Serva in tabula huius machinae.";
    public string UnsavedDialogTitle => "RoboSharp Studium";
    public string UnsavedDialogHeading => "Mutata servare vis?";
    public string UnsavedDialogBody =>
        "Textus programmatis mutatus est nec servatus. Servare scribit .robo in discum. " +
        "Noli servare manet in memoria tantum (perire potest). Revocare redit ad editorum.";
    public string ButtonSave => "Serva";
    public string ButtonDontSave => "Noli servare";
    public string ButtonCancel => "Revoca";
    public string AboutTitle => "De hoc";
    public string AboutAppName => "RoboSharp Studium";
    public string AboutBody =>
        "IDE docendi pro RoboSharp: lexicum → syntacticus → nexus → IL ficta → interpres in reticulo robotis. " +
        "Omnis gradus in tabulis patentibus cernitur. Vide docs/studio/ in repositorio.";
    public string ToolbarBuild => "Aedifica";
    public string ToolbarRun => "▶  Curr";
    public string ToolbarStepSpeed => "Celeritas graduum";
    public string ToolbarAppTitle => "RoboSharp Studium";
    public string ToolbarSubtitle =>
        "Lectionem et metam sinistra elige. Aedifica = compila et renova tabulas. Curr = compila dein robotem in meta gradatim age.";
    public string RunSpeedRealtime =>
        "Statim — ad finem sine mora inter singulas instructiones IL";
    public string RunSpeedSlow =>
        "Lente — brevis mora singulis gradibus IL ut robotem sequaris";
    public string RunSpeedGlacial =>
        "Glacialis — longior mora; optime cum classem demonstras";
    public string RunSpeedRealtimeShort => "Statim";
    public string RunSpeedSlowShort => "Lente";
    public string RunSpeedGlacialShort => "Glacialis";
    public string DefaultLiveRunStatus =>
        "Lectionem et metam sinistra elige, dein Aedifica vel Curr.";
    public string LiveRunInProgress => "Currit… compilat et interpretem gradatim agit.";
    public string LiveRunFinished => "Cursus finitus.";
    public string LiveRunFaultFallback => "Cursus substitit.";
    public string FormatLiveRunProgress(long steps, string? instructionHint) =>
        $"{steps} gradus IL · {instructionHint ?? "…"}";

    public string FormatLessonOutcomeLine(string story, int? score) =>
        score is { } sc ? $"{story}  →  Numerus: {sc}" : story;

    public string InterpreterUnexpectedStop =>
        "Interpres sine fine ordinario aut culpa structa substitit. Hoc non debet fieri — nota quid cucurristi et magistrum adi aut indicium mitte.";

    public string SettingsMenuHeader => "_Optiones";

    public string LanguageMenuHeader => "_Lingua";

    public string LanguageEnglishMenuLabel => "English (en)";

    public string LanguageLatinMenuLabel => "Latina — demonstratio (la)";
}

