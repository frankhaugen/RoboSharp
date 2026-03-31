namespace RoboSharp.Locales.English;

/// <summary>
/// Longer teaching paragraphs about the pipeline. All copy lives in code (no .resx) so learners can
/// jump to definitions and hosts can swap <see cref="ITeachingLocale"/> for another language pack (for example <see cref="RoboSharp.Locales.Latin.LatinTeachingLocale"/>).
/// </summary>
public static class EnglishTeachingExplainer
{
    /// <summary>Why we split lexer output from the parser tree.</summary>
    public static string LexerToParser =>
        "The lexer turns characters into tokens (words and punctuation). The parser checks grammar: whether those tokens form valid RoboSharp programs. " +
        "If you see token output but a messy syntax tree, the issue is grammar — not spelling of a single word.";

    /// <summary>What “binding” means in this teaching compiler.</summary>
    public static string WhatIsBinding =>
        "Binding connects each name in your source to a symbol (variable, parameter, function, builtin). " +
        "Types are checked here: you cannot add text to a number, and function arguments must match.";

    /// <summary>Clarifies RoboSharp IL vs CLR.</summary>
    public static string FakeIlVersusDotNet =>
        "RoboSharp IL is a small instruction list made for teaching: stack, calls, and builtins. " +
        "The .NET runtime never sees it — our interpreter runs it step by step so you can watch execution.";

    /// <summary>How stdout differs from compiler diagnostics.</summary>
    public static string StdoutVsDiagnostics =>
        "stdout is what your program chose to print(). Diagnostics are the compiler or interpreter telling you something went wrong. " +
        "They use different channels on purpose so you can tell user output apart from error explanation.";

    /// <summary>Why parse failures block later phases.</summary>
    public static string PhasesDependOnEachOther =>
        "Each stage needs the previous one to succeed: semantic analysis needs a syntax tree, lowering needs a bound tree, " +
        "and the interpreter needs lowered IL. Fixing errors top-down (parse → semantic → run) is usually fastest.";

    /// <summary>What span numbers mean in diagnostics.</summary>
    public static string WhatIsASourceSpan =>
        "A span is (start index, length) in your source string, counting from zero. The editor can highlight that slice so you see exactly " +
        "which characters the compiler is talking about.";

    /// <summary>Short note on lesson profiles vs the language grammar.</summary>
    public static string ProfilesVsGrammar =>
        "The grammar of RoboSharp is fixed; lesson profiles only decide which builtins and APIs exist for this exercise. " +
        "If the compiler says a name is unknown, either spell it like the lesson expects or pick a profile that includes it.";
}
