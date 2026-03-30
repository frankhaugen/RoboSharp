using Microsoft.Extensions.DependencyInjection;

namespace RoboSharp.Studio.Composition;

/// <summary>
/// Holds the root <see cref="IServiceProvider"/> for the Studio process.
/// Set exactly once from <c>Program.cs</c> after building the container — not a general service locator for random callsites.
/// </summary>
public static class CompositionRoot
{
    private static IServiceProvider? _provider;

    public static void Initialize(IServiceProvider provider) =>
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    public static IServiceProvider Services =>
        _provider ?? throw new InvalidOperationException("CompositionRoot not initialized. Call Initialize from Program first.");

    public static T GetRequiredService<T>() where T : notnull =>
        (T)Services.GetRequiredService(typeof(T));
}
