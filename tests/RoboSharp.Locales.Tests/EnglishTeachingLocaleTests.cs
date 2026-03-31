using RoboSharp.Locales;
using RoboSharp.Locales.English;

namespace RoboSharp.Locales.Tests;

public class EnglishTeachingLocaleTests
{
    [Test]
    public async Task LocaleId_is_en()
    {
        var locale = new EnglishTeachingLocale();
        await Assert.That(locale.LocaleId).IsEqualTo("en");
    }

    [Test]
    public async Task Shell_strings_are_non_empty_and_window_title_includes_file_name()
    {
        var locale = new EnglishTeachingLocale();
        var title = locale.Shell.FormatWindowTitle("Hello.robo", dirty: true);
        await Assert.That(title).Contains("Hello.robo");
        await Assert.That(title).Contains('*');
        await Assert.That(locale.Shell.DefaultLiveRunStatus.Length).IsGreaterThan(20);
    }

    [Test]
    public async Task Pipeline_formats_parse_and_semantic_lines_consistently()
    {
        var locale = new EnglishTeachingLocale();
        var p = locale.Pipeline.FormatParseDiagnosticLine(1, 2, "line 1", "oops");
        var s = locale.Pipeline.FormatSemanticDiagnosticLine(3, 4, "line 2", "bad");
        await Assert.That(p).StartsWith("parse");
        await Assert.That(s).StartsWith("semantic");
    }

    [Test]
    public async Task EnglishTeachingExplainer_paragraphs_have_substance()
    {
        await Assert.That(EnglishTeachingExplainer.LexerToParser.Length).IsGreaterThan(80);
        await Assert.That(EnglishTeachingExplainer.FakeIlVersusDotNet).Contains(".NET", StringComparison.Ordinal);
    }
}
