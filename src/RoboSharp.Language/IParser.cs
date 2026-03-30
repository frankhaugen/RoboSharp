namespace RoboSharp.Language;

/// <summary>Parsing seam (see <see cref="SyntaxTree.Parse"/>).</summary>
public interface IParser
{
    SyntaxTree Parse(SourceText sourceText);
}
