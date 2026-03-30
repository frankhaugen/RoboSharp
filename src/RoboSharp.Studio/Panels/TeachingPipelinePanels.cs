using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;
using RoboSharp.Toolchain;

namespace RoboSharp.Studio.Panels;

public sealed class BoundTreePipelinePanel : IStudioPanel
{
    private TextBlock? _text;

    public int Order => 40;

    public string DisplayName => "Bound tree";

    public Control CreateView()
    {
        _text = new TextBlock
        {
            FontFamily = StudioVisual.CodeFontFamily,
            FontSize = 12,
            Foreground = StudioVisual.TextPrimaryBrush,
            TextWrapping = TextWrapping.Wrap,
        };

        return new Border
        {
            Padding = new Thickness(4),
            Child = _text,
        };
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        if (_text is null)
            return;

        if (snapshot.BoundTreeText is { Length: > 0 } text)
        {
            _text.Text = text;
            return;
        }

        _text.Text = snapshot.CompileReachedPhase switch
        {
            CompilePhase.Parse => "Binding runs after a clean parse. Fix parse diagnostics first.",
            CompilePhase.Semantics => "Semantic model exists but tree text was empty, or binding failed before a full tree was produced. See Diagnostics.",
            CompilePhase.Lowered => "(No bound tree text — unexpected at Lowered phase.)",
            _ => "No bound tree yet.",
        };
    }
}

public sealed class IlPipelinePanel : IStudioPanel
{
    private TextBlock? _text;

    public int Order => 50;

    public string DisplayName => "IL (lowered)";

    public Control CreateView()
    {
        _text = new TextBlock
        {
            FontFamily = StudioVisual.CodeFontFamily,
            FontSize = 11,
            Foreground = StudioVisual.TextPrimaryBrush,
            TextWrapping = TextWrapping.Wrap,
        };

        return new Border
        {
            Padding = new Thickness(4),
            Child = _text,
        };
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        if (_text is null)
            return;

        if (snapshot.IlDisassemblyText is { Length: > 0 } il)
        {
            _text.Text = il;
            return;
        }

        _text.Text = snapshot.CompileReachedPhase < CompilePhase.Lowered
            ? "IL appears after successful binding and lowering (void main(), valid types, etc.)."
            : "No IL program in snapshot.";
    }
}

public sealed class WorldRuntimePipelinePanel : IStudioPanel
{
    private TextBlock? _text;

    public int Order => 60;

    public string DisplayName => "World & interpreter";

    public Control CreateView()
    {
        _text = new TextBlock
        {
            FontFamily = StudioVisual.CodeFontFamily,
            FontSize = 12,
            Foreground = StudioVisual.TextPrimaryBrush,
            TextWrapping = TextWrapping.Wrap,
        };

        return new Border
        {
            Padding = new Thickness(4),
            Child = _text,
        };
    }

    public void OnSnapshotChanged(PipelineSnapshot snapshot)
    {
        if (_text is null)
            return;

        if (snapshot.RuntimeSucceeded is null)
        {
            if (snapshot.IlDisassemblyText is { Length: > 0 })
            {
                _text.Text =
                    "Program compiled (Build). Press Run to execute on the grid — Run compiles again, then steps the interpreter at the chosen speed.";
            }
            else
            {
                _text.Text =
                    "Interpreter runs only when lowering succeeds. Use void main() { … } and fix semantic errors.";
            }

            return;
        }

        var parts = new List<string>();
        if (snapshot.WorldAfterRunSummary is not null)
        {
            parts.Add("── World ──");
            parts.Add(snapshot.WorldAfterRunSummary);
        }

        parts.Add("── Run ──");
        parts.Add(snapshot.RuntimeSucceeded == true ? "Completed without fault." : "Faulted.");
        if (!string.IsNullOrWhiteSpace(snapshot.RuntimeFaultMessage))
            parts.Add(snapshot.RuntimeFaultMessage);

        if (!string.IsNullOrWhiteSpace(snapshot.RuntimeStdout))
        {
            parts.Add("── stdout ──");
            parts.Add(snapshot.RuntimeStdout.TrimEnd());
        }

        if (!string.IsNullOrWhiteSpace(snapshot.RuntimeStderr))
        {
            parts.Add("── stderr ──");
            parts.Add(snapshot.RuntimeStderr.TrimEnd());
        }

        _text.Text = string.Join(Environment.NewLine, parts);
    }
}
