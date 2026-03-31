namespace RoboSharp.Locales.English;

internal sealed class EnglishShellTexts : IStudioShellTexts
{
    public string WindowTitleSuffix => "— RoboSharp Studio";

    public string FormatWindowTitle(string fileDisplayName, bool dirty) =>
        $"{fileDisplayName}{(dirty ? " *" : "")} {WindowTitleSuffix}";

    public string UntitledFileName => "Untitled.robo";
    public string FileMenuHeader => "_File";
    public string MenuNew => "_New";
    public string MenuOpen => "_Open…";
    public string MenuSave => "_Save";
    public string MenuSaveAs => "Save _As…";
    public string MenuExit => "E_xit";
    public string HelpMenuHeader => "_Help";
    public string MenuAbout => "_About…";
    public string RoboFileTypeDescription => "RoboSharp source (.robo)";
    public string OpenFilePickerTitle => "Open RoboSharp source";
    public string SaveFilePickerTitle => "Save RoboSharp source";
    public string DialogOk => "OK";
    public string SaveFailedTitle => "Save failed";
    public string OpenFailedTitle => "Open failed";
    public string SaveNoLocalPathMessage =>
        "Could not resolve a local file path. Try saving to a folder on this computer.";
    public string UnsavedDialogTitle => "RoboSharp Studio";
    public string UnsavedDialogHeading => "Save changes to the current document?";
    public string UnsavedDialogBody =>
        "Your program text has unsaved edits. Save writes the .robo file to disk. " +
        "Don't save keeps editing in memory only (changes can be lost). Cancel returns to the editor.";
    public string ButtonSave => "Save";
    public string ButtonDontSave => "Don't save";
    public string ButtonCancel => "Cancel";
    public string AboutTitle => "About";
    public string AboutAppName => "RoboSharp Studio";
    public string AboutBody =>
        "Teaching IDE for RoboSharp: lexer → parser → binder → fake IL → interpreter on a Karel grid. " +
        "Every stage is visible in the pipeline panels. See docs/studio/ in the repository for the full specification.";
    public string ToolbarBuild => "Build";
    public string ToolbarRun => "▶  Run";
    public string ToolbarStepSpeed => "Step speed";
    public string ToolbarAppTitle => "RoboSharp Studio";
    public string ToolbarSubtitle =>
        "Karel map (left) · Build = compile and refresh teaching panels · Run = compile again, then step the interpreter (pick a speed)";
    public string RunSpeedRealtime =>
        "Realtime — run to the end without pausing between IL instructions";
    public string RunSpeedSlow =>
        "Slow — short pause each IL step so you can follow the robot";
    public string RunSpeedGlacial =>
        "Glacial — longer pause; best when demonstrating in front of a class";
    public string DefaultLiveRunStatus =>
        "Pick a lesson profile and world map below, then Build (compile) or Run (compile + execute on the grid).";
    public string LiveRunInProgress => "Running… compiling and stepping the interpreter.";
    public string LiveRunFinished => "Finished run.";
    public string LiveRunFaultFallback => "Run stopped.";
    public string FormatLiveRunProgress(long steps, string? instructionHint) =>
        $"{steps} IL steps · {instructionHint ?? "…"}";

    public string FormatLessonOutcomeLine(string story, int? score) =>
        score is { } sc ? $"{story}  →  Score: {sc}" : story;

    public string InterpreterUnexpectedStop =>
        "The interpreter stopped without a normal completion or a structured fault. That should not happen — note what you ran and tell a teacher or file an issue.";

    public string SettingsMenuHeader => "_Settings";

    public string LanguageMenuHeader => "_Language";

    public string LanguageEnglishMenuLabel => "English (en)";

    public string LanguageLatinMenuLabel => "Latin — demo (la)";
}
