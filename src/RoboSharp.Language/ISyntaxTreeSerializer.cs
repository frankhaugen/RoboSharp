using RoboSharp.Language.Syntax;

namespace RoboSharp.Language;

/// <summary>Human-readable dump of a compilation unit for teaching / debugging.</summary>
public interface ISyntaxTreeSerializer
{
    string Serialize(CompilationUnitSyntax root);
}