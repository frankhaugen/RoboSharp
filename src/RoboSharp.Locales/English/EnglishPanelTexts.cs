namespace RoboSharp.Locales.English;

internal sealed class EnglishPanelTexts : IStudioPanelTexts
{
    public string ColoredSourceTitle => "Syntax colors";
    public string ColoredSourceSubtitle =>
        "Keywords, literals, and comments tinted from the last Build. The main editor updates live; this panel is a lexer snapshot when you Build.";
    public string ColoredSourcePreamble =>
        "# Syntax colors (lexer snapshot)\r\n" +
        EnglishTeachingExplainer.LexerToParser +
        "\r\n\r\nThis panel shows the same source text with colors from the lexer. Press Build to refresh after edits.\r\n\r\n";
    public string ColoredSourceEmpty => "(Nothing to show — source is empty or Build has not run yet.)";

    public string LessonToolboxTitle => "Lesson toolbox";
    public string LessonToolboxSubtitle =>
        "Built-ins allowed by the profile you chose. Smaller profiles keep puzzles focused; unknown names usually mean the profile, not a typo in the grammar.";
    public string LessonToolboxPreamble =>
        "# Lesson toolbox\r\n" +
        EnglishTeachingExplainer.ProfilesVsGrammar +
        "\r\n\r\nThe list below matches the Lesson profile dropdown.\r\n\r\n";
    public string LessonToolboxBuildPrompt =>
        "(Build once to load the active profile and world labels into the snapshot.)";

    public string TokensTitle => "Tokens";
    public string TokensSubtitle =>
        "Lexer output: one line per token (kind, start index, length, escaped text). Copy with Ctrl+A, Ctrl+C inside the box.";
    public string TokensPreamble =>
        "# Lexer tokens (lexical analysis)\r\n" +
        "Each line is one token: kind, @start character index, length, and the exact characters (\\r, \\n, \\t shown escaped).\r\n" +
        EnglishTeachingExplainer.LexerToParser +
        "\r\n\r\n";

    public string SyntaxTreeTitle => "Syntax tree";
    public string SyntaxTreeSubtitle =>
        "Parser output: nested syntax nodes. Good tree + diagnostics usually means the next step is binding, not more lexer tweaks.";
    public string SyntaxTreePreamble =>
        "# Syntax tree (parsing)\r\n" +
        "Concrete syntax: indentation shows how tokens were grouped into declarations, statements, and expressions.\r\n" +
        EnglishTeachingExplainer.PhasesDependOnEachOther +
        "\r\n\r\n";

    public string DiagnosticsTitle => "Diagnostics";
    public string DiagnosticsSubtitle =>
        "Parse, semantic (binder), and runtime messages from the last Build or Run. Each line is labeled by phase so you know which compiler part spoke.";
    public string DiagnosticsPreamble =>
        "# Diagnostics (compiler & interpreter)\r\n" +
        EnglishTeachingExplainer.WhatIsASourceSpan +
        "\r\n\r\n" +
        "• parse — lexer/parser could not produce a valid syntax tree.\r\n" +
        "• semantic — binder/type rules failed after a successful parse. " + EnglishTeachingExplainer.WhatIsBinding + "\r\n" +
        "• runtime — interpreter reported a fault while executing lowered IL (after Run). " + EnglishTeachingExplainer.StdoutVsDiagnostics + "\r\n" +
        "\r\n";

    public string DiagnosticsNone =>
        "(No diagnostics in these phases — last Build/Run did not report parse, semantic, or runtime faults here.)";

    public string DiagnosticsRuntimePrefix => "runtime   ";

    public string BoundTreeTitle => "Bound tree";
    public string BoundTreeSubtitle =>
        "Semantic analysis: each name resolved to a symbol, every expression typed. This is what lowering turns into fake IL.";
    public string BoundTreePreamble =>
        "# Bound tree (semantic analysis)\r\n" +
        EnglishTeachingExplainer.WhatIsBinding +
        "\r\n\r\nThe dump below is the meaning layer the IL lowering step consumes.\r\n\r\n";

    public string BoundTreeNeedParseFirst =>
        "Binding runs only after a successful parse. Fix parse diagnostics first (syntax tree panel + this panel’s heading), then Build again.";
    public string BoundTreeSemanticsStopped =>
        "Semantic analysis stopped early — see Diagnostics for binder messages. Often a type mismatch or unknown name in the profile.";
    public string BoundTreeUnexpectedEmpty =>
        "(No bound tree text even though lowering reported success — unexpected; check Diagnostics and file an issue if it persists.)";
    public string BoundTreeBuildPrompt => "Build to refresh the pipeline and populate this panel when compile succeeds.";

    public string IlTitle => "IL (lowered)";
    public string IlSubtitle =>
        "Fake IL disassembly: opcodes and operands the interpreter executes. Not CLR IL — see explainer in the trace footnote after Run.";
    public string IlPreamble =>
        "# Fake IL (lowering)\r\n" +
        EnglishTeachingExplainer.FakeIlVersusDotNet +
        "\r\n\r\n";

    public string IlWaitingForLowering =>
        "IL appears after binding and lowering succeed: valid types, resolved names, and a runnable entry. Clear semantic diagnostics, then Build.";
    public string IlNoTextUnexpected =>
        "(Lowering reported success but IL text is missing — unexpected. Try Build again and check Diagnostics.)";

    public string IlCopyDisassembly => "Copy IL text";

    public string WorldRuntimeTitle => "World & interpreter";
    public string WorldRuntimeSubtitle =>
        "After Run: world summary, completion vs fault, print() stdout, and stderr. Sections are labeled so pasted copies stay understandable.";
    public string FormatWorldRuntimePanel(
        bool? runtimeSucceeded,
        bool hasRunnableIl,
        string? lessonOutcome,
        int? lessonScore,
        string? worldAfterRunSummary,
        string? runtimeFaultMessage,
        string? runtimeStdout,
        string? runtimeStderr)
    {
        var doc =
            "# World & interpreter\r\n" +
            "Build compiles only. Run compiles again, then executes IL on the grid step by step.\r\n" +
            EnglishTeachingExplainer.StdoutVsDiagnostics +
            "\r\n\r\n";

        if (runtimeSucceeded is null)
        {
            if (hasRunnableIl)
            {
                return doc +
                    "## Execution status\r\n" +
                    "The program compiled successfully for this snapshot, but the interpreter has not run yet.\r\n" +
                    "\r\n" +
                    "## What to do next\r\n" +
                    "Press Run. Studio recompiles, then steps IL at the speed you chose so you can watch the robot.\r\n" +
                    "\r\n" +
                    "## Standard output (print)\r\n" +
                    "Output from print() in your program. Filled in after a successful Run.\r\n" +
                    "\r\n" +
                    "(not run yet)\r\n" +
                    "\r\n" +
                    "## Standard error\r\n" +
                    "Interpreter faults, step limits, and other runtime issues — separate from print().\r\n" +
                    "\r\n" +
                    "(not run yet)\r\n";
            }

            return doc +
                "## Execution status\r\n" +
                "Lowering did not produce a runnable program yet, so Run cannot execute meaningful IL.\r\n" +
                "\r\n" +
                "## Standard output (print)\r\n" +
                "(not available until compile succeeds)\r\n" +
                "\r\n" +
                "## Standard error\r\n" +
                "(not available until compile succeeds)\r\n" +
                "\r\n" +
                "Tip: add top-level calls at file scope (for example move();) and clear parse/semantic diagnostics. " +
                EnglishTeachingExplainer.PhasesDependOnEachOther +
                "\r\n";
        }

        var goalSection =
            lessonOutcome is { } lo
                ? "## Goal & score\r\n" +
                  "Lesson feedback: what happened at the goal and a simple score for practice.\r\n\r\n" +
                  lo.TrimEnd() + "\r\n" +
                  (lessonScore is { } sc ? $"\r\nScore: {sc}\r\n" : "\r\n")
                : "";

        var worldSection =
            "\r\n## World state (after last Run)\r\n" +
            "Robot position, facing direction, and map metadata from the world snapshot.\r\n" +
            "\r\n" +
            (string.IsNullOrWhiteSpace(worldAfterRunSummary)
                ? "(no summary text in snapshot)\r\n"
                : worldAfterRunSummary.TrimEnd() + "\r\n");

        var outcomeSection =
            "\r\n" +
            "## Interpreter outcome\r\n" +
            "Structured result from the interpreter (not a thrown .NET exception — RoboSharp reports faults as data).\r\n" +
            "\r\n" +
            (runtimeSucceeded == true
                ? "Completed without fault.\r\n"
                : "Stopped with a fault — read the message below and Diagnostics for compile-time issues.\r\n") +
            (string.IsNullOrWhiteSpace(runtimeFaultMessage)
                ? ""
                : "\r\n" + runtimeFaultMessage.TrimEnd() + "\r\n");

        var stdoutSection =
            "\r\n" +
            "## Standard output (print)\r\n" +
            "Everything your program wrote with print().\r\n" +
            "\r\n" +
            (string.IsNullOrWhiteSpace(runtimeStdout)
                ? "(no output)\r\n"
                : runtimeStdout.TrimEnd() + "\r\n");

        var stderrSection =
            "\r\n" +
            "## Standard error\r\n" +
            "Runtime channel for faults and limits — not the same channel as print().\r\n" +
            "\r\n" +
            (string.IsNullOrWhiteSpace(runtimeStderr)
                ? "(none)\r\n"
                : runtimeStderr.TrimEnd() + "\r\n");

        return doc + goalSection + worldSection + outcomeSection + stdoutSection + stderrSection;
    }
}
