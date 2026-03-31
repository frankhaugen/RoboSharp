using RoboSharp.Language;
using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

/// <summary>Maps bound nodes to syntax spans for IL stepping metadata.</summary>
public static class BoundTeachingSyntaxSpan
{
    public static TextSpan ForExpression(BoundExpression e) =>
        e switch
        {
            BoundLiteralExpression x => TeachingSyntaxSpan.Of(x.Syntax),
            BoundVariableExpression x => TeachingSyntaxSpan.Of(x.Syntax),
            BoundConversionExpression x => ForExpression(x.Operand),
            BoundUnaryExpression x => TeachingSyntaxSpan.Of(x.Syntax),
            BoundBinaryExpression x => TeachingSyntaxSpan.Of(x.Syntax),
            BoundCallExpression x => TeachingSyntaxSpan.Of(x.Syntax),
            BoundBuiltinCallExpression x => TeachingSyntaxSpan.Of(x.Syntax),
            BoundArrayCreationExpression x => TeachingSyntaxSpan.Of(x.Syntax),
            BoundIndexExpression x => TeachingSyntaxSpan.Of(x.Syntax),
            _ => TextSpan.Invalid,
        };

    public static TextSpan ForStatement(BoundStatement s) =>
        s switch
        {
            BoundBlockStatement x => TeachingSyntaxSpan.Of(x.Syntax),
            BoundExpressionStatement x => TeachingSyntaxSpan.Of(x.Syntax),
            BoundVariableDeclarationStatement x => TeachingSyntaxSpan.Of(x.Syntax),
            BoundAssignmentStatement x => TeachingSyntaxSpan.Of(x.Syntax),
            BoundIfStatement x => TeachingSyntaxSpan.Of(x.Syntax),
            BoundWhileStatement x => TeachingSyntaxSpan.Of(x.Syntax),
            BoundReturnStatement x => TeachingSyntaxSpan.Of(x.Syntax),
            _ => TextSpan.Invalid,
        };
}
