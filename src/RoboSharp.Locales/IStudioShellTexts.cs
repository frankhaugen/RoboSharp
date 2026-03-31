namespace RoboSharp.Locales;

public interface IStudioShellTexts
{
    string WindowTitleSuffix { get; }
    string FormatWindowTitle(string fileDisplayName, bool dirty);
    string UntitledFileName { get; }
    string FileMenuHeader { get; }
    string MenuNew { get; }
    string MenuOpen { get; }
    string MenuSave { get; }
    string MenuSaveAs { get; }
    string MenuExit { get; }
    string HelpMenuHeader { get; }
    string MenuAbout { get; }
    string RoboFileTypeDescription { get; }
    string OpenFilePickerTitle { get; }
    string SaveFilePickerTitle { get; }
    string DialogOk { get; }
    string SaveFailedTitle { get; }
    string OpenFailedTitle { get; }
    string SaveNoLocalPathMessage { get; }
    string UnsavedDialogTitle { get; }
    string UnsavedDialogHeading { get; }
    string UnsavedDialogBody { get; }
    string ButtonSave { get; }
    string ButtonDontSave { get; }
    string ButtonCancel { get; }
    string AboutTitle { get; }
    string AboutAppName { get; }
    string AboutBody { get; }
    string ToolbarBuild { get; }
    string ToolbarRun { get; }
    string ToolbarStepSpeed { get; }
    string ToolbarAppTitle { get; }
    string ToolbarSubtitle { get; }
    string RunSpeedRealtime { get; }
    string RunSpeedSlow { get; }
    string RunSpeedGlacial { get; }
    string DefaultLiveRunStatus { get; }
    string LiveRunInProgress { get; }
    string LiveRunFinished { get; }
    string LiveRunFaultFallback { get; }
    string FormatLiveRunProgress(long steps, string? instructionHint);
    string FormatLessonOutcomeLine(string story, int? score);
    string InterpreterUnexpectedStop { get; }

    /// <summary>Top-level menu for Studio preferences (mnemonic underscore where used).</summary>
    string SettingsMenuHeader { get; }

    /// <summary>Submenu grouping display language choices.</summary>
    string LanguageMenuHeader { get; }

    /// <summary>Menu item label for switching UI to English.</summary>
    string LanguageEnglishMenuLabel { get; }

    /// <summary>Menu item label for the Latin demo pack.</summary>
    string LanguageLatinMenuLabel { get; }
}