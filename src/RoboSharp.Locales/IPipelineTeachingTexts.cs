namespace RoboSharp.Locales;

public interface IPipelineTeachingTexts
{
    string FormatParseDiagnosticLine(int start, int length, string location, string message);
    string FormatSemanticDiagnosticLine(int start, int length, string location, string message);
    string BoundTreeFormatFailed(string exceptionMessage);
    string BuildProfileHelp(string profileLabel, string worldLabel, string builtinsBody);
    string WorldGridLine(int width, int height, string metadataName);
    string WorldGoalLine(int x, int y);
    string WorldRobotLine(int x, int y, string directionDisplay);
    string WorldNoPrimaryRobotLine { get; }
    string InterpreterStepLimitFault { get; }
    string InterpreterUnexpectedStepKind(string kindName);
    string IlTraceFootnote(int instructionsExecuted, string? lastInstructionDescription);
    string ProfileHelpYouCanCall { get; }
}