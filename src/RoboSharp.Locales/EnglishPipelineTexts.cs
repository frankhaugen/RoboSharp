namespace RoboSharp.Locales;

internal sealed class EnglishPipelineTexts : IPipelineTeachingTexts
{
    public string FormatParseDiagnosticLine(int start, int length, string location, string message) =>
        $"parse     @{start}:{length}  ({location})  {message}";

    public string FormatSemanticDiagnosticLine(int start, int length, string location, string message) =>
        $"semantic  @{start}:{length}  ({location})  {message}";

    public string BoundTreeFormatFailed(string exceptionMessage) =>
        $"(The teaching formatter could not print the bound tree: {exceptionMessage})";

    public string ProfileHelpYouCanCall => "You can call:";

    public string BuildProfileHelp(string profileLabel, string worldLabel, string builtinsBody) =>
        $"Lesson profile: {profileLabel}\r\n" +
        $"World map: {worldLabel}\r\n\r\n" +
        $"{ProfileHelpYouCanCall}\r\n" +
        builtinsBody;

    public string WorldGridLine(int width, int height, string metadataName) =>
        $"{width}×{height} grid ({metadataName})";

    public string WorldGoalLine(int x, int y) =>
        $"Goal tile: ({x}, {y}) — move the robot onto the teal square to satisfy many lessons.";

    public string WorldRobotLine(int x, int y, string directionDisplay) =>
        $"Robot: tile ({x}, {y}), facing {directionDisplay}";

    public string WorldNoPrimaryRobotLine =>
        "No robot with id 1 in this world snapshot (lessons usually expect actor 1).";

    public string InterpreterStepLimitFault =>
        "Interpreter step limit exceeded (safety cap). The program ran too many IL instructions — check for an infinite loop.";

    public string InterpreterUnexpectedStepKind(string kindName) =>
        $"Unexpected interpreter step outcome: {kindName}. (Expected completed, faulted, or advanced.)";

    public string IlTraceFootnote(int instructionsExecuted, string? lastInstructionDescription)
    {
        var foot =
            "# Execution trace (last Run)\r\n" +
            $"IL instructions executed: {instructionsExecuted}\r\n" +
            TeachingExplainer.FakeIlVersusDotNet +
            "\r\n";
        if (lastInstructionDescription is { } d)
            foot += $"Last stepped instruction: {d}\r\n";
        return foot;
    }
}
