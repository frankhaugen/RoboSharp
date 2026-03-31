using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public sealed record BoundReturnStatement(
    ReturnStatementSyntax Syntax,
    BoundExpression? Expression,
    TypeSymbol FunctionReturnType) : BoundStatement;