using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed record BoundFunctionDeclaration(
    FunctionDeclarationSyntax Syntax,
    FunctionSymbol Symbol,
    BoundBlockStatement Body) : BoundNode;