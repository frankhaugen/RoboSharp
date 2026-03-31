using RoboSharp.Locales;
using RoboSharp.Locales.English;
using RoboSharp.Locales.Latin;

namespace RoboSharp.Locales.Tests;

public class TeachingLocaleStringGuardTests
{
    [Test]
    public async Task English_teaching_locale_has_no_missing_or_blank_strings()
    {
        var issues = TeachingLocaleStringGuard.CollectIssues(new EnglishTeachingLocale());
        await Assert.That(issues).IsEmpty();
    }

    [Test]
    public async Task Latin_teaching_locale_has_no_missing_or_blank_strings()
    {
        var issues = TeachingLocaleStringGuard.CollectIssues(new LatinTeachingLocale());
        await Assert.That(issues).IsEmpty();
    }
}
