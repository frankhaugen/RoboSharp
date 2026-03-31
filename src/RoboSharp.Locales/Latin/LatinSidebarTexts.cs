namespace RoboSharp.Locales.Latin;

internal sealed class LatinSidebarTexts : IStudioSidebarTexts
{
    public string LessonAndMapHeading => "Lectio et charta";
    public string ProfileCaption => "Profilium (quae imperata in hac lectione adsunt)";
    public string WorldCaption => "Orbis (magnitudo, muri, meta)";
    public string KarelWorldHeading => "Orbis Karelianus";
    public string KarelWorldHint =>
        "Signa: tegulae obscurae = murus, caeruleo-griseae = solum, viridi-cyanidae = meta. Sagittae ostendunt quonam robot spectet.\n\n" +
        "Aedifica compilatorem tantum movet et signa, arborem, diagnostica, arborem nexam, et IL renovat. " +
        "Curr iterum compila, dein in hac charta ad celeritatem quam elegisti exsequitur (Statim / Lente / Glacialis).\n\n" +
        LatinTeachingExplainer.ProfilesVsGrammar;
}
