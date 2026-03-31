namespace RoboSharp.Locales;

/// <summary>Stable ids for pipeline inspector panels (<c>RoboSharp.Studio</c> <see cref="StudioPanelIds"/> mirror).</summary>
public static class StudioPanelIds
{
    public const string LessonToolbox = "lesson-toolbox";
    public const string Tokens = "tokens";
    public const string SyntaxTree = "syntax-tree";
    public const string Diagnostics = "diagnostics";
    public const string BoundTree = "bound-tree";
    public const string Il = "il";
    public const string SharpAssembly = "sharp-assembly";
    public const string FakeMachineCode = "fake-machine-code";
    public const string WorldRuntime = "world-runtime";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        LessonToolbox,
        Tokens,
        SyntaxTree,
        Diagnostics,
        BoundTree,
        Il,
        SharpAssembly,
        FakeMachineCode,
        WorldRuntime,
    };
}
