namespace RoboSharp.Locales.Latin;

internal sealed class LatinSidebarTexts : IStudioSidebarTexts
{
    public string StartHereHeading => "Hinc incipe";
    public string LessonPickerCaption => "Lectio (ubi in cursu es)";
    public string GoalCaption => "Meta (charta / provocatio — seorsum elige)";
    public string CommandsCaption => "Imperata licita (profilium)";
    public string KeywordsHeading => "Verba clavis et imperata huius lectionis";
    public string SyntaxHeading => "Monita syntactica huius lectionis";
    public string LoadLessonExampleButton => "Exemplum huius lectionis in editorum pone";
    public string WorldPreviewHeading => "Orbis praevius";
    public string WorldPreviewHint =>
        "Signa: tegulae obscurae = murus, caeruleo-griseae = solum, viridi-cyanidae = meta. Sagittae ostendunt quonam robot spectet.\n\n" +
        "Aedifica compilatorem tantum movet et signa, arborem, diagnostica, arborem nexam, et IL renovat. " +
        "Curr iterum compila, dein in meta quam elegisti ad celeritatem quam elegisti exsequitur (Statim / Lente / Glacialis).\n\n" +
        LatinTeachingExplainer.ProfilesVsGrammar;
}
