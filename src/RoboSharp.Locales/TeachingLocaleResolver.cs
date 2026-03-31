namespace RoboSharp.Locales;

/// <summary>
/// Picks a concrete <see cref="ITeachingLocale"/> for hosts. Studio reads <see cref="EnvironmentVariableName"/>.
/// </summary>
public static class TeachingLocaleResolver
{
    public const string EnvironmentVariableName = "ROBOSHARP_LOCALE";

    /// <summary>
    /// Returns <see cref="LatinTeachingLocale"/> for <c>la</c> or <c>latin</c> (case-insensitive); otherwise English.
    /// </summary>
    public static ITeachingLocale Create(string? localeId)
    {
        if (string.IsNullOrWhiteSpace(localeId))
            return new EnglishTeachingLocale();

        if (localeId.Equals("la", StringComparison.OrdinalIgnoreCase) ||
            localeId.Equals("latin", StringComparison.OrdinalIgnoreCase))
            return new LatinTeachingLocale();

        return new EnglishTeachingLocale();
    }

    /// <summary>Uses <see cref="Environment.GetEnvironmentVariable"/> with <see cref="EnvironmentVariableName"/>.</summary>
    public static ITeachingLocale FromEnvironment() =>
        Create(Environment.GetEnvironmentVariable(EnvironmentVariableName));
}
