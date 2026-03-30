using Microsoft.Extensions.Logging;
using RoboSharp.Language;

namespace RoboSharp.Studio.Pipeline;

public sealed class PipelineInspectionService : IPipelineInspectionService
{
    private readonly ILogger<PipelineInspectionService> _logger;

    public PipelineInspectionService(ILogger<PipelineInspectionService> logger) =>
        _logger = logger;

    public PipelineSnapshot Inspect(string source)
    {
        try
        {
            var text = SourceText.From(source);
            var tokens = Lexer.Tokenize(text);
            var tree = SyntaxTree.Parse(text);
            return new PipelineSnapshot(source, tokens, tree, tree.Diagnostics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Pipeline inspection failed");
            throw;
        }
    }
}
