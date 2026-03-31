using System.Reflection;
using System.Xml;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;

namespace RoboSharp.Studio.Editor;

internal static class RoboSharpSyntaxHighlighting
{
    private static readonly Lazy<IHighlightingDefinition?> Definition = new(LoadInternal);

    public static IHighlightingDefinition? Instance => Definition.Value;

    private static IHighlightingDefinition? LoadInternal()
    {
        const string name = "RoboSharp.Studio.Editor.RoboSharp.xshd";
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
        if (stream is null)
            return null;

        try
        {
            using var reader = XmlReader.Create(stream);
            return HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }
        catch
        {
            return null;
        }
    }
}
