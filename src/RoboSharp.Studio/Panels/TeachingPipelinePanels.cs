using Avalonia;
using Avalonia.Controls;
using RoboSharp.Studio.Pipeline;
using RoboSharp.Studio.Shell;
using RoboSharp.Toolchain;

namespace RoboSharp.Studio.Panels;

public sealed class BoundTreePipelinePanel : IStudioPanel
{
    private TextBox? _text;

    public int Order => 40;

    public string DisplayName => "Bound tree";

    public string? InspectorSubtitle =>
        "Semantic analysis: which symbol each name refers to and the type of every expression. Copy includes a short legend.";

    public Control CreateView()
    {
        _text = StudioCopyableText.CreateReadOnlyOutput();

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

        const string preamble =
            "# Bound tree (output of semantic analysis)\r\n" +
            "Names resolved to symbols, types attached — the meaning layer the IL lowering step consumes.\r\n" +
            "\r\n";

        if (snapshot.BoundTreeText is { Length: > 0 } body)
        {
            _text.Text = preamble + body;
            return;
        }

        var note = snapshot.CompileReachedPhase switch
        {
            CompilePhase.Parse =>
                "Binding runs only after a successful parse. Fix parse diagnostics first, then Build again.",
            CompilePhase.Semantics =>
                "A semantic model may exist, but bound-tree text was not produced (binding may have stopped early). See Diagnostics.",
            CompilePhase.Lowered =>
                "(No bound tree text — unexpected after lowering succeeded.)",
            _ => "No bound tree yet — Build to refresh the pipeline.",
        };

        _text.Text = preamble + note;
    }
}

public sealed class IlPipelinePanel : IStudioPanel
{
    private TextBox? _text;

    public int Order => 50;

    public string DisplayName => "IL (lowered)";

    public string? InspectorSubtitle =>
        "Fake IL disassembly: opcodes and operands the interpreter runs. Not CLR IL — RoboSharp teaching IR.";

    public Control CreateView()
    {
        _text = StudioCopyableText.CreateReadOnlyOutput(fontSize: 11);

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

        const string preamble =
            "# Fake IL (output of lowering)\r\n" +
            "Teaching instruction stream executed by the RoboSharp interpreter (stack, calls, builtins). This is not .NET IL.\r\n" +
            "\r\n";

        if (snapshot.IlDisassemblyText is { Length: > 0 } il)
        {
            _text.Text = preamble + il;
            return;
        }

        _text.Text = preamble + (snapshot.CompileReachedPhase < CompilePhase.Lowered
            ? "IL appears here after binding and lowering succeed (top-level entry lowered to TopLevel, valid types, no blocking semantic errors)."
            : "No IL text in this snapshot.");
    }
}

public sealed class WorldRuntimePipelinePanel : IStudioPanel
{
    private TextBox? _text;

    public int Order => 60;

    public string DisplayName => "World & interpreter";

    public string? InspectorSubtitle =>
        "After Run: grid snapshot, completion vs fault, print() output (stdout), and runtime/stderr lines. All sections are copyable together.";

    public Control CreateView()
    {
        _text = StudioCopyableText.CreateReadOnlyOutput();

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

        _text.Text = FormatWorldRuntimeText(snapshot);
    }

    private static string FormatWorldRuntimeText(PipelineSnapshot snapshot)
    {
        const string doc =
            "# World & interpreter\r\n" +
            "Build compiles only; Run compiles again and executes IL on the Karel world. Below, each section is labeled so you can copy/paste with context.\r\n" +
            "\r\n";

        if (snapshot.RuntimeSucceeded is null)
        {
            if (snapshot.IlDisassemblyText is { Length: > 0 })
            {
                return doc +
                    "## Execution status\r\n" +
                    "Program compiled (Build succeeded). The interpreter has not run yet for this snapshot.\r\n" +
                    "\r\n" +
                    "## What to do next\r\n" +
                    "Press Run to execute on the grid. Run recompiles, then steps IL at the speed you chose (Realtime / Slow / Glacial).\r\n" +
                    "\r\n" +
                    "## Standard output (print)\r\n" +
                    "Output from print() in your program. Not populated until after a successful Run.\r\n" +
                    "\r\n" +
                    "(not run yet)\r\n" +
                    "\r\n" +
                    "## Standard error\r\n" +
                    "Interpreter faults, step limits, and other non-print diagnostics. Not populated until after Run.\r\n" +
                    "\r\n" +
                    "(not run yet)\r\n";
            }

            return doc +
                "## Execution status\r\n" +
                "Lowering did not produce a runnable program. The interpreter will not run until compile succeeds.\r\n" +
                "\r\n" +
                "## Standard output (print)\r\n" +
                "(not available — fix compile errors first)\r\n" +
                "\r\n" +
                "## Standard error\r\n" +
                "(not available — fix compile errors first)\r\n" +
                "\r\n" +
                "Tip: add top-level statements at file scope (e.g. move();) and clear parse/semantic diagnostics.\r\n";
        }

        var worldSection =
            "## World state (after last Run)\r\n" +
            "Summary of the robot on the grid: position, facing, and related teaching fields from the world snapshot.\r\n" +
            "\r\n" +
            (string.IsNullOrWhiteSpace(snapshot.WorldAfterRunSummary)
                ? "(no summary text in snapshot)\r\n"
                : snapshot.WorldAfterRunSummary.TrimEnd() + "\r\n");

        var outcomeSection =
            "\r\n" +
            "## Interpreter outcome\r\n" +
            "Whether execution finished normally or the interpreter returned a structured fault (still not a thrown .NET exception).\r\n" +
            "\r\n" +
            (snapshot.RuntimeSucceeded == true ? "Completed without fault.\r\n" : "Faulted (see details below if present).\r\n") +
            (string.IsNullOrWhiteSpace(snapshot.RuntimeFaultMessage)
                ? ""
                : "\r\n" + snapshot.RuntimeFaultMessage.TrimEnd() + "\r\n");

        var stdoutSection =
            "\r\n" +
            "## Standard output (print)\r\n" +
            "Everything your RoboSharp program wrote using print(). This is ordinary program output, not compiler diagnostics.\r\n" +
            "\r\n" +
            (string.IsNullOrWhiteSpace(snapshot.RuntimeStdout)
                ? "(no output)\r\n"
                : snapshot.RuntimeStdout.TrimEnd() + "\r\n");

        var stderrSection =
            "\r\n" +
            "## Standard error\r\n" +
            "Interpreter channel for faults, step-limit messages, and other runtime issues — separate from print() stdout.\r\n" +
            "\r\n" +
            (string.IsNullOrWhiteSpace(snapshot.RuntimeStderr)
                ? "(none)\r\n"
                : snapshot.RuntimeStderr.TrimEnd() + "\r\n");

        return doc + worldSection + outcomeSection + stdoutSection + stderrSection;
    }
}
