using System.Reflection;

namespace RoboSharp.Locales;

/// <summary>
/// Reflection-based checks that every <see cref="ITeachingLocale"/> surface returns non-empty user-facing strings.
/// Used by tests so new interface members cannot ship with missing copy.
/// </summary>
public static class TeachingLocaleStringGuard
{
    /// <summary>Human-readable defect descriptions (empty when locale is complete).</summary>
    public static IReadOnlyList<string> CollectIssues(ITeachingLocale locale)
    {
        var issues = new List<string>();
        if (string.IsNullOrWhiteSpace(locale.LocaleId))
            issues.Add($"{nameof(ITeachingLocale.LocaleId)} is null or whitespace.");

        VerifyPart(locale.Shell, typeof(IStudioShellTexts), nameof(ITeachingLocale.Shell), issues);
        VerifyPart(locale.Sidebar, typeof(IStudioSidebarTexts), nameof(ITeachingLocale.Sidebar), issues);
        VerifyPart(locale.Panels, typeof(IStudioPanelTexts), nameof(ITeachingLocale.Panels), issues);
        VerifyPart(locale.Pipeline, typeof(IPipelineTeachingTexts), nameof(ITeachingLocale.Pipeline), issues);
        VerifyLessons(locale.Lessons, nameof(ITeachingLocale.Lessons), issues);
        return issues;
    }

    /// <summary>Throws <see cref="InvalidOperationException"/> if <see cref="CollectIssues"/> is non-empty.</summary>
    public static void ThrowIfIncomplete(ITeachingLocale locale)
    {
        var issues = CollectIssues(locale);
        if (issues.Count > 0)
            throw new InvalidOperationException(string.Join(Environment.NewLine, issues));
    }

    static void VerifyLessons(IStudioLessonCatalog catalog, string path, List<string> issues)
    {
        if (catalog.OrderedLessons.Count == 0)
        {
            issues.Add($"{path}.{nameof(IStudioLessonCatalog.OrderedLessons)} is empty.");
            return;
        }

        foreach (var L in catalog.OrderedLessons)
        {
            if (string.IsNullOrWhiteSpace(L.Id))
                issues.Add($"{path}: lesson with empty {nameof(StudioLessonDefinition.Id)}.");
            if (string.IsNullOrWhiteSpace(L.Title))
                issues.Add($"{path}: lesson '{L.Id}' has empty {nameof(StudioLessonDefinition.Title)}.");
            if (string.IsNullOrWhiteSpace(L.StartHereBlurb))
                issues.Add($"{path}: lesson '{L.Id}' has empty {nameof(StudioLessonDefinition.StartHereBlurb)}.");
            if (string.IsNullOrWhiteSpace(L.KeywordsSection))
                issues.Add($"{path}: lesson '{L.Id}' has empty {nameof(StudioLessonDefinition.KeywordsSection)}.");
            if (string.IsNullOrWhiteSpace(L.SyntaxSection))
                issues.Add($"{path}: lesson '{L.Id}' has empty {nameof(StudioLessonDefinition.SyntaxSection)}.");
            if (string.IsNullOrWhiteSpace(L.ExampleSource))
                issues.Add($"{path}: lesson '{L.Id}' has empty {nameof(StudioLessonDefinition.ExampleSource)}.");
            if (string.IsNullOrWhiteSpace(L.DefaultProfileId))
                issues.Add($"{path}: lesson '{L.Id}' has empty {nameof(StudioLessonDefinition.DefaultProfileId)}.");
            if (string.IsNullOrWhiteSpace(L.DefaultWorldPresetId))
                issues.Add($"{path}: lesson '{L.Id}' has empty {nameof(StudioLessonDefinition.DefaultWorldPresetId)}.");
            if (string.IsNullOrWhiteSpace(L.GoalSectionBody))
                issues.Add($"{path}: lesson '{L.Id}' has empty {nameof(StudioLessonDefinition.GoalSectionBody)}.");
            if (string.IsNullOrWhiteSpace(L.CommandsSectionBody))
                issues.Add($"{path}: lesson '{L.Id}' has empty {nameof(StudioLessonDefinition.CommandsSectionBody)}.");
            if (L.VisiblePanelIds is null || L.VisiblePanelIds.Count == 0)
            {
                issues.Add($"{path}: lesson '{L.Id}' has empty {nameof(StudioLessonDefinition.VisiblePanelIds)}.");
                continue;
            }

            foreach (var pid in L.VisiblePanelIds)
            {
                if (string.IsNullOrWhiteSpace(pid))
                    issues.Add($"{path}: lesson '{L.Id}' has a null/whitespace entry in {nameof(StudioLessonDefinition.VisiblePanelIds)}.");
                else if (!StudioPanelIds.All.Contains(pid))
                    issues.Add($"{path}: lesson '{L.Id}' references unknown panel id '{pid}' in {nameof(StudioLessonDefinition.VisiblePanelIds)}.");
            }
        }
    }

    static void VerifyPart(object target, Type iface, string path, List<string> issues)
    {
        foreach (var prop in iface.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.PropertyType != typeof(string))
                continue;
            var value = prop.GetValue(target) as string;
            if (string.IsNullOrWhiteSpace(value))
                issues.Add($"{path}.{prop.Name} is null or whitespace.");
        }

        foreach (var method in iface.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            if (method.IsSpecialName)
                continue;
            if (method.ReturnType != typeof(string))
                continue;

            if (string.Equals(method.Name, nameof(IStudioPanelTexts.FormatWorldRuntimePanel), StringComparison.Ordinal))
            {
                VerifyWorldRuntimePanel(target, method, path, issues);
                continue;
            }

            try
            {
                var args = method.GetParameters().Select(BuildDefaultArgument).ToArray();
                var result = method.Invoke(target, args) as string;
                if (string.IsNullOrWhiteSpace(result))
                    issues.Add($"{path}.{method.Name}(…) returned null or whitespace.");
            }
            catch (Exception ex)
            {
                issues.Add($"{path}.{method.Name}(…) threw: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }

    static void VerifyWorldRuntimePanel(object target, MethodInfo method, string path, List<string> issues)
    {
        var scenarios = new (bool? ok, bool il, string? lo, int? sc, string? sum, string? fault, string? @out, string? err)[]
        {
            (null, true, null, null, null, null, null, null),
            (null, false, null, null, null, null, null, null),
            (true, true, "outcome", 7, "world", null, "stdout", "stderr"),
            (false, true, null, null, null, "fault", null, null),
            (true, true, null, null, "", null, "", ""),
        };

        var i = 0;
        foreach (var s in scenarios)
        {
            i++;
            try
            {
                var result = method.Invoke(target, new object?[]
                {
                    s.ok,
                    s.il,
                    s.lo,
                    s.sc,
                    s.sum,
                    s.fault,
                    s.@out,
                    s.err,
                }) as string;
                if (string.IsNullOrWhiteSpace(result))
                    issues.Add($"{path}.{method.Name} scenario {i} returned null or whitespace.");
            }
            catch (Exception ex)
            {
                issues.Add($"{path}.{method.Name} scenario {i} threw: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }

    static object? BuildDefaultArgument(ParameterInfo p)
    {
        var t = p.ParameterType;
        var underlying = Nullable.GetUnderlyingType(t);
        if (underlying is not null)
        {
            if (underlying == typeof(bool))
                return null;
            if (underlying == typeof(int))
                return null;
            if (underlying == typeof(long))
                return null;
            t = underlying;
        }

        if (t == typeof(string))
            return "sample";
        if (t == typeof(bool))
            return false;
        if (t == typeof(int))
            return 0;
        if (t == typeof(long))
            return 0L;

        throw new NotSupportedException($"TeachingLocaleStringGuard: unsupported parameter type {p.ParameterType} ({p.Name}).");
    }
}
