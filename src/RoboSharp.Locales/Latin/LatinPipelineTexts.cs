namespace RoboSharp.Locales.Latin;

internal sealed class LatinPipelineTexts : IPipelineTeachingTexts
{
    public string FormatParseDiagnosticLine(int start, int length, string location, string message) =>
        $"lexica    @{start}:{length}  ({location})  {message}";

    public string FormatSemanticDiagnosticLine(int start, int length, string location, string message) =>
        $"significativa @{start}:{length}  ({location})  {message}";

    public string BoundTreeFormatFailed(string exceptionMessage) =>
        $"(Formatter docendi arborem nexam imprimere non potuit: {exceptionMessage})";

    public string ProfileHelpYouCanCall => "Hoc vocare potes:";

    public string BuildProfileHelp(string profileLabel, string worldLabel, string builtinsBody) =>
        $"Profilium lectionis: {profileLabel}\r\n" +
        $"Charta orbis: {worldLabel}\r\n\r\n" +
        $"{ProfileHelpYouCanCall}\r\n" +
        builtinsBody;

    public string WorldGridLine(int width, int height, string metadataName) =>
        $"Rete {width}×{height} ({metadataName})";

    public string WorldGoalLine(int x, int y) =>
        $"Meta in tegula ({x}, {y}) — robotem in quadratum cyanidum age ut multas lectiones impleas.";

    public string WorldRobotLine(int x, int y, string directionDisplay) =>
        $"Robot: tegula ({x}, {y}), spectans {directionDisplay}";

    public string WorldNoPrimaryRobotLine =>
        "Nullus robot id 1 in hac visione orbis (lectiones plerumque actorem 1 exspectant).";

    public string InterpreterStepLimitFault =>
        "Finis graduum interpretis excessus (claustrum salutis). Programma nimis multas instructiones IL cucurrit — vide an circulus infinitus sit.";

    public string InterpreterUnexpectedStepKind(string kindName) =>
        $"Eventus gradus interpretis inopinatus: {kindName}. (Exspectabatur perfectum, culpans, aut progressum.)";

    public string IlTraceFootnote(int instructionsExecuted, string? lastInstructionDescription)
    {
        var foot =
            "# Vestigium exsecutionis (ultimus cursus)\r\n" +
            $"Instructiones IL exsecutae: {instructionsExecuted}\r\n" +
            LatinTeachingExplainer.FakeIlVersusDotNet +
            "\r\n";
        if (lastInstructionDescription is { } d)
            foot += $"Ultima instructio gradus: {d}\r\n";
        return foot;
    }
}
