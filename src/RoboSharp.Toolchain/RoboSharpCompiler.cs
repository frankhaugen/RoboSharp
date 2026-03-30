using RoboSharp.IL;
using RoboSharp.Language;
using RoboSharp.Semantics;

namespace RoboSharp.Toolchain;

/// <summary>Phase-ordered in-memory compile (single compilation unit). Matches <c>docs/compiler/v1-compiler-spec.md</c> at a teaching granularity.</summary>
public static class RoboSharpCompiler
{
    public static CompileResult Compile(string source, IBuiltinProfileProvider? builtinProfile = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        builtinProfile ??= new FullBuiltinProfileProvider();

        var syntaxTree = SyntaxTree.Parse(SourceText.From(source));
        if (syntaxTree.Diagnostics.Count > 0)
        {
            return new CompileResult
            {
                SyntaxTree = syntaxTree,
                ReachedPhase = CompilePhase.Parse,
            };
        }

        var model = new Binder(builtinProfile).Bind(syntaxTree.Root);
        if (model.Diagnostics.Count > 0 || model.Root.EntryPoint is null)
        {
            return new CompileResult
            {
                SyntaxTree = syntaxTree,
                SemanticModel = model,
                ReachedPhase = CompilePhase.Semantics,
            };
        }

        var program = new IlLowerer().Lower(model.Root);
        var executable = RoboExecutable.FromProgram(program);

        return new CompileResult
        {
            SyntaxTree = syntaxTree,
            SemanticModel = model,
            Program = program,
            Executable = executable,
            ReachedPhase = CompilePhase.Lowered,
        };
    }
}

public enum CompilePhase
{
    Parse,
    Semantics,
    Lowered,
}

public sealed class CompileResult
{
    public SyntaxTree? SyntaxTree { get; init; }
    public SemanticModel? SemanticModel { get; init; }
    public RoboProgram? Program { get; init; }
    public RoboExecutable? Executable { get; init; }
    public CompilePhase ReachedPhase { get; init; }

    public bool Succeeded => Executable is not null && ReachedPhase == CompilePhase.Lowered;
}
