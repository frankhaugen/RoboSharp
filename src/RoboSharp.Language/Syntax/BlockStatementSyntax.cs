namespace RoboSharp.Language.Syntax;

public sealed record BlockStatementSyntax(
    SyntaxToken OpenBraceToken,
    IReadOnlyList<StatementSyntax> Statements,
    SyntaxToken CloseBraceToken) : StatementSyntax;

public sealed record VariableDeclarationStatementSyntax(
    TypeSyntax Type,
    SyntaxToken Identifier,
    SyntaxToken EqualsToken,
    ExpressionSyntax Initializer,
    SyntaxToken SemicolonToken) : StatementSyntax;

public sealed record AssignmentStatementSyntax(
    SyntaxToken Identifier,
    SyntaxToken EqualsToken,
    ExpressionSyntax Expression,
    SyntaxToken SemicolonToken) : StatementSyntax;

public sealed record ExpressionStatementSyntax(
    ExpressionSyntax Expression,
    SyntaxToken SemicolonToken) : StatementSyntax;

public sealed record IfStatementSyntax(
    SyntaxToken IfKeyword,
    SyntaxToken OpenParenToken,
    ExpressionSyntax Condition,
    SyntaxToken CloseParenToken,
    StatementSyntax ThenStatement,
    ElseClauseSyntax? ElseClause) : StatementSyntax;

public sealed record ElseClauseSyntax(
    SyntaxToken ElseKeyword,
    StatementSyntax Statement) : SyntaxNode;

public sealed record WhileStatementSyntax(
    SyntaxToken WhileKeyword,
    SyntaxToken OpenParenToken,
    ExpressionSyntax Condition,
    SyntaxToken CloseParenToken,
    StatementSyntax Body) : StatementSyntax;

public sealed record ReturnStatementSyntax(
    SyntaxToken ReturnKeyword,
    ExpressionSyntax? Expression,
    SyntaxToken SemicolonToken) : StatementSyntax;
