namespace RoboSharp.Semantics;

/// <summary>
/// Internal IL/binder name for the function that holds top-level statements.
/// This is not a user-declared function; teaching UIs should label it as the program entry body, not as a C#-style synthetic.
/// </summary>
public static class CompilationArtifacts
{
    public const string TopLevelStatementsFunctionName = "TopLevel";
}
