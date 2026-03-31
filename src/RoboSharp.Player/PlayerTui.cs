using System.Text;
using RoboSharp.Application;
using Spectre.Console;

namespace RoboSharp.Player;

internal static class PlayerTui
{
    public static void ShowResult(
        string roboExePath,
        ProgramRunResult result,
        string stdoutText,
        string stderrText,
        string worldAscii,
        int? maxSteps)
    {
        AnsiConsole.Write(new Rule("[bold cyan]RoboSharp Player[/]") { Justification = Justify.Left });
        AnsiConsole.MarkupLine($"[grey]Artifact:[/] [white]{Markup.Escape(roboExePath)}[/]");
        if (maxSteps is { } cap)
            AnsiConsole.MarkupLine($"[grey]Instruction cap:[/] [white]{cap}[/]");

        var root = new Layout("root")
            .SplitColumns(
                new Layout("world")
                {
                    Ratio = 2,
                    MinimumSize = 24,
                },
                new Layout("io")
                {
                    Ratio = 3,
                });

        root["world"].Update(
            new Panel(new Text(worldAscii, new Style(foreground: Color.Aquamarine1)))
            {
                Header = new PanelHeader("[bold]World after run[/] (# wall, G goal, . floor, ^>v< robot)"),
                Border = BoxBorder.Rounded,
            });

        root["io"].SplitRows(
            new Layout("stdout"),
            new Layout("stderr"),
            new Layout("status"));

        var outBody = string.IsNullOrWhiteSpace(stdoutText)
            ? "[grey](no stdout)[/]"
            : $"[white]{Markup.Escape(stdoutText.TrimEnd())}[/]";
        root["io"]["stdout"].Update(
            new Panel(new Markup(outBody))
            {
                Header = new PanelHeader("[bold]Program stdout[/] (print)"),
                Border = BoxBorder.Rounded,
            });

        var errBody = string.IsNullOrWhiteSpace(stderrText)
            ? "[grey](none)[/]"
            : $"[yellow]{Markup.Escape(stderrText.TrimEnd())}[/]";
        root["io"]["stderr"].Update(
            new Panel(new Markup(errBody))
            {
                Header = new PanelHeader("[bold]Stderr[/] (faults / limits)"),
                Border = BoxBorder.Rounded,
            });

        var status = new StringBuilder();
        status.Append(result.Succeeded ? "[green]Exit: success[/]" : "[red]Exit: fault or build issue[/]");
        status.AppendLine();
        status.Append($"[grey]Code:[/] [white]{result.ExitCode}[/]");
        if (result.Fault is { } f)
        {
            status.AppendLine();
            status.Append($"[red]{Markup.Escape(f.Message)}[/]");
        }

        root["io"]["status"].Update(
            new Panel(new Markup(status.ToString()))
            {
                Header = new PanelHeader("[bold]Outcome[/]"),
                Border = BoxBorder.Rounded,
            });

        AnsiConsole.Write(root);
        AnsiConsole.WriteLine();
    }
}
