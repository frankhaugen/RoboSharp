using RoboSharp.Locales;

namespace RoboSharp.Locales.Tests;

public class LatinTeachingLocaleTests
{
    [Test]
    public async Task LocaleId_is_la()
    {
        var locale = new LatinTeachingLocale();
        await Assert.That(locale.LocaleId).IsEqualTo("la");
    }

    [Test]
    public async Task Shell_uses_Latin_flavor_in_title_and_toolbar()
    {
        var locale = new LatinTeachingLocale();
        await Assert.That(locale.Shell.ToolbarBuild).IsEqualTo("Aedifica");
        await Assert.That(locale.Shell.WindowTitleSuffix).Contains("Studium", StringComparison.Ordinal);
        var title = locale.Shell.FormatWindowTitle("Salve.robo", dirty: false);
        await Assert.That(title).Contains("Salve.robo");
    }

    [Test]
    public async Task Pipeline_uses_Latin_diagnostic_labels()
    {
        var locale = new LatinTeachingLocale();
        var p = locale.Pipeline.FormatParseDiagnosticLine(1, 2, "linea 1", "heu");
        var s = locale.Pipeline.FormatSemanticDiagnosticLine(3, 4, "linea 2", "malum");
        await Assert.That(p).StartsWith("lexica");
        await Assert.That(s).StartsWith("significativa");
    }

    [Test]
    public async Task TeachingLocaleResolver_Selects_Latin_for_la_and_latin()
    {
        await Assert.That(TeachingLocaleResolver.Create("la")).IsAssignableTo(typeof(LatinTeachingLocale));
        await Assert.That(TeachingLocaleResolver.Create("LA")).IsAssignableTo(typeof(LatinTeachingLocale));
        await Assert.That(TeachingLocaleResolver.Create("latin")).IsAssignableTo(typeof(LatinTeachingLocale));
    }

    [Test]
    public async Task TeachingLocaleResolver_Defaults_to_English_for_unknown_or_empty()
    {
        await Assert.That(TeachingLocaleResolver.Create(null)).IsAssignableTo(typeof(EnglishTeachingLocale));
        await Assert.That(TeachingLocaleResolver.Create("")).IsAssignableTo(typeof(EnglishTeachingLocale));
        await Assert.That(TeachingLocaleResolver.Create("fr")).IsAssignableTo(typeof(EnglishTeachingLocale));
    }

    [Test]
    public async Task LatinTeachingExplainer_has_substance()
    {
        await Assert.That(LatinTeachingExplainer.LexerToParser.Length).IsGreaterThan(80);
        await Assert.That(LatinTeachingExplainer.FakeIlVersusDotNet).Contains("IL", StringComparison.Ordinal);
    }
}
