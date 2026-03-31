namespace RoboSharp.Semantics;

public sealed record BoundCompilationUnit(
    IReadOnlyList<BoundFunctionDeclaration> Functions,
    FunctionSymbol? EntryPoint) : BoundNode;