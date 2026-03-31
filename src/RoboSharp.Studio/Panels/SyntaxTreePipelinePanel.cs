using Avalonia;
using Avalonia.Controls;
using RoboSharp.Language;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;

namespace RoboSharp.Studio.Panels;

public sealed class SyntaxTreePipelinePanel : IStudioPanel
{
    private readonly ISyntaxTreeSerializer _serializer;
    private TextBox? _text;

    public SyntaxTreePipelinePanel(ISyntaxTreeSerializer serializer) =>
        _serializer = serializer;

    public int Order => 20;

    public string DisplayName => "Syntax tree";

    public string? InspectorSubtitle =>
        "Parser output: indented tree of syntax nodes. The copyable area includes a heading so pasted text explains itself.";

    public Control CreateView()
    {
        _text = StudioCopyableText.CreateReadOnlyOutput();

        return new Border
        {
            Padding = new Thickness(8),
            Child = _text,
        };
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        if (_text is null)
            return;

        const string preamble =
            "# Syntax tree (output of the parser)\r\n" +
            "Concrete syntax: nesting shows how the parser grouped tokens into declarations, statements, and expressions.\r\n" +
            "\r\n";

        _text.Text = preamble + _serializer.Serialize(snapshot.SyntaxTree.Root);
    }
}
