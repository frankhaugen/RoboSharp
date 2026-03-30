namespace RoboSharp.Language;

/// <summary>Default <see cref="IParser"/> using <see cref="SyntaxTree.Parse"/>.</summary>
public sealed class RoboParser : IParser
{
    public SyntaxTree Parse(SourceText sourceText) => SyntaxTree.Parse(sourceText);
}
