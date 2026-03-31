namespace RoboSharp.Locales.Latin;

internal sealed class LatinSidebarTexts : IStudioSidebarTexts
{
    public string StartHereHeading => "Quid discis";
    public string LessonRibbonSubtitle =>
        "Lectionem in fasciā elige — charta exercitationis, regulae compilatōris, et tabulae inspectoris lectionem sequuntur.";
    public string LessonSectionGoalHeading => "Charta exercitationis (haec lectio)";
    public string LessonWorldNameLabel => "Area";
    public string LessonSectionCommandsHeading => "Quid scribere licet (haec lectio)";
    public string LessonProfileNameLabel => "Profilium compilatōris";
    public string LessonSectionReferenceHeading => "Referentia brevis";
    public string KeywordsHeading => "Verba clavis et imperata huius lectionis";
    public string SyntaxHeading => "Monita syntactica huius lectionis";
    public string LoadLessonExampleButton => "Exemplum huius lectionis in editorum pone";
    public string WorldPreviewHeading => "Orbis praevius";
    public string WorldPreviewHint =>
        "Signa: tegulae obscurae = murus, caeruleo-griseae = solum, viridi-cyanidae = meta. Sagittae ostendunt quonam robot spectet.\n\n" +
        "Aedifica compilatorem tantum movet et tabulas ductus renovat. " +
        "Curr iterum compila, dein in huius lectionis agro ad celeritatem electam gradatim agit (Statim / Lente / Glacialis).\n\n" +
        LatinTeachingExplainer.ProfilesVsGrammar;
}
