using RoboSharp.Language.Syntax;

namespace RoboSharp.Semantics;

public abstract record BoundNode;

public abstract record BoundStatement : BoundNode;

public abstract record BoundExpression(TypeSymbol Type) : BoundNode;

public sealed record BoundCompilationUnit(
    IReadOnlyList<BoundFunctionDeclaration> Functions,
    FunctionSymbol? EntryPoint) : BoundNode;

public sealed record BoundFunctionDeclaration(
    FunctionDeclarationSyntax Syntax,
    FunctionSymbol Symbol,
    BoundBlockStatement Body) : BoundNode;

public sealed record BoundBlockStatement(
    BlockStatementSyntax Syntax,
    IReadOnlyList<BoundStatement> Statements) : BoundStatement;

public sealed record BoundVariableDeclarationStatement(
    VariableDeclarationStatementSyntax Syntax,
    LocalSymbol Symbol,
    BoundExpression Initializer) : BoundStatement;

public sealed record BoundAssignmentStatement(
    AssignmentStatementSyntax Syntax,
    LocalSymbol Symbol,
    BoundExpression Expression) : BoundStatement;

public sealed record BoundExpressionStatement(
    ExpressionStatementSyntax Syntax,
    BoundExpression Expression) : BoundStatement;

public sealed record BoundIfStatement(
    IfStatementSyntax Syntax,
    BoundExpression Condition,
    BoundStatement ThenStatement,
    BoundStatement? ElseStatement) : BoundStatement;

public sealed record BoundWhileStatement(
    WhileStatementSyntax Syntax,
    BoundExpression Condition,
    BoundStatement Body) : BoundStatement;

public sealed record BoundReturnStatement(
    ReturnStatementSyntax Syntax,
    BoundExpression? Expression,
    TypeSymbol FunctionReturnType) : BoundStatement;

public sealed record BoundLiteralExpression(
    LiteralExpressionSyntax Syntax,
    TypeSymbol Type,
    object Value) : BoundExpression(Type);

public sealed record BoundVariableExpression(
    NameExpressionSyntax Syntax,
    LocalSymbol Symbol) : BoundExpression(Symbol.Type);

public sealed record BoundBinaryExpression(
    BinaryExpressionSyntax Syntax,
    TypeSymbol Type,
    BoundExpression Left,
    BoundExpression Right) : BoundExpression(Type);

public sealed record BoundUnaryExpression(
    UnaryExpressionSyntax Syntax,
    TypeSymbol Type,
    BoundExpression Operand) : BoundExpression(Type);

public sealed record BoundCallExpression(
    CallExpressionSyntax Syntax,
    TypeSymbol Type,
    FunctionSymbol Function,
    IReadOnlyList<BoundExpression> Arguments) : BoundExpression(Type);

public sealed record BoundBuiltinCallExpression(
    CallExpressionSyntax Syntax,
    TypeSymbol Type,
    BuiltinId Builtin,
    IReadOnlyList<BoundExpression> Arguments) : BoundExpression(Type);

public sealed record BoundArrayCreationExpression(
    ArrayLiteralExpressionSyntax Syntax,
    ArrayTypeSymbol ArrayType,
    IReadOnlyList<BoundExpression> Elements) : BoundExpression(ArrayType);

public sealed record BoundIndexExpression(
    IndexExpressionSyntax Syntax,
    TypeSymbol ElementType,
    BoundExpression Target,
    BoundExpression Index) : BoundExpression(ElementType);

public sealed record BoundConversionExpression(
    BoundExpression Operand,
    TypeSymbol TargetType) : BoundExpression(TargetType);
