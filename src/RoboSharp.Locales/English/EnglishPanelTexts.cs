namespace RoboSharp.Locales.English;

internal sealed class EnglishPanelTexts : IStudioPanelTexts
{
    public string LessonToolboxTitle => "Lesson toolbox";
    public string LessonToolboxSubtitle =>
        "Names the compiler will accept for this lesson — tied to the lesson profile, not every word RoboSharp might ever learn.";
    public string LessonToolboxLead => "What you’re allowed to call";
    public string LessonToolboxGuide =>
        "This list is the **profile**: built-ins and types your program may use right now. " +
        EnglishTeachingExplainer.ProfilesVsGrammar +
        " After **Build**, the lines below are the live checklist from that profile.";
    public string LessonToolboxFooter =>
        "If a name you expect is missing, you’re probably ahead of the lesson — switch ribbon lessons or ask for a wider profile in a later track.";
    public string LessonToolboxBuildPrompt =>
        "(Build once to load the active profile into this snapshot.)";

    public string TokensTitle => "Tokens";
    public string TokensSubtitle =>
        "The lexer’s output: how plain text became a stream of labeled pieces before the parser runs.";
    public string TokensLead => "Lexical analysis";
    public string TokensGuide =>
        "The **lexer** walks your source left-to-right and groups characters into **tokens** — keywords, names, numbers, punctuation, and comments. " +
        "Each row below is one token: its kind, where it starts in the file, how long it is, and the exact text (with \\r, \\n, \\t escaped so spaces stay visible).\r\n\r\n" +
        EnglishTeachingExplainer.LexerToParser;
    public string TokensColumnHeader => "kind                      @start  len   text";
    public string TokensFootnote =>
        "Tokens are a snapshot from the last **Build**. Edit the editor, then Build again to refresh. Weird tokens often mean a stray character the lexer had to mark as “bad.”";

    public string SyntaxTreeTitle => "Syntax tree";
    public string SyntaxTreeSubtitle =>
        "How the parser grouped tokens into the grammar’s shapes — procedures, blocks, expressions.";
    public string SyntaxTreeLead => "Parsing";
    public string SyntaxTreeGuide =>
        "The **parser** reads the token stream and builds a **syntax tree**: nested nodes that match RoboSharp’s grammar. " +
        "Indentation in the dump shows parent/child structure. A clean tree here usually means “syntax is fine; if something’s still wrong, look at binding or types next,” not more lexer tweaks.\r\n\r\n" +
        EnglishTeachingExplainer.PhasesDependOnEachOther;
    public string SyntaxTreeFootnote =>
        "The tree is read-only text you can copy. It’s the same structure the binder walks in the next stage.";

    public string DiagnosticsTitle => "Compiler messages";
    public string DiagnosticsSubtitle =>
        "Parse, semantic, and runtime lines from the last Build or Run — each tagged by which part of the pipeline spoke.";
    public string DiagnosticsPreamble =>
        "# Compiler messages\r\n" +
        EnglishTeachingExplainer.WhatIsASourceSpan +
        "\r\n\r\n" +
        "• **parse** — lexer/parser could not build a valid syntax tree.\r\n" +
        "• **semantic** — binder/type rules failed after a good parse. " + EnglishTeachingExplainer.WhatIsBinding + "\r\n" +
        "• **runtime** — interpreter hit a fault while running lowered IL. " + EnglishTeachingExplainer.StdoutVsDiagnostics + "\r\n" +
        "\r\n";

    public string DiagnosticsNone =>
        "(No messages in these phases — last Build/Run did not report parse, semantic, or runtime faults here.)";

    public string DiagnosticsRuntimePrefix => "runtime   ";

    public string DiagnosticsLead => "Compile-time & runtime signals";
    public string DiagnosticsGuide =>
        "Parse, semantic, and runtime lines from the last Build or Run — each tagged by which part of the pipeline spoke.";
    public string DiagnosticsFooter =>
        "Tip: **View → Compiler messages…** opens this list in its own window when you want more room.";

    public string BoundTreeTitle => "Bound tree";
    public string BoundTreeSubtitle =>
        "Semantic analysis: every name tied to a symbol, every expression given a type — the input to lowering.";
    public string BoundTreeLead => "Binding & types";
    public string BoundTreeGuide =>
        "The **binder** resolves names to declarations, checks types, and attaches meaning to the syntax tree. " +
        EnglishTeachingExplainer.WhatIsBinding +
        "\r\n\r\nWhat you see below is the **bound** program: the layer the IL lowering step consumes. When this text appears, the compiler understood your program’s meaning well enough to try generating instructions.";
    public string BoundTreeFootnote =>
        "If this panel is empty or shows a short note, fix the issue it describes, then Build again. Use **View → Compiler messages** for the full diagnostic list.";
    public string BoundTreeNeedParseFirst =>
        "Binding only runs after a successful parse. Fix parse messages first (syntax tree + Compiler messages), then Build again.";
    public string BoundTreeSemanticsStopped =>
        "Semantic analysis stopped early — open **View → Compiler messages** for binder details (unknown names, wrong types, profile limits).";
    public string BoundTreeUnexpectedEmpty =>
        "(No bound tree text even though lowering reported success — unexpected; check Compiler messages and report if it persists.)";
    public string BoundTreeBuildPrompt => "Build to refresh the pipeline and populate this panel when compile succeeds.";

    public string IlTitle => "IL (lowered)";
    public string IlSubtitle =>
        "Fake instruction list the interpreter steps through — not CLR IL; see the guide and footnote after Run.";
    public string IlLead => "Lowering";
    public string IlGuide =>
        EnglishTeachingExplainer.FakeIlVersusDotNet +
        "\r\n\r\nEach instruction is one step the teaching interpreter executes. While **Run** is stepping, the current line highlights so you can connect IL motion to the robot.";

    public string IlWaitingForLowering =>
        "IL appears only after binding and lowering succeed: valid types, resolved names, and a runnable entry. Clear semantic issues, then Build.";
    public string IlNoTextUnexpected =>
        "(Lowering reported success but IL text is missing — unexpected. Try Build again and check Compiler messages.)";

    public string IlCopyDisassembly => "Copy IL text";

    public string SharpAssemblyTitle => "SharpAssembly (teaching)";
    public string SharpAssemblySubtitle =>
        "IL rewritten as mnemonic assembly — same steps, syntax that looks closer to “real” asm (still not a CPU).";
    public string SharpAssemblyLead => "Below IL in the ladder";
    public string SharpAssemblyGuide =>
        "**SharpAssembly** is a readable spelling of the same RoboSharp IL the interpreter runs. " +
        "It is not CLR IL and not tied to x86/ARM. Use it to practice reading operands and control flow after you understand the IL panel.";
    public string SharpAssemblyFooter =>
        "Compare line-by-line with **IL (lowered)** — counts and jump targets should match.";
    public string SharpAssemblyWaitingForProgram =>
        "(No lowered program yet — fix **Compiler messages**, then Build so IL and SharpAssembly can appear.)";

    public string FakeMachineCodeTitle => "Machine words (teaching)";
    public string FakeMachineCodeSubtitle =>
        "Synthetic 32-bit hex per instruction — shows how “bytes on the wire” might look without teaching a real ISA.";
    public string FakeMachineCodeLead => "Encoding (not functional hardware)";
    public string FakeMachineCodeGuide =>
        "Each **word** is a deterministic fake encoding of opcode + operands for this teaching VM. " +
        "Real CPUs use different layouts; this is only to connect “hex dumps” to the instructions you already see in IL.";
    public string FakeMachineCodeFooter =>
        "The right-hand columns echo IL so you can verify the mapping; the hex is for classroom intuition only.";
    public string FakeMachineCodeWaitingForProgram =>
        "(No lowered program yet — Build after compile succeeds to fill machine words.)";

    public string WorldRuntimeTitle => "Run report";
    public string WorldRuntimeSubtitle =>
        "After Run: goal feedback, world summary, print() output, stderr, and interpreter outcome — opened from the View menu.";
    public string WorldRuntimeLead => "Execution snapshot";
    public string WorldRuntimeFooter =>
        "You can also open **View → Run report…** in its own window when this stack feels crowded.";
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
            "# Run report\r\n" +
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
                "Tip: add top-level calls at file scope (for example move();) and clear parse/semantic messages. " +
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
                : "Stopped with a fault — read the message below and **View → Compiler messages** for compile-time issues.\r\n") +
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
